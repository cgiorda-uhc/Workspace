using AutoMapper;
using ClosedXML.Excel;
using Dapper;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using DataAccessLibrary.Scripts;
using DataAccessLibrary.Shared;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSExcel;
using FileParsingLibrary.MSExcel.Custom.ProcCodeTrends;
using FileParsingLibrary.MSExcel.Custom.TAT;
using FileParsingLibrary.MSWord;
using MathNet.Numerics.Providers.SparseSolver;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Utilities;
using ProjectManagerLibrary.Models;
using ProjectManagerLibrary.Shared;
using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VCPortal_Models.Models.DQC_Reporting;
using VCPortal_Models.Models.EBM;
using VCPortal_Models.Models.ETGFactSymmetry.Configs;
using VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
using VCPortal_Models.Models.PEG;
using VCPortal_Models.Models.Report_Timeliness;
using VCPortal_Models.Models.Shared;
using VCPortal_Models.Models.TAT;
using VCPortal_Models.Parameters.EDCAdhoc;
using VCPortal_Models.Parameters.MHP;

namespace ConsoleLibraryTesting
{
    public class AdHoc
    {


        public string PEGReportTemplatePath { get; set; }
        public string EBMReportTemplatePath { get; set; }

        public string TATReportTemplatePath { get; set; }



        public string ConnectionStringVC { get; set; }

        public string ConnectionStringMSSQL { get; set; }


        public string ConnectionStringUHPD { get; set; }

        public string ConnectionStringPD { get; set; }


        public string ConnectionStringSnowflake { get; set; }


        public string ConnectionStringUHN { get; set; }


        public string ConnectionStringNDAR { get; set; }
        public string ConnectionStringGalaxy { get; set; }

        public string TableMHP { get; set; }
        public string ConnectionStringTD { get; set; }
        public string TableUGAP { get; set; }
        public int Limit { get; set; }



        private Timer _timer = new Timer(TimerCallback, null, 0, 1);
        private static Stopwatch _stop_watch = new Stopwatch();
        private static string _console_message;

        private static void TimerCallback(Object o)
        {

            if (!string.IsNullOrEmpty(_console_message))
            {
                var time_span = _stop_watch.Elapsed;
                var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                time_span.Hours, time_span.Minutes, time_span.Seconds,
                time_span.Milliseconds / 10);


                Console.Write("\r" + _console_message + " : " + elapsedTime);
            }

        }

        private static DateTime getLastChosenDayOfTheMonth(DateTime date, DayOfWeek dayOfWeek)
        {
            var lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            while (lastDayOfMonth.DayOfWeek != dayOfWeek)
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);

            return lastDayOfMonth;
        }



        //MHP UGAP CLEAN
        public async Task cleanupMemberDataAsync(List<string> files_loaded)
        {

            var files_csv = "'" + string.Join("','", files_loaded.Select(n => n.ToString()).ToArray()) + "'";


            //TWO DBS
            IRelationalDataAccess db_td = new TeraDataAccess();
            IRelationalDataAccess db_sql = new SqlDataAccess();

            //DRIVING LOOP
            var parameters = MHPCustomSQL.MHPParameters();

            string sql;
            StringBuilder sbSQL = new StringBuilder();

            int total;
            int total_counter;
            int limit_counter;
            var columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();
            foreach (var param in parameters)
            {
                sql = MHPCustomSQL.MSSQLMHPMember(TableMHP, TableUGAP, files_csv, param.MHPSQL);
                //FIND CURRENT MEMBERS
                var mhp_search = (await db_sql.LoadData<MHPMemberSearchModel>(connectionString: ConnectionStringMSSQL, sql));
                total = mhp_search.Count();
                Console.WriteLine(total + " records found for  SM:" + param.SearchMethod + "   LOS:" + param.LOS + "");
                total_counter = 0;
                limit_counter = 0;

                foreach (var m in mhp_search)
                {
                    sbSQL.Append(MHPCustomSQL.UGAPVolatileInsert(m, param));
                    limit_counter++;
                    total_counter++;
                    if (limit_counter == Limit)
                    {
                        Console.WriteLine("Searching UGAP for " + total_counter + " out of " + total);
                        if (param.LOS == LOS.EI || param.LOS == LOS.EI_OX)
                            sql = MHPCustomSQL.UGAPSQLLMemberDataEI(param.UGAPSQL, param.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
                        else
                            sql = MHPCustomSQL.UGAPSQLMemberDataCS(param.UGAPSQL, param.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

                        var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: ConnectionStringTD, sql);
                        foreach (var u in ugap)
                        {
                            u.SearchMethod = param.SearchMethod;
                        }

                        Console.WriteLine("Loading " + ugap.Count() + " UGAP rows into MHP source.");
                        await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: ConnectionStringMSSQL, TableUGAP, ugap, columns);



                        sbSQL.Remove(0, sbSQL.Length);
                        limit_counter = 0;
                    }
                }
                //FINISHED BEFORE LIMIT SO PROCESS REMAINDER
                if (sbSQL.Length > 0)
                {
                    Console.WriteLine("Searching UGAP for " + total_counter + " out of " + total);

                    if (param.LOS == LOS.EI || param.LOS == LOS.EI_OX)
                        sql = MHPCustomSQL.UGAPSQLLMemberDataEI(param.UGAPSQL, param.LOS == LOS.EI_OX).Replace("{$Inserts}", sbSQL.ToString());
                    else
                        sql = MHPCustomSQL.UGAPSQLMemberDataCS(param.UGAPSQL, param.LOS == LOS.CS).Replace("{$Inserts}", sbSQL.ToString());

                    var ugap = await db_td.LoadData<MHPMemberDetailsModel>(connectionString: ConnectionStringTD, sql);
                    foreach (var u in ugap)
                    {
                        u.SearchMethod = param.SearchMethod;
                    }

                    Console.WriteLine("Loading " + ugap.Count() + " UGAP rows into MHP source.");
                    await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: ConnectionStringMSSQL, TableUGAP, ugap, columns);

                    sbSQL.Remove(0, sbSQL.Length);

                }

            }


            await db_sql.Execute(ConnectionStringMSSQL, "exec [IL_UCA].[dbo].[sp_mhp_refesh_filter_cache]");

        }


        //MHP ILUCA TO VC
        public async Task transferMHPDataAsync(List<string> files_loaded)
        {



            //TWO DBS
            IRelationalDataAccess db_td = new TeraDataAccess();
            IRelationalDataAccess db_sql = new SqlDataAccess();


            StringBuilder sb = new StringBuilder();

            foreach (string file in files_loaded)
            {
                sb.Append("'" + file + "',");
            }

            string strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Yearly_Universes]  WHERE file_name in (" + sb.ToString().TrimEnd(',') + ");";
            var mhp = await db_sql.LoadData<MHPUniverseModel>(connectionString: ConnectionStringMSSQL, strSQL);
            var columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<MHPUniverseModel>(connectionString: ConnectionStringVC, "mhp.MHP_Yearly_Universes", mhp, columns);

            strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] WHERE mhp_uni_id in (SELECT [mhp_uni_id] FROM [IL_UCA].[stg].[MHP_Yearly_Universes] WHERE file_name in (" + sb.ToString().TrimEnd(',') + "));";
            var mhp_ugap = await db_sql.LoadData<MHPMemberDetailsModel>(connectionString: ConnectionStringMSSQL, strSQL);
            columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: ConnectionStringVC, "mhp.MHP_Yearly_Universes_UGAP", mhp_ugap, columns);


            strSQL = "SELECT * FROM  [IL_UCA].[dbo].[cs_product_map];";
            var pm = await db_sql.LoadData<CS_Product_Map>(connectionString: ConnectionStringMSSQL, strSQL);
            columns = typeof(CS_Product_Map).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<CS_Product_Map>(connectionString: ConnectionStringVC, "vct.cs_product_map", pm, columns, truncate: true);


            strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Group_State];";
            var gs = await db_sql.LoadData<MHP_Group_State_Model>(connectionString: ConnectionStringMSSQL, strSQL);
            columns = typeof(MHP_Group_State_Model).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<MHP_Group_State_Model>(connectionString: ConnectionStringVC, "mhp.MHP_Group_State", gs, columns, truncate: true);


            strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Universes_Filter_Cache];";
            var fs = await db_sql.LoadData<MHP_Reporting_Filters>(connectionString: ConnectionStringMSSQL, strSQL);
            columns = typeof(MHP_Reporting_Filters).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<MHP_Reporting_Filters>(connectionString: ConnectionStringVC, "mhp.MHP_Universes_Filter_Cache", fs, columns, truncate: true);


            await SharedFunctions.EmailAsync("jon.piotrowski@uhc.com;renee_l_struck@uhc.com;hong_gao@uhc.com", "chris_giordano@uhc.com", "MHPUniverse February 2024 was refreshed", "MHPUniverse February 2024 was refreshed", "chris_giordano@uhc.com;laura_fischer@uhc.com;inna_rudi@uhc.com", null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
        }



        //ETG SYMM SOURCE AUTOMATION
        public async Task getETGSymmSourceDataAsync(float version)
        {
            //ETG DATA LOAD
            //ETG DATA LOAD
            //ETG DATA LOAD

            List<ETGVersion_Model> v = new List<ETGVersion_Model>();

            v.Add(new ETGVersion_Model() { PD_Version = 18, Year = 2024 });
            v.Add(new ETGVersion_Model() { PD_Version = 17, Year = 2023 });
            v.Add(new ETGVersion_Model() { PD_Version = 16, Year = 2022 }); //RUN
            v.Add(new ETGVersion_Model() { PD_Version = 15, Year = 2021 });
            v.Add(new ETGVersion_Model() { PD_Version = 14, Year = 2020 });

            var year = v.Where(x => x.PD_Version == version).Select(x => x.Year).FirstOrDefault();


            IRelationalDataAccess db_sql = new SqlDataAccess();
            IRelationalDataAccess db_td = new TeraDataAccess();

            //STEP 1 etg.NRX_Cost_UGAP_SOURCE
            string strSQL = "select ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD, Count(Distinct ETG_D.INDV_SYS_ID) as MEMBER_COUNT, Count(Distinct ETG_D.EPSD_NBR) as EPSD_COUNT, Sum(ETG_D.TOT_ALLW_AMT) as ETGD_TOT_ALLW_AMT, Sum(ETG_D.RX_ALLW_AMT) as ETGD_RX_ALLW_AMT, case when Sum(ETG_D.TOT_ALLW_AMT) = 0 then 0 else NVL(Sum(ETG_D.RX_ALLW_AMT), 0) / Sum(ETG_D.TOT_ALLW_AMT) end as RX_RATE from ( select ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND as TRT_CD, Sum(ED1.QLTY_INCNT_RDUC_AMT) as TOT_ALLW_AMT, Query1.RX_ALLW_AMT from CLODM001.ETG_DETAIL ED1 inner join CLODM001.ETG_NUMBER EN1 on ED1.ETG_SYS_ID = EN1.ETG_SYS_ID inner join CLODM001.DATE_FST_SRVC DFS1 on ED1.FST_SRVC_DT_SYS_ID = DFS1.FST_SRVC_DT_SYS_ID inner join ( select C.INDV_SYS_ID from ( select B.INDV_SYS_ID, Min(B.PHRM_BEN_FLG) as MIN_PHARMACY_FLG, Sum(B.NUM_DAY) as NUM_DAY from ( select a.INDV_SYS_ID, ( case when a.END_DT > '" + year + "-12-31' then Cast('" + year + "-12-31' as Date) else a.END_DT end - case when a.EFF_DT < '" + year + "-01-01' then Cast('" + year + "-01-01' as Date) else a.EFF_DT end) + 1 as NUM_DAY, a.PHRM_BEN_FLG from CLODM001.MEMBER_DETAIL_INPUT a where a.EFF_DT <= '" + year + "-12-31' and a.END_DT >= '" + year + "-01-01') as B group by B.INDV_SYS_ID ) C where C.MIN_PHARMACY_FLG = 'Y' and C.NUM_DAY >= 210 ) as MT on ED1.INDV_SYS_ID = MT.INDV_SYS_ID left join ( select ED2.INDV_SYS_ID, ED2.EPSD_NBR, Sum(ED2.QLTY_INCNT_RDUC_AMT) as RX_ALLW_AMT from CLODM001.ETG_DETAIL ED2 inner join CLODM001.DATE_FST_SRVC DFS2 on ED2.FST_SRVC_DT_SYS_ID = DFS2.FST_SRVC_DT_SYS_ID inner join CLODM001.HP_SERVICE_TYPE_CODE HSTC2 on ED2.HLTH_PLN_SRVC_TYP_CD_SYS_ID = HSTC2.HLTH_PLN_SRVC_TYP_CD_SYS_ID where DFS2.FST_SRVC_DT Between '" + year + "-01-01'and '" + year + "-12-31'  and ED2.QLTY_INCNT_RDUC_AMT > 0 and HSTC2.HLTH_PLN_SRVC_TYP_LVL_1_NM = 'PHARMACY' group by ED2.INDV_SYS_ID, ED2.EPSD_NBR ) Query1 on ED1.INDV_SYS_ID = Query1.INDV_SYS_ID and ED1.EPSD_NBR = Query1.EPSD_NBR where ED1.EPSD_NBR not in (0, -1) and DFS1.FST_SRVC_DT Between '" + year + "-01-01' and '" + year + "-12-31' and ED1.QLTY_INCNT_RDUC_AMT > 0 group by ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND, Query1.RX_ALLW_AMT ) as ETG_D group by ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD";

            var nrxx = await db_td.LoadData<NRX_Cost_UGAPModel>(connectionString: ConnectionStringTD, strSQL);

            string[] columns = typeof(NRX_Cost_UGAPModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<NRX_Cost_UGAPModel>(connectionString: ConnectionStringVC, "etg.NRX_Cost_UGAP_SOURCE", nrxx, columns, truncate: true);

            //STEP 2 etg.ETG_Episodes_UGAP_SOURCE
            //BROKEN APART DUE TO 200+ MILLION ROWS
            List<string> lst_lob = new List<string>();
            lst_lob.Add("COMMERCIAL");
            lst_lob.Add("MEDICARE");
            lst_lob.Add("MEDICAID");

            List<string> lst_yr = new List<string>(); //July 2021 - June 2023
            lst_yr.Add("2021");
            lst_yr.Add("2022");


            List<string> lst_qrt = new List<string>();
            lst_qrt.Add("01-01~03-31");
            lst_qrt.Add("04-01~06-30");
            lst_qrt.Add("07-01~09-30");
            lst_qrt.Add("10-01~12-31");

            int lob_id;

            bool blTruncate = true;


            foreach (var l in lst_lob)
            {
                lob_id = (l == "COMMERCIAL" ? 1 : (l == "MEDICARE" ? 2 : 3));


                Console.WriteLine("LOB:" + lob_id + " - " + l);

                foreach (var y in lst_yr)
                {


                    foreach (var q in lst_qrt)
                    {
                        var startdate = y + "-" + q.Split('~')[0];
                        var enddate = y + "-" + q.Split('~')[1];

                        Console.WriteLine("ETG Start Date: " + startdate);
                        Console.WriteLine("ETG End Date: " + enddate);


                        strSQL = "select es.EPSD_NBR, es.TOT_ALLW_AMT, en.SVRTY, en.ETG_BAS_CLSS_NBR, en.ETG_TX_IND, up.PROV_MPIN, es.TOT_NP_ALLW_AMT, " + lob_id + " as LOB_ID from CLODM001.ETG_SUMMARY es inner join CLODM001.ETG_NUMBER en on es.ETG_SYS_ID = en.ETG_SYS_ID inner join CLODM001.UNIQUE_PROVIDER up on es.RESP_UNIQ_PROV_SYS_ID = up.UNIQ_PROV_SYS_ID inner join CLODM001.INDIVIDUAL ind on es.INDV_SYS_ID = ind.INDV_SYS_ID inner join CLODM001.CLNOPS_CUSTOMER_SEGMENT ccs on ind.CLNOPS_CUST_SEG_SYS_ID = ccs.CLNOPS_CUST_SEG_SYS_ID inner join CLODM001.PRODUCT prod on ccs.PRDCT_SYS_ID = prod.PRDCT_SYS_ID inner join CLODM001.DATE_ETG_START DES on es.ETG_STRT_DT_SYS_ID = DES.ETG_STRT_DT_SYS_ID where es.EP_TYP_NBR in (0, 1, 2, 3) and es.TOT_ALLW_AMT >= 35 and COALESCE(en.SVRTY,'') <> '' and prod.PRDCT_LVL_1_NM = '" + l + "' and DES.ETG_STRT_DT >= '" + startdate + "' and DES.ETG_STRT_DT <= '" + enddate + "'";

                        Console.WriteLine("UGAP Pull Start Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

                        var cnt = await db_td.ExecuteScalar(connectionString: ConnectionStringTD, "SELECT COUNT(*) FROM (" + strSQL + ") tmp;");

                        Console.WriteLine("Count: " + string.Format("{0:#,0}", cnt));

                        var ugap = await db_td.LoadData<ETG_Episodes_UGAP>(connectionString: ConnectionStringTD, strSQL);

                        columns = typeof(ETG_Episodes_UGAP).GetProperties().Select(p => p.Name).ToArray();
                        await db_sql.BulkSave<ETG_Episodes_UGAP>(connectionString: ConnectionStringVC, "etg.ETG_Episodes_UGAP_SOURCE", ugap, columns, truncate: blTruncate);
                        Console.WriteLine("UGAP Pull End Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

                        blTruncate = false;
                        ugap = null;
                    }

                }

            }

            //STEP 3 etg.PrimarySpecWithCode_PDNDB_SOURCE
            //1 NDB  NDB NDB NDB NDB  NDB NDB NDB NDB NDB NDB NDB NDB
            strSQL = "Select prov.MPIN, prov.ProvType, prov.PrimSpec NDB_SPCL_CD, spcl.SpecTypeCd, spcl.PrimaryInd, spcltyp.ShortDesc From dbo.PROVIDER As prov Left Join dbo.PROV_SPECIALTIES spcl On prov.MPIN = spcl.MPIN And spcl.PractInSpecInd = 'Y' Left Join dbo.SPECIALTY_TYPES spcltyp On spcl.SpecTypeCd = spcltyp.SpecTypeCd;";
            var ndb = await db_sql.LoadData<PrimarySpecUHNModel>(connectionString: ConnectionStringUHN, strSQL);
            //2 PD
            //strSQL = "select A.PREM_SPCL_CD, A.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP A where A.PREM_DESG_VER_NBR = 15;"; //SELECT MAX(PREM_DESG_VER_NBR) FROM 
            strSQL = "select A.PREM_SPCL_CD, A.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP A where A.PREM_DESG_VER_NBR = (SELECT MAX(PREM_DESG_VER_NBR) FROM PD.CNFG_PREM_SPCL_MAP)";
            var pd = await db_sql.LoadData<PremiumSpecPDModel>(connectionString: ConnectionStringPD, strSQL);
            //3 JOIN NDB + PD INTO etg.PrimarySpecWithCode_PDNDB_SOURCE
            var pd_ndb = from n in ndb
                         join p in pd on n.NDB_SPCL_CD equals p.NDB_SPCL_TYP_CD into n_p_join
                         from np in n_p_join.DefaultIfEmpty()
                         select new PrimarySpecWithCodeModel
                         {
                             MPIN = n.MPIN,
                             ProvType = n.ProvType,
                             NDB_SPCL_CD = n.NDB_SPCL_CD,
                             SpecTypeCd = n.SpecTypeCd,
                             PrimaryInd = n.PrimaryInd,
                             ShortDesc = n.ShortDesc,
                             PREM_SPCL_CD = ((n.NDB_SPCL_CD == "033" || n.NDB_SPCL_CD == "101" || n.NDB_SPCL_CD == "500") ? "CARDVS" : ((n.NDB_SPCL_CD == "007") ? "DERMA" : ((n.NDB_SPCL_CD == "038") ? "GERIA" : ((n.NDB_SPCL_CD == "093" || n.NDB_SPCL_CD == "504" || n.NDB_SPCL_CD == "059") ? "HEMAONC" : ((n.NDB_SPCL_CD == "479" || n.NDB_SPCL_CD == "095") ? "VASC" : ((n.NDB_SPCL_CD == "024" || n.NDB_SPCL_CD == "359" || n.NDB_SPCL_CD == "337" || n.NDB_SPCL_CD == "233") ? "PLASTIC" : (np == null ? null : np.PREM_SPCL_CD))))))),
                             Secondary_Spec = (n.SpecTypeCd == "304" ? "CARDEP" : null)
                         };

            columns = typeof(PrimarySpecWithCodeModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<PrimarySpecWithCodeModel>(connectionString: ConnectionStringVC, "etg.PrimarySpecWithCode_PDNDB_SOURCE", pd_ndb, columns, truncate: true);


            //UNUSED DELETE???
            //strSQL = "SELECT prim.MPIN, CASE WHEN prim.[PREM_SPCL_CD] ='CARDCD' AND sec.[secondary_spec] = 'CARDEP' THEN 'CARDEP' ELSE CASE WHEN prim.[PREM_SPCL_CD] in ('NS', 'ORTHO') THEN 'NOS' ELSE [PREM_SPCL_CD] END END as [PREM_SPCL_CD] FROM (SELECT [PREM_SPCL_CD], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [PREM_SPCL_CD], [MPIN] ) prim LEFT JOIN (SELECT [Secondary_Spec], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [Secondary_Spec], [MPIN]) sec ON prim.MPIN = sec.MPIN";
            //VC DB 


            //STEP 4 etg.ETG_Cancer_Flag_PD_SOURCE
            strSQL = "select a.ETG_BASE_CLASS, a.CNCR_IND from PD.CNFG_CNCR_REL_ETG a inner join ( select Max(PD.CNFG_CNCR_REL_ETG.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_CNCR_REL_ETG ) b on a.PREM_DESG_VER_NBR = b.Max_PREM_DESG_VER_NBR";
            var can = await db_sql.LoadData<ETG_Cancer_Flag_PDModel>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(ETG_Cancer_Flag_PDModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<ETG_Cancer_Flag_PDModel>(connectionString: ConnectionStringVC, "etg.ETG_Cancer_Flag_PD_SOURCE", can, columns, truncate: true);

            //STEP 5 etg.PremiumNDBSpec_PD_SOURCE
            strSQL = "select n.NDB_SPCL_TYP_CD, n.SPCL_TYP_CD_DESC, c.PREM_SPCL_CD from pd.CLCT_SPCL_TYP_CD n left join ( select b.PREM_SPCL_CD, b.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP b inner join ( select Max(PD.CNFG_PREM_SPCL_MAP.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_PREM_SPCL_MAP ) a on b.PREM_DESG_VER_NBR = a.Max_PREM_DESG_VER_NBR ) c on n.NDB_SPCL_TYP_CD = c.NDB_SPCL_TYP_CD where n.NDB_SPCL_TYP_CD <> ' '";
            var pndb = await db_sql.LoadData<PremiumNDBSpecPDModel>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(PremiumNDBSpecPDModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<PremiumNDBSpecPDModel>(connectionString: ConnectionStringVC, "etg.PremiumNDBSpec_PD_SOURCE", pndb, columns, truncate: true);

            //STEP 6 etg.ETG_Mapped_PD_SOURCE
            strSQL = "select LTRIM(RTRIM(a.PREM_SPCL_CD)) as PREM_SPCL_CD, a.TRT_CD, a.ETG_BASE_CLASS from pd.CNFG_ETG_SPCL a inner join ( select Max(PD.CNFG_ETG_SPCL.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_ETG_SPCL ) Query1 on a.PREM_DESG_VER_NBR = Query1.Max_PREM_DESG_VER_NBR";
            var map = await db_sql.LoadData<ETG_Mapped_PD>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(ETG_Mapped_PD).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<ETG_Mapped_PD>(connectionString: ConnectionStringVC, "etg.ETG_Mapped_PD_SOURCE", map, columns, truncate: true);


            //STEP 7 [etg].[ETG_Dataload_NRX_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_NRX_AGG]; INSERT INTO [etg].[ETG_Dataload_NRX_AGG] ([ETG_Base_Class] ,MEMBER_COUNT,EPSD_COUNT,ETGD_TOT_ALLW_AMT,ETGD_RX_ALLW_AMT,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX]) SELECT [ETG_Base_Class] ,MEMBER_COUNT,EPSD_COUNT,ETGD_TOT_ALLW_AMT,ETGD_RX_ALLW_AMT,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX] FROM [etg].[VW_ETG_Dataload_NRX_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);


            //STEP 8 [etg].[ETG_Dataload_EC_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_EC_AGG];INSERT INTO [etg].[ETG_Dataload_EC_AGG] ([Premium_Specialty] ,[ETG_Base_Class] ,[EC_Treatment_Indicator] ,[EC_Episode_Count] ,[EC_Total_Cost] ,[EC_Average_Cost] ,[EC_Coefficients_of_Variation] ,[EC_Normalized_Pricing_Episode_Count] ,[EC_Normalized_Pricing_Total_Cost] ,[EC_Spec_Episode_Count] ,[EC_Spec_Total_Cost] ,[EC_Spec_Average_Cost] ,[EC_Spec_Coefficients_of_Variation] ,[EC_Spec_Percent_of_Episodes] ,[EC_Spec_Normalized_Pricing_Episode_Count] ,[EC_Spec_Normalized_Pricing_Total_Cost] ,[EC_CV3] ,[EC_Spec_Episode_Volume] ,[PD_Mapped]) SELECT [Premium_Specialty] ,[ETG_Base_Class] ,[EC_Treatment_Indicator] ,[EC_Episode_Count] ,[EC_Total_Cost] ,[EC_Average_Cost] ,[EC_Coefficients_of_Variation] ,[EC_Normalized_Pricing_Episode_Count] ,[EC_Normalized_Pricing_Total_Cost] ,[EC_Spec_Episode_Count] ,[EC_Spec_Total_Cost] ,[EC_Spec_Average_Cost] ,[EC_Spec_Coefficients_of_Variation] ,[EC_Spec_Percent_of_Episodes] ,[EC_Spec_Normalized_Pricing_Episode_Count] ,[EC_Spec_Normalized_Pricing_Total_Cost] ,[EC_CV3] ,[EC_Spec_Episode_Volume] ,[PD_Mapped] FROM [etg].[VW_ETG_Dataload_EC_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);


            //STEP 9 [etg].[ETG_Dataload_PC_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_PC_AGG];INSERT INTO [etg].[ETG_Dataload_PC_AGG] ([Premium_Specialty] ,[ETG_Base_Class] ,[PC_Episode_Count] ,[PC_Total_Cost] ,[PC_Average_Cost] ,[PC_Coefficients_of_Variation] ,[PC_Normalized_Pricing_Episode_Count] ,[PC_Normalized_Pricing_Total_Cost] ,[PC_Spec_Episode_Count] ,[PC_Spec_Total_Cost] ,[PC_Spec_Average_Cost] ,[PC_Spec_CV] ,[PC_Spec_Percent_of_Episodes] ,[PC_Spec_Normalized_Pricing_Episode_Count] ,[PC_Spec_Normalized_Pricing_Total_Cost] ,[PC_CV3] ,[PC_Spec_Epsd_Volume]) SELECT [Premium_Specialty] ,[ETG_Base_Class] ,[PC_Episode_Count] ,[PC_Total_Cost] ,[PC_Average_Cost] ,[PC_Coefficients_of_Variation] ,[PC_Normalized_Pricing_Episode_Count] ,[PC_Normalized_Pricing_Total_Cost] ,[PC_Spec_Episode_Count] ,[PC_Spec_Total_Cost] ,[PC_Spec_Average_Cost] ,[PC_Spec_CV] ,[PC_Spec_Percent_of_Episodes] ,[PC_Spec_Normalized_Pricing_Episode_Count] ,[PC_Spec_Normalized_Pricing_Total_Cost] ,[PC_CV3] ,[PC_Spec_Epsd_Volume] FROM [etg].[VW_ETG_Dataload_PC_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);





        }


        public async Task getETGSymmSourceDataOriginalAsync(Int16 year = 2022)
        {
            //ETG DATA LOAD
            //ETG DATA LOAD
            //ETG DATA LOAD

            IRelationalDataAccess db_sql = new SqlDataAccess();
            IRelationalDataAccess db_td = new TeraDataAccess();

            //STEP 1 etg.NRX_Cost_UGAP_SOURCE
            string strSQL = "select ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD, Count(Distinct ETG_D.INDV_SYS_ID) as MEMBER_COUNT, Count(Distinct ETG_D.EPSD_NBR) as EPSD_COUNT, Sum(ETG_D.TOT_ALLW_AMT) as ETGD_TOT_ALLW_AMT, Sum(ETG_D.RX_ALLW_AMT) as ETGD_RX_ALLW_AMT, case when Sum(ETG_D.TOT_ALLW_AMT) = 0 then 0 else NVL(Sum(ETG_D.RX_ALLW_AMT), 0) / Sum(ETG_D.TOT_ALLW_AMT) end as RX_RATE from ( select ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND as TRT_CD, Sum(ED1.QLTY_INCNT_RDUC_AMT) as TOT_ALLW_AMT, Query1.RX_ALLW_AMT from CLODM001.ETG_DETAIL ED1 inner join CLODM001.ETG_NUMBER EN1 on ED1.ETG_SYS_ID = EN1.ETG_SYS_ID inner join CLODM001.DATE_FST_SRVC DFS1 on ED1.FST_SRVC_DT_SYS_ID = DFS1.FST_SRVC_DT_SYS_ID inner join ( select C.INDV_SYS_ID from ( select B.INDV_SYS_ID, Min(B.PHRM_BEN_FLG) as MIN_PHARMACY_FLG, Sum(B.NUM_DAY) as NUM_DAY from ( select a.INDV_SYS_ID, ( case when a.END_DT > '" + year + "-12-31' then Cast('" + year + "-12-31' as Date) else a.END_DT end - case when a.EFF_DT < '" + year + "-01-01' then Cast('" + year + "-01-01' as Date) else a.EFF_DT end) + 1 as NUM_DAY, a.PHRM_BEN_FLG from CLODM001.MEMBER_DETAIL_INPUT a where a.EFF_DT <= '" + year + "-12-31' and a.END_DT >= '" + year + "-01-01') as B group by B.INDV_SYS_ID ) C where C.MIN_PHARMACY_FLG = 'Y' and C.NUM_DAY >= 210 ) as MT on ED1.INDV_SYS_ID = MT.INDV_SYS_ID left join ( select ED2.INDV_SYS_ID, ED2.EPSD_NBR, Sum(ED2.QLTY_INCNT_RDUC_AMT) as RX_ALLW_AMT from CLODM001.ETG_DETAIL ED2 inner join CLODM001.DATE_FST_SRVC DFS2 on ED2.FST_SRVC_DT_SYS_ID = DFS2.FST_SRVC_DT_SYS_ID inner join CLODM001.HP_SERVICE_TYPE_CODE HSTC2 on ED2.HLTH_PLN_SRVC_TYP_CD_SYS_ID = HSTC2.HLTH_PLN_SRVC_TYP_CD_SYS_ID where DFS2.FST_SRVC_DT Between '" + year + "-01-01'and '" + year + "-12-31'  and ED2.QLTY_INCNT_RDUC_AMT > 0 and HSTC2.HLTH_PLN_SRVC_TYP_LVL_1_NM = 'PHARMACY' group by ED2.INDV_SYS_ID, ED2.EPSD_NBR ) Query1 on ED1.INDV_SYS_ID = Query1.INDV_SYS_ID and ED1.EPSD_NBR = Query1.EPSD_NBR where ED1.EPSD_NBR not in (0, -1) and DFS1.FST_SRVC_DT Between '" + year + "-01-01' and '" + year + "-12-31' and ED1.QLTY_INCNT_RDUC_AMT > 0 group by ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND, Query1.RX_ALLW_AMT ) as ETG_D group by ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD";

            var nrxx = await db_td.LoadData<NRX_Cost_UGAPModel>(connectionString: ConnectionStringTD, strSQL);

            string[] columns = typeof(NRX_Cost_UGAPModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<NRX_Cost_UGAPModel>(connectionString: ConnectionStringVC, "etg.NRX_Cost_UGAP_SOURCE_PREVIOUS", nrxx, columns, truncate: true);

            //STEP 2 etg.ETG_Episodes_UGAP_SOURCE
            //BROKEN APART DUE TO 200+ MILLION ROWS
            List<string> lst_lob = new List<string>();
            lst_lob.Add("COMMERCIAL");
            lst_lob.Add("MEDICARE");
            lst_lob.Add("MEDICAID");

            List<string> lst_yr = new List<string>();
            lst_yr.Add("2021");
            lst_yr.Add("2022");


            List<string> lst_qrt = new List<string>();
            lst_qrt.Add("01-01~03-31");
            lst_qrt.Add("04-01~06-30");
            lst_qrt.Add("07-01~09-30");
            lst_qrt.Add("10-01~12-31");

            int lob_id;

            bool blTruncate = true;


            foreach (var l in lst_lob)
            {
                lob_id = (l == "COMMERCIAL" ? 1 : (l == "MEDICARE" ? 2 : 3));


                Console.WriteLine("LOB:" + lob_id + " - " + l);

                foreach (var y in lst_yr)
                {


                    foreach (var q in lst_qrt)
                    {
                        var startdate = y + "-" + q.Split('~')[0];
                        var enddate = y + "-" + q.Split('~')[1];

                        Console.WriteLine("ETG Start Date: " + startdate);
                        Console.WriteLine("ETG End Date: " + enddate);


                        strSQL = "select es.EPSD_NBR, es.TOT_ALLW_AMT, en.SVRTY, en.ETG_BAS_CLSS_NBR, en.ETG_TX_IND, up.PROV_MPIN, es.TOT_NP_ALLW_AMT, " + lob_id + " as LOB_ID from CLODM001.ETG_SUMMARY es inner join CLODM001.ETG_NUMBER en on es.ETG_SYS_ID = en.ETG_SYS_ID inner join CLODM001.UNIQUE_PROVIDER up on es.RESP_UNIQ_PROV_SYS_ID = up.UNIQ_PROV_SYS_ID inner join CLODM001.INDIVIDUAL ind on es.INDV_SYS_ID = ind.INDV_SYS_ID inner join CLODM001.CLNOPS_CUSTOMER_SEGMENT ccs on ind.CLNOPS_CUST_SEG_SYS_ID = ccs.CLNOPS_CUST_SEG_SYS_ID inner join CLODM001.PRODUCT prod on ccs.PRDCT_SYS_ID = prod.PRDCT_SYS_ID inner join CLODM001.DATE_ETG_START DES on es.ETG_STRT_DT_SYS_ID = DES.ETG_STRT_DT_SYS_ID where es.EP_TYP_NBR in (0, 1, 2, 3) and es.TOT_ALLW_AMT >= 35 and ISNULL(en.SVRTY,'') <> '' and prod.PRDCT_LVL_1_NM = '" + l + "' and DES.ETG_STRT_DT >= '" + startdate + "' and DES.ETG_STRT_DT <= '" + enddate + "'";

                        Console.WriteLine("UGAP Pull Start Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

                        var cnt = await db_td.ExecuteScalar(connectionString: ConnectionStringTD, "SELECT COUNT(*) FROM (" + strSQL + ") tmp;");

                        Console.WriteLine("Count: " + string.Format("{0:#,0}", cnt));

                        var ugap = await db_td.LoadData<ETG_Episodes_UGAP>(connectionString: ConnectionStringTD, strSQL);

                        columns = typeof(ETG_Episodes_UGAP).GetProperties().Select(p => p.Name).ToArray();
                        await db_sql.BulkSave<ETG_Episodes_UGAP>(connectionString: ConnectionStringVC, "etg.ETG_Episodes_UGAP_SOURCE", ugap, columns, truncate: blTruncate);
                        Console.WriteLine("UGAP Pull End Time: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

                        blTruncate = false;
                        ugap = null;
                    }

                }

            }


            //STEP 3 etg.PrimarySpecWithCode_PDNDB_SOURCE
            //1 NDB
            strSQL = "Select prov.MPIN, prov.ProvType, prov.PrimSpec NDB_SPCL_CD, spcl.SpecTypeCd, spcl.PrimaryInd, spcltyp.ShortDesc From dbo.PROVIDER As prov Left Join dbo.PROV_SPECIALTIES spcl On prov.MPIN = spcl.MPIN And spcl.PractInSpecInd = 'Y' Left Join dbo.SPECIALTY_TYPES spcltyp On spcl.SpecTypeCd = spcltyp.SpecTypeCd;";
            var ndb = await db_sql.LoadData<PrimarySpecUHNModel>(connectionString: ConnectionStringUHN, strSQL);
            //2 PD
            strSQL = "select A.PREM_SPCL_CD, A.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP A where A.PREM_DESG_VER_NBR = 15;"; //SELECT MAX(PREM_DESG_VER_NBR) FROM 
            var pd = await db_sql.LoadData<PremiumSpecPDModel>(connectionString: ConnectionStringPD, strSQL);
            //3 JOIN NDB + PD INTO etg.PrimarySpecWithCode_PDNDB_SOURCE
            var pd_ndb = from n in ndb
                         join p in pd on n.NDB_SPCL_CD equals p.NDB_SPCL_TYP_CD into n_p_join
                         from np in n_p_join.DefaultIfEmpty()
                         select new PrimarySpecWithCodeModel
                         {
                             MPIN = n.MPIN,
                             ProvType = n.ProvType,
                             NDB_SPCL_CD = n.NDB_SPCL_CD,
                             SpecTypeCd = n.SpecTypeCd,
                             PrimaryInd = n.PrimaryInd,
                             ShortDesc = n.ShortDesc,
                             PREM_SPCL_CD = ((n.NDB_SPCL_CD == "033" || n.NDB_SPCL_CD == "101" || n.NDB_SPCL_CD == "500") ? "CARDVS" : ((n.NDB_SPCL_CD == "007") ? "DERMA" : ((n.NDB_SPCL_CD == "038") ? "GERIA" : ((n.NDB_SPCL_CD == "093" || n.NDB_SPCL_CD == "504" || n.NDB_SPCL_CD == "059") ? "HEMAONC" : ((n.NDB_SPCL_CD == "479" || n.NDB_SPCL_CD == "095") ? "VASC" : ((n.NDB_SPCL_CD == "024" || n.NDB_SPCL_CD == "359" || n.NDB_SPCL_CD == "337" || n.NDB_SPCL_CD == "233") ? "PLASTIC" : (np == null ? null : np.PREM_SPCL_CD))))))),
                             Secondary_Spec = (n.SpecTypeCd == "304" ? "CARDEP" : null)
                         };

            columns = typeof(PrimarySpecWithCodeModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<PrimarySpecWithCodeModel>(connectionString: ConnectionStringVC, "etg.PrimarySpecWithCode_PDNDB_SOURCE", pd_ndb, columns, truncate: true);


            //UNUSED DELETE???
            //strSQL = "SELECT prim.MPIN, CASE WHEN prim.[PREM_SPCL_CD] ='CARDCD' AND sec.[secondary_spec] = 'CARDEP' THEN 'CARDEP' ELSE CASE WHEN prim.[PREM_SPCL_CD] in ('NS', 'ORTHO') THEN 'NOS' ELSE [PREM_SPCL_CD] END END as [PREM_SPCL_CD] FROM (SELECT [PREM_SPCL_CD], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [PREM_SPCL_CD], [MPIN] ) prim LEFT JOIN (SELECT [Secondary_Spec], [MPIN] FROM [vct].[PrimarySpecWithCode] GROUP BY [Secondary_Spec], [MPIN]) sec ON prim.MPIN = sec.MPIN";
            //VC DB 


            //STEP 4 etg.ETG_Cancer_Flag_PD_SOURCE
            strSQL = "select a.ETG_BASE_CLASS, a.CNCR_IND from PD.CNFG_CNCR_REL_ETG a inner join ( select Max(PD.CNFG_CNCR_REL_ETG.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_CNCR_REL_ETG ) b on a.PREM_DESG_VER_NBR = b.Max_PREM_DESG_VER_NBR";
            var can = await db_sql.LoadData<ETG_Cancer_Flag_PDModel>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(ETG_Cancer_Flag_PDModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<ETG_Cancer_Flag_PDModel>(connectionString: ConnectionStringVC, "etg.ETG_Cancer_Flag_PD_SOURCE", can, columns, truncate: true);

            //STEP 5 etg.PremiumNDBSpec_PD_SOURCE
            strSQL = "select n.NDB_SPCL_TYP_CD, n.SPCL_TYP_CD_DESC, c.PREM_SPCL_CD from pd.CLCT_SPCL_TYP_CD n left join ( select b.PREM_SPCL_CD, b.NDB_SPCL_TYP_CD from PD.CNFG_PREM_SPCL_MAP b inner join ( select Max(PD.CNFG_PREM_SPCL_MAP.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_PREM_SPCL_MAP ) a on b.PREM_DESG_VER_NBR = a.Max_PREM_DESG_VER_NBR ) c on n.NDB_SPCL_TYP_CD = c.NDB_SPCL_TYP_CD where n.NDB_SPCL_TYP_CD <> ' '";
            var pndb = await db_sql.LoadData<PremiumNDBSpecPDModel>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(PremiumNDBSpecPDModel).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<PremiumNDBSpecPDModel>(connectionString: ConnectionStringVC, "etg.PremiumNDBSpec_PD_SOURCE", pndb, columns, truncate: true);

            //STEP 6 etg.ETG_Mapped_PD_SOURCE
            strSQL = "select LTRIM(RTRIM(a.PREM_SPCL_CD)) as PREM_SPCL_CD, a.TRT_CD, a.ETG_BASE_CLASS from pd.CNFG_ETG_SPCL a inner join ( select Max(PD.CNFG_ETG_SPCL.PREM_DESG_VER_NBR) as Max_PREM_DESG_VER_NBR from PD.CNFG_ETG_SPCL ) Query1 on a.PREM_DESG_VER_NBR = Query1.Max_PREM_DESG_VER_NBR";
            var map = await db_sql.LoadData<ETG_Mapped_PD>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(ETG_Mapped_PD).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<ETG_Mapped_PD>(connectionString: ConnectionStringVC, "etg.ETG_Mapped_PD_SOURCE", map, columns, truncate: true);


            //STEP 7 [etg].[ETG_Dataload_NRX_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_NRX_AGG];INSERT INTO [etg].[ETG_Dataload_NRX_AGG] ([ETG_Base_Class], [CNCR_IND], MEMBER_COUNT,EPSD_COUNT,ETGD_TOT_ALLW_AMT,ETGD_RX_ALLW_AMT ,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX]) SELECT [ETG_Base_Class], [CNCR_IND], MEMBER_COUNT,EPSD_COUNT,ETGD_TOT_ALLW_AMT,ETGD_RX_ALLW_AMT  ,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX] FROM [etg].[VW_ETG_Dataload_NRX_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);


            //STEP 8 [etg].[ETG_Dataload_EC_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_EC_AGG];INSERT INTO [etg].[ETG_Dataload_EC_AGG] ([Premium_Specialty] ,[ETG_Base_Class] ,[EC_Treatment_Indicator] ,[EC_Episode_Count] ,[EC_Total_Cost] ,[EC_Average_Cost] ,[EC_Coefficients_of_Variation] ,[EC_Normalized_Pricing_Episode_Count] ,[EC_Normalized_Pricing_Total_Cost] ,[EC_Spec_Episode_Count] ,[EC_Spec_Total_Cost] ,[EC_Spec_Average_Cost] ,[EC_Spec_Coefficients_of_Variation] ,[EC_Spec_Percent_of_Episodes] ,[EC_Spec_Normalized_Pricing_Episode_Count] ,[EC_Spec_Normalized_Pricing_Total_Cost] ,[EC_CV3] ,[EC_Spec_Episode_Volume] ,[PD_Mapped]) SELECT [Premium_Specialty] ,[ETG_Base_Class] ,[EC_Treatment_Indicator] ,[EC_Episode_Count] ,[EC_Total_Cost] ,[EC_Average_Cost] ,[EC_Coefficients_of_Variation] ,[EC_Normalized_Pricing_Episode_Count] ,[EC_Normalized_Pricing_Total_Cost] ,[EC_Spec_Episode_Count] ,[EC_Spec_Total_Cost] ,[EC_Spec_Average_Cost] ,[EC_Spec_Coefficients_of_Variation] ,[EC_Spec_Percent_of_Episodes] ,[EC_Spec_Normalized_Pricing_Episode_Count] ,[EC_Spec_Normalized_Pricing_Total_Cost] ,[EC_CV3] ,[EC_Spec_Episode_Volume] ,[PD_Mapped] FROM [etg].[VW_ETG_Dataload_EC_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);


            //STEP 9 [etg].[ETG_Dataload_PC_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_PC_AGG];INSERT INTO [etg].[ETG_Dataload_PC_AGG] ([Premium_Specialty] ,[ETG_Base_Class] ,[PC_Episode_Count] ,[PC_Total_Cost] ,[PC_Average_Cost] ,[PC_Coefficients_of_Variation] ,[PC_Normalized_Pricing_Episode_Count] ,[PC_Normalized_Pricing_Total_Cost] ,[PC_Spec_Episode_Count] ,[PC_Spec_Total_Cost] ,[PC_Spec_Average_Cost] ,[PC_Spec_CV] ,[PC_Spec_Percent_of_Episodes] ,[PC_Spec_Normalized_Pricing_Episode_Count] ,[PC_Spec_Normalized_Pricing_Total_Cost] ,[PC_CV3] ,[PC_Spec_Epsd_Volume]) SELECT [Premium_Specialty] ,[ETG_Base_Class] ,[PC_Episode_Count] ,[PC_Total_Cost] ,[PC_Average_Cost] ,[PC_Coefficients_of_Variation] ,[PC_Normalized_Pricing_Episode_Count] ,[PC_Normalized_Pricing_Total_Cost] ,[PC_Spec_Episode_Count] ,[PC_Spec_Total_Cost] ,[PC_Spec_Average_Cost] ,[PC_Spec_CV] ,[PC_Spec_Percent_of_Episodes] ,[PC_Spec_Normalized_Pricing_Episode_Count] ,[PC_Spec_Normalized_Pricing_Total_Cost] ,[PC_CV3] ,[PC_Spec_Epsd_Volume] FROM [etg].[VW_ETG_Dataload_PC_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);





        }

        //EBM SOURCE AUTOMATION
        public async Task getEBMSourceDataAsync()
        {
            //EBM DATA LOAD
            //EBM DATA LOAD
            //EBM DATA LOAD

            IRelationalDataAccess db_sql = new SqlDataAccess();


            //1 ebm.DQC_DATA_UHPD_SOURCE
            string strSQL = "select cur.REPORT_CASE_ID, cur.REPORT_RULE_ID, cur.COND_NM, cur.RULE_DESC, cur.PREM_SPCL_CD, cur.CNFG_POP_SYS_ID, case when cur.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when cur.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when cur.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else 'UNKNOWN' end as LOB, Replace(Str(cur.UNET_MKT_NBR, 7), Space(1), '0') as MKT_NBR, cur.UNET_MKT_NBR, cur.MKT_DESC as UNET_MKT_DESC, cur.Cur_Version as Current_Version, cur.Cur_CMPLNT_CNT as Current_Market_Compliant, cur.Cur_OPRTNTY_CNT as Current_Market_Opportunity, cur.Cur_NAT_CMPLNC_CNT as Current_National_Compliant, cur.Cur_NAT_OPRTNTY_CNT as Current_National_Opportunity, prev.Prev_Version as Previous_Version, prev.Prev_CMPLNT_CNT as Previous_Market_Compliant, prev.Prev_OPRTNTY_CNT as Previous_Market_Opportunity, prev.Prev_NAT_CMPLNC_CNT as Previous_National_Compliant, prev.Prev_NAT_OPRTNTY_CNT as Previous_National_Opportunity, Concat(@@servername, ' - ', Db_Name()) as DTLocation, Cast(GetDate() as Date) as data_Extract_Dt from ( select a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Sum(a.CMPLNT_CNT) as Cur_CMPLNT_CNT, Sum(a.OPRTNTY_CNT) as Cur_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Cur_Version, b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT as Cur_NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT as Cur_NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC from PD_Reporting.DQC.DQC_342_EBM_QLTY_MPIN_MSR_SUMMARY a inner join PD_Reporting.DQC.DQC_342_EBM_RULE_DESCRIPTION b on a.REPORT_CASE_ID = b.REPORT_CASE_ID and a.REPORT_RULE_ID = b.REPORT_RULE_ID and a.Iteration = b.Iteration and a.PD_Version = b.PD_Version and a.Run = b.Run inner join PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR c on b.REPORT_CASE_ID = c.REPORT_CASE_ID and b.REPORT_RULE_ID = c.REPORT_RULE_ID and a.CNFG_POP_SYS_ID = c.CNFG_POP_SYS_ID and a.PREM_SPCL_CD = c.PREM_SPCL_CD and b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on a.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join ( select b.* from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR from PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR ) a ) b where b.rank = 1 ) f on a.Iteration = f.Iteration and a.Run = f.Run and a.PREM_DESG_VER_NBR = f.PREM_DESG_VER_NBR group by a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC ) cur left join ( select a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Sum(a.CMPLNT_CNT) as Prev_CMPLNT_CNT, Sum(a.OPRTNTY_CNT) as Prev_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Prev_Version, b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT as Prev_NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT as Prev_NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC from PD_Reporting.DQC.DQC_342_EBM_QLTY_MPIN_MSR_SUMMARY a inner join PD_Reporting.DQC.DQC_342_EBM_RULE_DESCRIPTION b on a.REPORT_CASE_ID = b.REPORT_CASE_ID and a.REPORT_RULE_ID = b.REPORT_RULE_ID and a.Iteration = b.Iteration and a.PD_Version = b.PD_Version and a.Run = b.Run inner join PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR c on b.REPORT_CASE_ID = c.REPORT_CASE_ID and b.REPORT_RULE_ID = c.REPORT_RULE_ID and a.CNFG_POP_SYS_ID = c.CNFG_POP_SYS_ID and a.PREM_SPCL_CD = c.PREM_SPCL_CD and b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on a.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join ( select b.* from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR from PD_Reporting.DQC.DQC_342_EBM_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR ) a ) b where b.rank = 2 ) f on a.Iteration = f.Iteration and a.Run = f.Run and a.PREM_DESG_VER_NBR = f.PREM_DESG_VER_NBR group by a.REPORT_CASE_ID, a.REPORT_RULE_ID, a.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), b.COND_NM, b.RULE_DESC, c.NAT_CMPLNC_CNT, c.NAT_OPRTNTY_CNT, a.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC ) prev on cur.REPORT_CASE_ID = prev.REPORT_CASE_ID and cur.REPORT_RULE_ID = prev.REPORT_RULE_ID and cur.PREM_SPCL_CD = prev.PREM_SPCL_CD and cur.UNET_MKT_NBR = prev.UNET_MKT_NBR and cur.CNFG_POP_SYS_ID = prev.CNFG_POP_SYS_ID";

            var ebm = await db_sql.LoadData<DQC_DATA_EBM_UHPD_SOURCE_Model>(connectionString: ConnectionStringUHPD, strSQL);

            string[] columns = typeof(DQC_DATA_EBM_UHPD_SOURCE_Model).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<DQC_DATA_EBM_UHPD_SOURCE_Model>(connectionString: ConnectionStringVC, "ebm.DQC_DATA_UHPD_SOURCE", ebm, columns, truncate: true);



            //EBM DATA LOAD
            //EBM DATA LOAD
            //EBM DATA LOAD

        }

        //PEG SOURCE AUTOMATION
        public async Task getPEGSourceDataAsync()
        {
            //PEG DATA LOAD
            //PEG DATA LOAD
            //PEG DATA LOAD

            IRelationalDataAccess db_sql = new SqlDataAccess();


            //3 peg.PEG_ANCH_UHPD_SOURCE
            string strSQL = "select b.PEG_ANCH_CATGY, b.PEG_ANCH_SBCATGY, b.PEG_ANCH_SBCATGY_DESC, a.PEG_ANCH_CATGY_ID, a.PEG_ANCH_CATGY_DESC, Concat(@@servername, ' - ', Db_Name()) as PACLocation from PD.CNFG_ANCH_SBCATGY b inner join PD.PEG_ANCHOR_CATEGORY a on b.PEG_ANCH_CATGY = a.PEG_ANCH_CATGY group by b.PEG_ANCH_CATGY, b.PEG_ANCH_SBCATGY, b.PEG_ANCH_SBCATGY_DESC, a.PEG_ANCH_CATGY_ID, a.PEG_ANCH_CATGY_DESC";
            var pa = await db_sql.LoadData<PEG_ANCH_Model>(connectionString: ConnectionStringPD, strSQL);
            string[] columns = typeof(PEG_ANCH_Model).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<PEG_ANCH_Model>(connectionString: ConnectionStringVC, "peg.PEG_ANCH_UHPD_SOURCE", pa, columns, truncate: true);


            //2 vct.Rate_Region
            strSQL = "select PD.RATE_REGION.MKT_NBR, PD.RATE_REGION.MKT_NM, PD.RATE_REGION.MAJ_MKT_NM, PD.RATE_REGION.RGN_NM, PD.RATE_REGION.MKT_RLLP_NM, Concat(@@servername, ' - ', Db_Name()) as RRLocation from PD.RATE_REGION";
            var rr = await db_sql.LoadData<Rate_Region_Model>(connectionString: ConnectionStringPD, strSQL);
            columns = typeof(Rate_Region_Model).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<Rate_Region_Model>(connectionString: ConnectionStringVC, "vct.Rate_Region", rr, columns, truncate: true);


            //1 peg.DQC_DATA_UHPD_SOURCE
            strSQL = "select cur.PEG_ANCH_CATGY, cur.PEG_ANCH_SBCATGY, cur.PREM_SPCL_CD, cur.SVRTY_LVL_CD, cur.APR_DRG_RLLP_NBR, cur.QLTY_MSR_NM, cur.CNFG_POP_SYS_ID, case when cur.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when cur.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when cur.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else 'UNKNOWN' end as LOB, Replace(Str(cur.UNET_MKT_NBR, 7), Space(1), '0') as MKT_NBR, cur.UNET_MKT_NBR, cur.MKT_DESC as UNET_MKT_DESC, cur.Cur_Version as Current_Version, cur.Cur_CMPLNT_CNT as Current_Market_Compliant, cur.Cur_OPRTNTY_CNT as Current_Market_Opportunity, cur.Cur_NAT_CMPLNC_CNT as Current_National_Compliant, cur.Cur_NAT_OPRTNTY_CNT as Current_National_Opportunity, prev.Prev_Version as Previous_Version, prev.Prev_CMPLNT_CNT as Previous_Market_Compliant, prev.Prev_OPRTNTY_CNT as Previous_Market_Opportunity, prev.Prev_NAT_CMPLNC_CNT as Previous_National_Compliant, prev.Prev_NAT_OPRTNTY_CNT as Previous_National_Opportunity, Concat(@@servername, ' - ', Db_Name()) as DTLocation, Cast(GetDate() as Date) as data_Extract_Dt from ( select c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Sum(c.CMPLNT_CNT) as Cur_CMPLNT_CNT, Sum(c.OPRTNTY_CNT) as Cur_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Cur_Version, c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT as Cur_NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT as Cur_NAT_OPRTNTY_CNT from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank, a.PD_Version from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR, a.PD_Version from PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR, a.PD_Version ) a ) b inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_MPIN_MSR_SUMMARY c on b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on c.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR f on c.PEG_ANCH_SBCATGY = f.PEG_ANCH_SBCATGY and c.PEG_ANCH_CATGY = f.PEG_ANCH_CATGY and c.SVRTY_LVL_CD = f.SVRTY_LVL_CD and c.QLTY_MSR_NM = f.QLTY_MSR_NM and c.CNFG_POP_SYS_ID = f.CNFG_POP_SYS_ID and c.PREM_SPCL_CD = f.PREM_SPCL_CD and d.Iteration = f.Iteration and d.PD_Version = f.PD_Version and d.Run = f.Run and c.APR_DRG_RLLP_NBR = f.APR_DRG_RLLP_NBR where b.rank = 1 group by c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT ) cur left join ( select c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Sum(c.CMPLNT_CNT) as Prev_CMPLNT_CNT, Sum(c.OPRTNTY_CNT) as Prev_OPRTNTY_CNT, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration) as Prev_Version, c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT as Prev_NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT as Prev_NAT_OPRTNTY_CNT from ( select a.Iteration, a.Run, a.run_sequence, a.PREM_DESG_VER_NBR, Rank() over (Order by a.PREM_DESG_VER_NBR Desc, a.run_sequence Desc, a.Iteration Desc) as rank, a.PD_Version from ( select a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end as run_sequence, a.PREM_DESG_VER_NBR, a.PD_Version from PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR a group by a.Iteration, a.Run, case when Upper(a.Run) = 'DEV' then 1 when Upper(a.Run) = 'TRIAL' then 2 when Upper(a.Run) = 'STAGE' then 3 when Upper(a.Run) = 'PROD' then 4 end, a.PREM_DESG_VER_NBR, a.PD_Version ) a ) b inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_MPIN_MSR_SUMMARY c on b.Iteration = c.Iteration and b.PD_Version = c.PD_Version and b.Run = c.Run inner join PD_Reporting.DQC.DQC_341_PROV_ROLLOUT_UNET_MKT d on c.MPIN = d.MPIN and c.Iteration = d.Iteration and c.PD_Version = d.PD_Version and c.Run = d.Run inner join PD_Reporting.DQC.DQC_341_UNET_MKT e on d.UNET_MKT_NBR = e.UNET_MKT_NBR inner join PD_Reporting.DQC.DQC_341_PEG_QLTY_EXPT_MSR f on c.PEG_ANCH_SBCATGY = f.PEG_ANCH_SBCATGY and c.PEG_ANCH_CATGY = f.PEG_ANCH_CATGY and c.SVRTY_LVL_CD = f.SVRTY_LVL_CD and c.QLTY_MSR_NM = f.QLTY_MSR_NM and c.CNFG_POP_SYS_ID = f.CNFG_POP_SYS_ID and c.PREM_SPCL_CD = f.PREM_SPCL_CD and d.Iteration = f.Iteration and d.PD_Version = f.PD_Version and d.Run = f.Run and c.APR_DRG_RLLP_NBR = f.APR_DRG_RLLP_NBR where b.rank = 2 group by c.PEG_ANCH_SBCATGY, c.PEG_ANCH_CATGY, c.SVRTY_LVL_CD, c.PREM_SPCL_CD, Concat('PD', c.PD_Version, '-', c.Run, ' Iteration - ', c.Iteration), c.APR_DRG_RLLP_NBR, c.QLTY_MSR_NM, c.CNFG_POP_SYS_ID, d.UNET_MKT_NBR, e.MKT_DESC, f.NAT_CMPLNC_CNT, f.NAT_OPRTNTY_CNT ) prev on cur.PEG_ANCH_SBCATGY = prev.PEG_ANCH_SBCATGY and cur.PEG_ANCH_CATGY = prev.PEG_ANCH_CATGY and cur.SVRTY_LVL_CD = prev.SVRTY_LVL_CD and cur.PREM_SPCL_CD = prev.PREM_SPCL_CD and cur.APR_DRG_RLLP_NBR = prev.APR_DRG_RLLP_NBR and cur.QLTY_MSR_NM = prev.QLTY_MSR_NM and cur.CNFG_POP_SYS_ID = prev.CNFG_POP_SYS_ID and cur.UNET_MKT_NBR = prev.UNET_MKT_NBR";
            var dqc = await db_sql.LoadData<DQC_DATA_UHPD_SOURCE_Model>(connectionString: ConnectionStringUHPD, strSQL);
            columns = typeof(DQC_DATA_UHPD_SOURCE_Model).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<DQC_DATA_UHPD_SOURCE_Model>(connectionString: ConnectionStringVC, "peg.DQC_DATA_UHPD_SOURCE", dqc, columns, truncate: true);


            //PEG DATA LOAD
            //PEG DATA LOAD
            //PEG DATA LOAD

        }



        //GET CONFIG FROM UGAP
        public async Task UGAPConfig()
        {
            //DECLARE LOCAL VARIABLES
            char chrDelimiter = '|';
            List<string>? strLstColumnNames = null;
            StreamReader? csvreader = null;
            string _strTableName;
            //string[] strLstFiles;
            string[] strLstFiles = Directory.GetFiles(@"C:\Users\cgiorda\Desktop\Projects\UGAP Configuration", "*.txt", SearchOption.TopDirectoryOnly);
            string? strInputLine = "";
            string[] csvArray;
            string strSQL;
            int intBulkSize = 10000;
            IRelationalDataAccess db_sql = new SqlDataAccess();
            IRelationalDataAccess db_td = new TeraDataAccess();
            System.Data.DataTable dtTransfer = new System.Data.DataTable();
            System.Data.DataRow? drCurrent = null;
            string filename;


            //1 GET FILES
            foreach (var strFile in strLstFiles)
            {
                filename = "ugapcfg_" + Path.GetFileName(strFile).Replace(".txt", "");

                var table = CommonFunctions.getCleanTableName(filename);
                var tmp_table = table.Substring(0, Math.Min(28, table.Length)) + "_TMP";


                csvreader = new StreamReader(strFile);
                while ((strInputLine = csvreader.ReadLine()) != null)
                {
                    csvArray = strInputLine.Split(new char[] { chrDelimiter });
                    //FIRST PASS ONLY GETS COLUMNS AND CREATES TABLE SQL
                    if (strLstColumnNames == null)
                    {
                        strLstColumnNames = new List<string>();
                        //GET AND CLEAN COLUMN NAMES FOR TABLE
                        foreach (string c in csvArray)
                        {
                            var colName = c.getSafeFileName();
                            strLstColumnNames.Add(colName.ToUpper());
                        }


                        //SQL FOR TMP TABLE TO STORE ALL VALUES A VARCHAR(MAX)
                        strSQL = CommonFunctions.getCreateTmpTableScript("vct", tmp_table, strLstColumnNames);
                        await db_sql.Execute(connectionString: ConnectionStringVC, strSQL);

                        strSQL = "SELECT * FROM [vct].[" + tmp_table + "]; ";
                        //CREATE TMP TABLE AND COLLECT NEW DB TABLE FOR BULK TRANSFERS
                        dtTransfer = await db_sql.LoadDataTable(ConnectionStringVC, strSQL);
                        dtTransfer.TableName = "vct." + tmp_table;

                        //GOT COLUMNS, CREATED TMP TABLE FOR FIRST PASS
                        continue;
                    }
                    //CLONE ROW FOR TRANSFER
                    drCurrent = dtTransfer.NewRow();
                    //POPULATE ALL COLUMNS FOR CURRENT ROW
                    for (int i = 0; i < strLstColumnNames.Count; i++)
                    {
                        drCurrent[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

                    }
                    dtTransfer.Rows.Add(drCurrent);

                    if (dtTransfer.Rows.Count == intBulkSize) //intBulkSize = 10000 DEFAULT
                    {
                        await db_sql.BulkSave(connectionString: ConnectionStringVC, dtTransfer);
                        dtTransfer.Rows.Clear();
                    }


                }

                //CATCH REST OF UPLOADS OUTSIDE CSV LOOP
                if (dtTransfer.Rows.Count > 0)
                    await db_sql.BulkSave(connectionString: ConnectionStringVC, dtTransfer);


                //GET DATA TYPES TO CREATE DYNAMIC TABLE
                strSQL = CommonFunctions.getTableAnalysisScript("vct", tmp_table, strLstColumnNames);
                var dataTypes = (await db_sql.LoadData<DataTypeModel>(connectionString: ConnectionStringVC, strSQL));

                //USE DATA TYPES ABOVE TO CREATE DYNAMIC TABLE
                strSQL = CommonFunctions.getCreateFinalTableScript("vct", table, dataTypes);
                await db_sql.Execute(connectionString: ConnectionStringVC, strSQL);

                //MOVE DATA FROM TMP TABLE IN FINAL TABLE WITH PROPER TYPES
                strSQL = CommonFunctions.getSelectInsertScript("vct", tmp_table, table, strLstColumnNames);
                await db_sql.Execute(connectionString: ConnectionStringVC, strSQL);

                strLstColumnNames = null;
            }

            //2 GENERTATE FINAL OUTPUT
            strSQL = "Select distinct ETG_BAS_CLSS_NBR, MPC_NBR from CLODM001.ETG_NUMBER";
            var mcp = await db_td.LoadData<UGAPMPCNBRModel>(connectionString: ConnectionStringTD, strSQL);



            strSQL = "SELECT [MPC_NBR] ,[ETG_BAS_CLSS_NBR] ,[ALWAYS] ,[ATTRIBUTED] ,[ERG_SPCL_CATGY_CD] ,[TRT_CD] ,[RX] ,[NRX] ,[RISK_Model] ,[LOW_MONTH] ,[HIGH_MONTH] FROM [VCT_DB].[etgsymm].[VW_UGAPCFG_FINAL]";

            var etg = await db_sql.LoadData<UGAPETGModel>(connectionString: ConnectionStringVC, strSQL);


            foreach (var item in etg)
            {
                var m = mcp.Where(x => x.ETG_BAS_CLSS_NBR == item.ETG_BAS_CLSS_NBR).Select(x => x.MPC_NBR).FirstOrDefault();
                item.MPC_NBR = m;
            }


            List<UGAPETGModel> etg_final = etg.OrderBy(o => o.RISK_Model).ThenBy(o => o.MPC_NBR).ToList();
            StringBuilder sb = new StringBuilder();

            filename = "C:\\Users\\cgiorda\\Desktop\\Projects\\UGAP Configuration\\output\\UGAP_Config_Automated.txt";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (var file = File.CreateText(filename))
            {
                string[] columns = typeof(UGAPETGModel).GetProperties().Select(p => p.Name).ToArray();
                foreach (var column in columns)
                {
                    sb.Append(column + "|");

                }
                file.WriteLine(sb.ToString().TrimEnd('|'));
                file.Flush();
                sb.Clear();

                foreach (var e in etg_final)
                {
                    sb.Append((e.MPC_NBR == null ? "" : e.MPC_NBR) + "|");
                    sb.Append((e.ETG_BAS_CLSS_NBR == null ? "" : e.ETG_BAS_CLSS_NBR) + "|");
                    sb.Append((e.ALWAYS == null ? "" : e.ALWAYS) + "|");
                    sb.Append((e.ATTRIBUTED == null ? "" : e.ATTRIBUTED) + "|");
                    sb.Append((e.ERG_SPCL_CATGY_CD == null ? "" : e.ERG_SPCL_CATGY_CD) + "|");
                    sb.Append((e.TRT_CD == null ? "" : e.TRT_CD) + "|");
                    sb.Append((e.RX == null ? "" : e.RX) + "|");
                    sb.Append((e.NRX == null ? "" : e.NRX) + "|");
                    sb.Append((e.RISK_Model == null ? "" : e.RISK_Model) + "|");
                    sb.Append((e.LOW_MONTH == null ? "" : e.LOW_MONTH) + "|");
                    sb.Append((e.HIGH_MONTH == null ? "" : e.HIGH_MONTH));
                    file.WriteLine(sb.ToString());
                    sb.Clear();
                }
                file.Flush();
            }


            return;
        }


        public async Task getReportsTimelinessAsync(string search_path)
        {
            string file_name; //INDIVIDUAL FILES
            string zip_file_name = ""; //FILE WITHIN ZIP

            string last_file_location = null; //USED TO TRACK WHEN NEXT FILE IS DIFFERENT TAHN ZIP BEING PROCESSED


            List<Report_Timeliness_Model> rtm = new List<Report_Timeliness_Model>();
            IRelationalDataAccess db_sql = new SqlDataAccess();

            //GET FILE MASTER LIST FOR SEARCHING
            var rtfm = await db_sql.LoadData<Report_Timeliness_Files_Model>(connectionString: ConnectionStringMSSQL, "SELECT [ertf_id],[file_location_wild],[file_name_wild] FROM [IL_UCA].[stg].[Evicore_Report_Timeliness_Files] ");
            //GET LATEST MONTH
            var month = Int16.Parse(await db_sql.ExecuteScalar(connectionString: ConnectionStringMSSQL, "SELECT MAX(CASE WHEN [file_month] = 12 THEN  1 ELSE [file_month] + 1  END) FROM [stg].[Evicore_Report_Timeliness] WHERE [file_date] = (SELECT MAX([file_date])  FROM [stg].[Evicore_Report_Timeliness] );") + "");
            //GET LATEST YEAR
            var year = Int16.Parse(await db_sql.ExecuteScalar(connectionString: ConnectionStringMSSQL, "SELECT MAX(CASE WHEN [file_month] = 12 THEN  [file_year] + 1 ELSE [file_year]  END) FROM [stg].[Evicore_Report_Timeliness] WHERE [file_date] = (SELECT MAX([file_date])  FROM [stg].[Evicore_Report_Timeliness] );") + "");




            DateTime dropped_date;
            string found_file_name = null;


            foreach (var rf in rtfm)
            {
                bool is_zip = (rf.file_location_wild.Contains(".zip") ? true : false);

                if (is_zip)
                {
                    var arr = rf.file_location_wild.Split('\\');
                    zip_file_name = arr[arr.Length - 1].Replace("MMMM", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)).Replace("MM", (month < 10 ? "0" + month : month.ToString())).Replace("YYYY", year.ToString()).Replace("YY", year.ToString().Substring(2, 2));
                }

                //LOOP THROUGH ONE ZIP FILE AND CONTINUE
                if (is_zip && last_file_location == rf.file_location_wild)
                {
                    continue;
                }
                last_file_location = rf.file_location_wild; //TRACK LAST FILE FOR ABOVE

                Console.WriteLine("Processing " + month + " " + year + " - " + rf.file_location_wild + "/" + rf.file_name_wild);

                //REPLACE MM YY WITH PROPER DATE DISPLAY
                file_name = rf.file_name_wild.Replace("MMMM", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)).Replace("MM", (month < 10 ? "0" + month : month.ToString())).Replace("YYYY", year.ToString()).Replace("YY", year.ToString().Substring(2, 2));

                var files = Directory.GetFiles(search_path, (is_zip ? zip_file_name : file_name), SearchOption.TopDirectoryOnly);

                foreach (var fl in files) //HOPEFULLY ONLY ONE FILE FOUND. IF NOT CAPTURE IT!
                {
                    FileInfo fi = new FileInfo(fl);
                    dropped_date = fi.CreationTime; //FILE DROPPED DATE

                    if (is_zip) //MULTIPLE FILES WITHIN ZIP
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(fl)) //UNZIP
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries) //LOOP ALL FILES WITHIN
                            {
                                found_file_name = entry.FullName; //CAPTURE FILE NAME WITHIN ZIP

                                var r = new Report_Timeliness_Model(); //NEW INSTANCE PER ROW

                                var cleanString = new string(found_file_name.Where(Char.IsLetter).ToArray()); //ONLY ALPHA CHARS
                                foreach (var x in rtfm) //GET ID's PER EACH FILE IN ZIP
                                {
                                    var cs = new string(x.file_name_wild.Where(Char.IsLetter).ToArray()).Replace("MMMM", "").Replace("MM", "").Replace("YYYY", "").Replace("YY", "");  //ONLY ALPHA CHARS MINUS DATE HOLDERS
                                    if (cleanString.ToLower().StartsWith(cs.ToLower()))
                                    {
                                        r.ertf_id = x.ertf_id; //LINK FILE LIST WITH CURRENT FILE FOUND
                                        break;
                                    }
                                }
                                if (r.ertf_id == null) //NO MACTH WILL APPEAR AS NULL FIELDS IN OUTPUT
                                {
                                    //r.ertf_id = rf.ertf_id;
                                    r.ertf_id = -1;
                                }

                                r.file_location = search_path;
                                r.file_name = found_file_name;
                                r.file_date = new DateTime(year, month, 1);
                                r.file_month = month;
                                r.file_year = year;
                                r.drop_date = dropped_date;
                                rtm.Add(r);

                                Console.WriteLine("Added " + found_file_name);

                            }
                        }
                    }
                    else //INDIVIDUAL FILE
                    {
                        var r = new Report_Timeliness_Model();

                        r.ertf_id = rf.ertf_id;
                        r.file_location = search_path + zip_file_name;
                        r.file_name = file_name;
                        r.file_date = new DateTime(year, month, 1);
                        r.file_month = month;
                        r.file_year = year;
                        r.drop_date = dropped_date;
                        rtm.Add(r);

                        Console.WriteLine("Added " + file_name);
                    }

                }

            }


            //SAVE FINDINGS TO DB
            var columns = typeof(Report_Timeliness_Model).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<Report_Timeliness_Model>(connectionString: ConnectionStringMSSQL, "stg.Evicore_Report_Timeliness", rtm, columns, truncate: false);

        }





        public async Task generateTATReportsAsync()
        {
            //LIST TO HOLD DATA AND SHEETS TO PASS TO EXCEL GENERATOR
            List<ExcelExport> export = new List<ExcelExport>();

            //INSTANC OF EXCEL TAT GENERATOR CLASS
            var closed_xml = new TATExport();

            //INSTANCE OF SQL SERVER GENERIC FUNCTIONS
            IRelationalDataAccess db_sql = new SqlDataAccess();

            int row = 0;

            Console.SetCursorPosition(0, row);
            _console_message = "Get latest dates from stg.EviCore_YTDMetrics";
            _stop_watch.Start();
            string strSQL = "select max(file_date) from stg.EviCore_YTDMetrics;"; //GET LATEST DATE FROM DB
            var obj = await db_sql.ExecuteScalar(connectionString: ConnectionStringMSSQL, strSQL);
            var dt = DateTime.Parse(obj.ToString());
            int month = dt.Month;
            string current = dt.Month + "-" + dt.Year; //STRING LABEL FOR REPORT
            string current_spelled = dt.ToString("MMMM") + ", " + dt.Year;  //STRING LABEL FOR REPORT
            strSQL = "select dateadd(mm, -1, max(file_date)) from stg.EviCore_YTDMetrics;"; //GET LATEST DATE MINUS 1 MONTH FROM DB
            obj = await db_sql.ExecuteScalar(connectionString: ConnectionStringMSSQL, strSQL);
            dt = DateTime.Parse(obj.ToString());
            string previous = dt.Month + "-" + dt.Year; //STRING LABEL FOR REPORT
            _stop_watch.Stop();
            row++;
            _console_message = "";


            int month_total = 0;
            bool ox = false;
            bool mr = false;
            bool cs = false;
            bool com = false;

            string message = "Getting data for";

            //SQL FOUNDATION PROVIDED BY INNA RUDI
            string strSheetName = "Urgent TAT"; //GET SHEET NAME
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "SELECT t.* FROM ( Select s.lob, s.Modality, case when denom is null then 1 else cast(num/denom as decimal(6,4)) end as pct, s.SLA, CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Current' as section from (select * from stg.SLA_Lookup where Metric_id=4 and Is_Archived=0) as s LEFT JOIN (Select report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where file_date in(select max(file_date) from stg.EviCore_TAT) and report_type = 'Urgent TAT' group by report_type,lob, rpt_Modality ) as e on s.modality=e.rpt_modality and s.lob=e.lob and s.metric=e.report_type UNION ALL Select s.lob, s.Modality, case when denom is null then 1 else cast(num/denom as decimal(6,4)) end as pct, s.SLA, CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Previous' as section from (select * from stg.SLA_Lookup where Metric_id=4 and Is_Archived=0) as s LEFT JOIN (Select report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where file_date in(select dateadd(mm,-1,max(file_date)) from stg.EviCore_TAT) and report_type = 'Urgent TAT' group by report_type,lob, rpt_Modality ) as e on s.modality=e.rpt_modality and s.lob=e.lob and s.metric=e.report_type ) t order by t.section, t.lob, t.Modality ";
            var utat = await db_sql.LoadData<TAT_Model>(connectionString: ConnectionStringMSSQL, strSQL); //GET DATA
            export.Add(new ExcelExport() { ExportList = utat.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List
            _stop_watch.Stop();
            row++;
            _console_message = "";

            strSheetName = "Routine TAT";//GET SHEET NAME
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "SELECT t.* FROM ( Select s.lob, s.Modality, case when denom is null then 1 else cast(num/denom as decimal(6,4)) end as pct, s.SLA, CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Current' as section from (select * from stg.SLA_Lookup where Metric_id=3 and Is_Archived=0) as s LEFT JOIN (Select report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where file_date in(select max(file_date) from stg.EviCore_TAT) and report_type = 'Routine TAT' group by report_type,lob, rpt_Modality ) as e on s.modality=e.rpt_modality and s.lob=e.lob and s.metric=e.report_type UNION ALL Select s.lob, s.Modality, case when denom is null then 1 else cast(num/denom as decimal(6,4)) end as pct, s.SLA, CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Previous' as section from (select * from stg.SLA_Lookup where Metric_id=3 and Is_Archived=0) as s LEFT JOIN (Select report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where file_date in(select dateadd(mm,-1,max(file_date)) from stg.EviCore_TAT) and report_type = 'Routine TAT' group by report_type,lob, rpt_Modality ) as e on s.modality=e.rpt_modality and s.lob=e.lob and s.metric=e.report_type ) t order by t.section, t.lob, t.Modality ";
            var rtat = await db_sql.LoadData<TAT_Model>(connectionString: ConnectionStringMSSQL, strSQL);//GET DATA
            export.Add(new ExcelExport() { ExportList = rtat.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List
            _stop_watch.Stop();
            row++;
            _console_message = "";

            strSheetName = "Abandoned Rate";//GET SHEET NAME
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "SELECT t.* FROM ( Select DISTINCT s.lob, s.Modality, Abandoned_Pct as Pct, s.SLA , CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Current' as section FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0) as s left join (Select lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,4) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select max(file_date) from stg.EviCore_YTDMetrics) and LOB<>'E&I' UNION ALL Select lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),5) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select max(file_date) from stg.EviCore_YTDMetrics) and lob='E&I' and rpt_Modality<>'' group by lob,rpt_Modality) as t on s.lob=t.lob and s.Modality=t.rpt_modality UNION ALL Select DISTINCT s.lob, s.Modality, Abandoned_Pct as Pct, s.SLA , CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Previous' as section FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0) as s left join (Select lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,4) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select dateadd(mm,-1,max(file_date)) from stg.EviCore_YTDMetrics) and LOB<>'E&I' UNION ALL Select lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),5) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select dateadd(mm,-1,max(file_date)) from stg.EviCore_YTDMetrics) and lob='E&I' and rpt_Modality<>'' group by lob,rpt_Modality) as t on s.lob=t.lob and s.Modality=t.rpt_modality ) t order by t.section, t.lob, t.Modality ";
            var ar = await db_sql.LoadData<TAT_Model>(connectionString: ConnectionStringMSSQL, strSQL);//GET DATA
            export.Add(new ExcelExport() { ExportList = ar.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List
            _stop_watch.Stop();
            row++;
            _console_message = "";

            strSheetName = "ASA";//GET SHEET NAME
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "SELECT t.* FROM ( Select s.LOB, s.Modality, Avg_Speed_Answer as Pct, s.SLA , CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Current' as section FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0) as s left join (Select lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select max(file_date) from stg.EviCore_YTDMetrics) and LOB<>'E&I' UNION ALL Select lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select max(file_date) from stg.EviCore_YTDMetrics) and lob='E&I' and rpt_Modality<>'' group by lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality UNION ALL Select s.LOB, s.Modality, Avg_Speed_Answer as Pct, s.SLA , CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA, 'Previous' as section FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0) as s left join (Select lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select dateadd(mm,-1,max(file_date)) from stg.EviCore_YTDMetrics) and LOB<>'E&I' UNION ALL Select lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where file_date in(select dateadd(mm,-1,max(file_date)) from stg.EviCore_YTDMetrics) and lob='E&I' and rpt_Modality<>'' group by lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality ) t order by t.section, t.lob, t.Modality ";
            var asa = await db_sql.LoadData<TAT_Model>(connectionString: ConnectionStringMSSQL, strSQL);//GET DATA
            export.Add(new ExcelExport() { ExportList = asa.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List
            _stop_watch.Stop();
            row++;
            _console_message = "";

            strSheetName = "SLA summary, penalties"; //GET SHEET NAME
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("DECLARE @CURRENT_DATE INT;");
            sbSQL.Append("SELECT @CURRENT_DATE= year(max(file_date)) FROM stg.EviCore_TAT;");
            //sbSQL.Append("SET @CURRENT_DATE= 2023;");

            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Avg_Speed_Answer, s.SLA, CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and rpt_Modality<>'' and lob not in('Empire','E&I') UNION ALL Select file_date,lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and lob='E&I' and rpt_Modality<>'' group by file_date,lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,Metric_desc, LOB, Modality ");

            sbSQL.Append("UNION ALL ");

            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Abandoned_Pct, s.SLA, CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and rpt_Modality<>'' and lob not in('Empire','E&I') UNION ALL Select file_date,lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and lob='E&I' and rpt_Modality<>'' group by file_date,lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,metric_desc, LOB, Modality ");

            sbSQL.Append("UNION ALL ");

            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, case when s.lob like '%E&I%' then 'E&I' else s.lob end as lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=3 and Is_Archived=0) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and report_type = 'Routine TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, case when s.lob like '%E&I%' then 'E&I' else s.lob end, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc ");

            sbSQL.Append("UNION ALL ");

            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, case when s.lob like '%E&I%' then 'E&I' else s.lob end as lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=4 and Is_Archived=0) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and report_type = 'Urgent TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, case when s.lob like '%E&I%' then 'E&I' else s.lob end, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc ");

            sbSQL.Append("UNION ALL ");



            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select metric_id,metric_desc,file_date,lob, Modality, CASE when drop_date > datefromparts ( year(drop_date), month(drop_date), ( CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '15')) = 1 AND DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 2 THEN '16' ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 7 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '17')) = 1 THEN '18' ELSE '17' END ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 1 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '16')) = 1 THEN '17' ELSE '16' END ELSE '15' END END END ) ) THEN tot.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select distinct Metric_id,Metric_desc,lob,Modality,penalty_SLA from stg.SLA_Lookup where Is_Archived=0 and Metric_id=5) as b cross join (select distinct file_date,file_month,file_year,drop_date from stg.Evicore_Report_Timeliness where file_year=@CURRENT_DATE) as t ) as tot ) as tmp group by metric_id,metric_desc,lob, Modality ");



            sbSQL.Append("order by modality desc, lob,metric_id  ");

            var tsum = await db_sql.LoadData<TAT_Summary_Model>(connectionString: ConnectionStringMSSQL, sbSQL.ToString());//GET DATA
            export.Add(new ExcelExport() { ExportList = tsum.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List

            _stop_watch.Stop();
            row++;
            _console_message = "";



            strSheetName = "COM SLA summary, penalties";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();


            sbSQL.Remove(0, sbSQL.Length);
            sbSQL.Append("DECLARE @CURRENT_DATE INT;");
            sbSQL.Append("SELECT @CURRENT_DATE= year(max(file_date)) FROM stg.EviCore_TAT;");

            sbSQL.Append("Select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Avg_Speed_Answer, s.SLA, CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0 and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV')) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV') and rpt_Modality<>'' group by file_date,lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,Metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Abandoned_Pct, s.SLA, CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0 and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV')) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV') and rpt_Modality<>'' group by file_date,lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, case when s.lob like '%E&I%' then 'E&I' else s.lob end as lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=3 and Is_Archived=0 and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV')) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV') and report_type = 'Routine TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, case when s.lob like '%E&I%' then 'E&I' else s.lob end, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, case when s.lob like '%E&I%' then 'E&I' else s.lob end as lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=4 and Is_Archived=0 and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV')) as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV') and report_type = 'Urgent TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, case when s.lob like '%E&I%' then 'E&I' else s.lob end, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select metric_id,metric_desc,file_date,lob, Modality, CASE when drop_date > datefromparts ( year(drop_date), month(drop_date), ( CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '15')) = 1 AND DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 2 THEN '16' ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 7 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '17')) = 1 THEN '18' ELSE '17' END ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 1 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '16')) = 1 THEN '17' ELSE '16' END ELSE '15' END END END ) ) THEN tot.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select distinct Metric_id,Metric_desc,lob,Modality,penalty_SLA from stg.SLA_Lookup where Is_Archived=0 and Metric_id=5 and LOB in('E&I','E&I_Notif','E&I_PA','NHP','RV')) as b cross join (select distinct file_date,file_month,file_year,drop_date from stg.Evicore_Report_Timeliness where file_year=@CURRENT_DATE) as t ) as tot ) as tmp group by metric_id,metric_desc,lob, Modality  order by modality desc, lob,metric_id ");

            tsum = await db_sql.LoadData<TAT_Summary_Model>(connectionString: ConnectionStringMSSQL, sbSQL.ToString());//GET DATA

            foreach (var s in tsum)
            {
                var val = ObjectExtensions.GetPropValue(s, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month));
                month_total += int.Parse(val + "");
            }
            if (month_total > 0)
            {
                com = true;
            }
            month_total = 0;

            export.Add(new ExcelExport() { ExportList = tsum.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List

            _stop_watch.Stop();
            row++;
            _console_message = "";


            strSheetName = "OXF SLA summary, penalties";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();

            sbSQL.Remove(0, sbSQL.Length);
            sbSQL.Append("DECLARE @CURRENT_DATE INT;");
            sbSQL.Append("SELECT @CURRENT_DATE= year(max(file_date)) FROM stg.EviCore_TAT;");


            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Avg_Speed_Answer, s.SLA, CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0 and LOB='OHP') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and lob='OHP' and rpt_Modality<>'' group by file_date,lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,Metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Abandoned_Pct, s.SLA, CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0 and LOB='OHP') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, CAST(ROUND(sum(Total_Calls * Avg_Speed_Answer)/sum(Total_Calls),2) as decimal(5,0)) as Avg_Speed_Answer, CAST(ROUND(sum(Abandoned_Calls)/sum(Total_Calls),3) as decimal(5,4)) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and lob='OPH' and rpt_Modality<>'' group by file_date,lob,rpt_Modality) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, case when s.lob like '%E&I%' then 'E&I' else s.lob end as lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=3 and Is_Archived=0 and LOB='OHP') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB='OHP' and report_type = 'Routine TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, case when s.lob like '%E&I%' then 'E&I' else s.lob end, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, case when s.lob like '%E&I%' then 'E&I' else s.lob end as lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=4 and Is_Archived=0 and LOB='OHP') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB='OHP' and report_type = 'Urgent TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, case when s.lob like '%E&I%' then 'E&I' else s.lob end, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select metric_id,metric_desc,file_date,lob, Modality, CASE when drop_date > datefromparts ( year(drop_date), month(drop_date), ( CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '15')) = 1 AND DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 2 THEN '16' ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 7 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '17')) = 1 THEN '18' ELSE '17' END ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 1 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '16')) = 1 THEN '17' ELSE '16' END ELSE '15' END END END ) ) THEN tot.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select distinct Metric_id,Metric_desc,lob,Modality,penalty_SLA from stg.SLA_Lookup where Is_Archived=0 and Metric_id=5 and LOB='OHP') as b cross join (select distinct file_date,file_month,file_year,drop_date from stg.Evicore_Report_Timeliness where file_year=@CURRENT_DATE) as t ) as tot ) as tmp group by metric_id,metric_desc,lob, Modality  order by modality desc, lob,metric_id ");



            tsum = await db_sql.LoadData<TAT_Summary_Model>(connectionString: ConnectionStringMSSQL, sbSQL.ToString());//GET DATA

            foreach (var s in tsum)
            {
                var val = ObjectExtensions.GetPropValue(s, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month));
                month_total += int.Parse(val + "");
            }
            if (month_total > 0)
            {
                ox = true;
            }
            month_total = 0;

            export.Add(new ExcelExport() { ExportList = tsum.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List

            _stop_watch.Stop();
            row++;
            _console_message = "";

            strSheetName = "CS SLA summary, penalties";

            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            sbSQL.Remove(0, sbSQL.Length);
            sbSQL.Append("DECLARE @CURRENT_DATE INT;");
            sbSQL.Append("SELECT @CURRENT_DATE= year(max(file_date)) FROM stg.EviCore_TAT;");


            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Avg_Speed_Answer, s.SLA, CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0 and LOB='C&S') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and rpt_Modality<>'' and LOB='C&S' ) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,Metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Abandoned_Pct, s.SLA, CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0 and LOB='C&S') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and rpt_Modality<>'' and LOB='C&S') as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date,s.lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=3 and Is_Archived=0 and LOB='C&S') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB='C&S' and report_type = 'Routine TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, s.lob, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, s.lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=4 and Is_Archived=0 and LOB='C&S') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB='C&S' and report_type = 'Urgent TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, s.lob, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select metric_id,metric_desc,file_date,lob, Modality, CASE when drop_date > datefromparts ( year(drop_date), month(drop_date), ( CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '15')) = 1 AND DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 2 THEN '16' ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 7 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '17')) = 1 THEN '18' ELSE '17' END ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 1 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '16')) = 1 THEN '17' ELSE '16' END ELSE '15' END END END ) ) THEN tot.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select distinct Metric_id,Metric_desc,lob,Modality,penalty_SLA from stg.SLA_Lookup where Is_Archived=0 and Metric_id=5 and LOB='C&S') as b cross join (select distinct file_date,file_month,file_year,drop_date from stg.Evicore_Report_Timeliness where file_year=@CURRENT_DATE) as t ) as tot ) as tmp group by metric_id,metric_desc,lob, Modality order by modality desc, lob,metric_id ");




            tsum = await db_sql.LoadData<TAT_Summary_Model>(connectionString: ConnectionStringMSSQL, sbSQL.ToString());//GET DATA

            foreach (var s in tsum)
            {
                var val = ObjectExtensions.GetPropValue(s, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month));
                month_total += int.Parse(val + "");
            }
            if (month_total > 0)
            {
                cs = true;
            }
            month_total = 0;

            export.Add(new ExcelExport() { ExportList = tsum.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List

            _stop_watch.Stop();
            row++;
            _console_message = "";



            strSheetName = "MR SLA summary, penalties";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            sbSQL.Remove(0, sbSQL.Length);
            sbSQL.Append("DECLARE @CURRENT_DATE INT;");
            sbSQL.Append("SELECT @CURRENT_DATE= year(max(file_date)) FROM stg.EviCore_TAT;");


            sbSQL.Append("select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Avg_Speed_Answer, s.SLA, CASE WHEN t.Avg_Speed_Answer > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=1 and Is_Archived=0 and LOB='M&R') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and rpt_Modality<>'' and LOB='M&R' ) as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,Metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select Metric_id,Metric_desc, s.file_date,s.LOB, s.Modality, Abandoned_Pct, s.SLA, CASE WHEN t.Abandoned_Pct > s.SLA THEN s.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=2 and Is_Archived=0 and LOB='M&R') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s left join (Select file_date,lob,rpt_Modality, Avg_Speed_Answer, round(Abandoned_Percent,3) as Abandoned_Pct from stg.EviCore_YTDMetrics where year(file_date)=@CURRENT_DATE and rpt_Modality<>'' and LOB='M&R') as t on s.lob=t.lob and s.modality=t.rpt_modality and s.file_date=t.file_date ) as tmp group by Metric_id,metric_desc, LOB, Modality UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date,s.lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=3 and Is_Archived=0 and LOB='M&R') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(LessEqual_2_BUS_Days) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB='M&R' and report_type = 'Routine TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, s.lob, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select s.Metric_id,s.Metric_desc,s.file_date, s.lob,s.Modality, sum(CASE when cast(num/denom as decimal(6,4)) < s.SLA THEN s.Penalty_SLA else 0 end) as Penalty_SLA from (select * FROM (select * from stg.SLA_Lookup WHERE Metric_id=4 and Is_Archived=0 and LOB='M&R') as b cross join (select distinct file_date from stg.EviCore_TAT where year(file_date)=@CURRENT_DATE) as a ) as s LEFT JOIN (Select file_date,report_type,LOB,rpt_modality, cast(sum(Less_State_TAT_Requirements) as decimal) as num, cast(sum(Total_Authorizations_Notifications)as decimal) as denom from stg.EviCore_TAT as e where year(file_date)=@CURRENT_DATE and LOB='M&R' and report_type = 'Urgent TAT' group by file_date,report_type,lob, rpt_Modality ) as t on s.modality=t.rpt_modality and s.lob=t.lob and s.file_date=t.file_date group by Metric_id,Metric_desc, s.lob, s.Modality,s.file_date ) as tmp group by Metric_id,LOB, Modality,metric_desc UNION select Metric_id,LOB, Modality,metric_desc ,isnull(sum(Penalty_SLA), 0) YTD_Penalty ,isnull(sum(case when month(tmp.file_date) = 1 then tmp.Penalty_SLA end), 0) Jan ,isnull(sum(case when month(tmp.file_date) = 2 then tmp.Penalty_SLA end), 0) Feb ,isnull(sum(case when month(tmp.file_date) = 3 then tmp.Penalty_SLA end), 0) Mar ,isnull(sum(case when month(tmp.file_date) = 4 then tmp.Penalty_SLA end), 0) Apr ,isnull(sum(case when month(tmp.file_date) = 5 then tmp.Penalty_SLA end), 0) May ,isnull(sum(case when month(tmp.file_date) = 6 then tmp.Penalty_SLA end), 0) Jun ,isnull(sum(case when month(tmp.file_date) = 7 then tmp.Penalty_SLA end), 0) Jul ,isnull(sum(case when month(tmp.file_date) = 8 then tmp.Penalty_SLA end), 0) Aug ,isnull(sum(case when month(tmp.file_date) = 9 then tmp.Penalty_SLA end), 0) Sep ,isnull(sum(case when month(tmp.file_date) = 10 then tmp.Penalty_SLA end), 0) Oct ,isnull(sum(case when month(tmp.file_date) = 11 then tmp.Penalty_SLA end), 0) Nov ,isnull(sum(case when month(tmp.file_date) = 12 then tmp.Penalty_SLA end), 0) Dec from (Select metric_id,metric_desc,file_date,lob, Modality, CASE when drop_date > datefromparts ( year(drop_date), month(drop_date), ( CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '15')) = 1 AND DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 2 THEN '16' ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 7 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '17')) = 1 THEN '18' ELSE '17' END ELSE CASE WHEN DATEPART(DW,datefromparts(year(drop_date), month(drop_date), '15')) = 1 THEN CASE WHEN dbo.fn_IsHoliday(datefromparts(year(drop_date), month(drop_date), '16')) = 1 THEN '17' ELSE '16' END ELSE '15' END END END ) ) THEN tot.Penalty_SLA else 0 end as Penalty_SLA from (select * FROM (select distinct Metric_id,Metric_desc,lob,Modality,penalty_SLA from stg.SLA_Lookup where Is_Archived=0 and Metric_id=5 and LOB='M&R') as b cross join (select distinct file_date,file_month,file_year,drop_date from stg.Evicore_Report_Timeliness where file_year=@CURRENT_DATE) as t ) as tot ) as tmp group by metric_id,metric_desc,lob, Modality order by modality desc, lob,metric_id  ");

            tsum = await db_sql.LoadData<TAT_Summary_Model>(connectionString: ConnectionStringMSSQL, sbSQL.ToString());//GET DATA

            foreach (var s in tsum)
            {
                var val = ObjectExtensions.GetPropValue(s, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month));
                month_total += int.Parse(val + "");
            }
            if (month_total > 0)
            {
                mr = true;
            }
            month_total = 0;

            export.Add(new ExcelExport() { ExportList = tsum.ToList<object>(), SheetName = strSheetName });//ADD SHEET AND DATA TO export List

            _stop_watch.Stop();
            row++;
            _console_message = "";



            Thread.Sleep(100);

            //GENERATE EXCEL FILE IN BYTES
            _console_message = "Creating final spreadsheet";
            Console.SetCursorPosition(0, row);
            _stop_watch.Reset();
            _stop_watch.Start();
            var bytes = await closed_xml.ExportToTATExcelTemplateAsync(TATReportTemplatePath, export, current, current_spelled, previous, 1, 8, 4);
            _stop_watch.Stop();
            row++;
            _console_message = "";

            //CREATE FILE NAME
            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TAT_Reporting_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";

            //DELETE IF ALREADY EXISTS
            if (File.Exists(file))
                File.Delete(file);

            //CONVERT BYTES TO FINAL EXCEL
            _console_message = "Saving spreadsheet";
            Console.SetCursorPosition(0, row);
            _stop_watch.Reset();
            _stop_watch.Start();
            await File.WriteAllBytesAsync(file, bytes);
            _stop_watch.Stop();
            row++;
            _console_message = "";




            //THIS SECTION TAKES THE FILE ABOVE AND BREAKS THEM INTO 4 ADDITONAL FILES
            _console_message = "Breaking up multiple spreadsheets";
            Console.SetCursorPosition(0, row);
            _stop_watch.Reset();
            _stop_watch.Start();

            string sheet_main = "All SLAs, no current metrics"; //THIS SHEET WILL BE IN ALL
            string sheet_common = "SLA summary, penalties"; //THIS WILL BE APPENDED TO THE 4 BELOW


            List<string> sheets = new List<string>(); //LOOP THROUGH THESE 4
            sheets.Add("COM");
            sheets.Add("MR");
            sheets.Add("CS");
            sheets.Add("OXF");


            foreach (var s in sheets)
            {
                var wb = new XLWorkbook(file); //CRTEAT INTSANCE  FOR NEW FILE

                using (var spreadSheetDocument = SpreadsheetDocument.Open(file, false)) //OPEN FILE ABOVE
                {
                    int sheetIndex = 0;
                    foreach (var worksheetpart in spreadSheetDocument.WorkbookPart.WorksheetParts) //LOOP EACH SHEET
                    {
                        string sheetName = spreadSheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().ElementAt(sheetIndex).Name;


                        if (sheetName != s + " " + sheet_common && sheetName != sheet_main) //IF ITS NOT THE SHEETS THEN DELETE
                        {
                            wb.Worksheet(sheetName).Delete();
                        }

                        sheetIndex++;
                    }
                }

                var final = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + s + "_" + System.IO.Path.GetFileName(file);
                wb.SaveAs(final);//SAVE NEW FILE AND CONTINUE LOOP

            }
            _stop_watch.Stop();
            row++;
            _console_message = "";

            return;


            //HERE WE A START EMAIL PROCESS
            string emailFilePath = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\EmailTemplates\";

            string subject; 
            string body;
            string recipients;
            string from;
            string cc;
            string attachment;
            string attachmentT;

            


            //FOR TESTING
            attachment = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + System.IO.Path.GetFileName(file);
            //To: MaryAnnDimartino
            subject = "TESTING SLA metrics for – " + current;
            body = "<p>" + (ox ? "Oxford Flagged" : "Oxford Skipped") + "</p><p>" + (mr ? "M&R Flagged" : "M&R  Skipped") + "</p><p>" + (cs ? "C&S Flagged" : "C&S Skipped") + "</p><p>" + (com ? "Comm Flagged" : "Comm Skipped") + "</p>";
            recipients = "mary_ann_dimartino@uhc.com";
            from = "chris_giordano@uhc.com";
            cc = "chris_giordano@uhc.com";




            _console_message = "Creating ReportTimeliness spreadsheet";
            Console.SetCursorPosition(0, row);
            _stop_watch.Reset();
            _stop_watch.Start();
            sbSQL.Remove(0, sbSQL.Length);
            sbSQL.Append("SELECT j.[ertf_id] ,[file_location_wild] ,[file_name_wild] ,[file_name] ,[file_date] ,[file_month] ,[file_year] ,[drop_date] FROM [stg].[Evicore_Report_Timeliness_Files] j INNER JOIN [IL_UCA].[stg].[Evicore_Report_Timeliness] t ON j.[ertf_id] = t.[ertf_id] ORDER BY [file_date], j.[ertf_id]");
            var rt = await db_sql.LoadData<Report_Timeliness_Output_Model>(connectionString: ConnectionStringMSSQL, sbSQL.ToString());//GET DATA
            var columns = typeof(Report_Timeliness_Output_Model).GetProperties().Select(p => p.Name).ToList();
            var closed_xmlf = new ClosedXMLFunctions();
            bytes = await closed_xmlf.ExportToExcelAsync(rt.ToList(), "ReportTimeliness");
            attachmentT = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Report_Timeliness_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";
            _stop_watch.Stop();
            row++;
            _console_message = "";


            //CONVERT BYTES TO FINAL EXCEL
            _console_message = "Saving ReportTimeliness spreadsheet";
            Console.SetCursorPosition(0, row);
            _stop_watch.Reset();
            _stop_watch.Start();
            await File.WriteAllBytesAsync(attachmentT, bytes);
            _stop_watch.Stop();
            row++;
            _console_message = "";



            await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment + ";" + attachmentT, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);



            //ox = false;
            //mr = true;
            //cs = true;
            //com = false;

            //COMPLETE EMAILS
            if (ox || mr || cs || com)
            {
  

                //To: Rosamond Eschert; CC Laura Fischer
                subject = "SLA metrics for – " + current;
                body = File.ReadAllText(emailFilePath + "SLA_TAT.txt").Replace("[Date]", current);
                recipients = "mary_ann_dimartino@uhc.com";
                from = "chris_giordano@uhc.com";
                cc = "chris_giordano@uhc.com";

                await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);



                //To: Judith Bourdeau
                subject = "CareCore SLA metrics for – " + current;
                body = File.ReadAllText(emailFilePath + "CareCore_TAT.txt").Replace("[Date]", current);
                recipients = "mary_ann_dimartino@uhc.com";
                from = "chris_giordano@uhc.com";
                cc = "chris_giordano@uhc.com";

                await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);

            }


            //COM EMAIL
            if (com)
            {

                attachment = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "COM_" + System.IO.Path.GetFileName(file);

                //E&I: To: Kathryn Tschida (E&I finance); CC: Laura Fischer
                subject = "COM - CCN SLA Penalties – " + current;
                body = File.ReadAllText(emailFilePath + "COMM_TAT.txt").Replace("[Date]", current);
                recipients = "mary_ann_dimartino@uhc.com";
                from = "chris_giordano@uhc.com";
                cc = "chris_giordano@uhc.com";

                await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);

            }

            //OX EMAIL
            if (ox)
            {

                attachment = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "OXF_" + System.IO.Path.GetFileName(file);

                //Oxford: To: Sharon Wallhofer; CC: Chris Jacozzi, Allyson Clark, and Laura Fischer 
                subject = "Oxford - CCN SLA Penalties – " + current;
                body = File.ReadAllText(emailFilePath + "OXF_TAT.txt").Replace("[Date]", current);
                recipients = "mary_ann_dimartino@uhc.com";
                from = "chris_giordano@uhc.com";
                cc = "chris_giordano@uhc.com";

                await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);

            }

            //CS EMAIL
            if (cs)
            {

                attachment = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "CS_" + System.IO.Path.GetFileName(file);

                //Oxford: To: Sharon Wallhofer; CC: Chris Jacozzi, Allyson Clark, and Laura Fischer 
                subject = "C&S - CCN SLA Penalties – " + current;
                body = File.ReadAllText(emailFilePath + "CS_TAT.txt").Replace("[Date]", current);
                recipients = "mary_ann_dimartino@uhc.com";
                from = "chris_giordano@uhc.com";
                cc = "chris_giordano@uhc.com";

                await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);

            }

            //MR EMAIL
            if (mr)
            {

                attachment = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "MR_" + System.IO.Path.GetFileName(file);

                //Oxford: To: Sharon Wallhofer; CC: Chris Jacozzi, Allyson Clark, and Laura Fischer 
                subject = "M&R - CCN SLA Penalties – " + current;
                body = File.ReadAllText(emailFilePath + "MR_TAT.txt").Replace("[Date]", current);
                recipients = "mary_ann_dimartino@uhc.com";
                from = "chris_giordano@uhc.com";
                cc = "chris_giordano@uhc.com";

                await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, attachment, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);

            }




        }

       

        public async Task generatePEGReportsAsync()
        {

            List<ExcelExport> export = new List<ExcelExport>();

            var closed_xml = new ClosedXMLFunctions();

            string message = "Getting data for";
            int row = 0;

            IRelationalDataAccess db_sql = new SqlDataAccess();


            string strSheetName = "PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Start();
            string strSQL = "select Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var peg = await db_sql.LoadData<PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = peg.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "PEG Subcat";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ',VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var pegsub = await db_sql.LoadData<PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = pegsub.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end";
            var lob = await db_sql.LoadData<LOB_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lob.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Specialty";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD";
            var spec = await db_sql.LoadData<Specialty_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = spec.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Quality Metric";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var qm = await db_sql.LoadData<Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qm.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var r = await db_sql.LoadData<Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = r.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Major Market";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var maj = await db_sql.LoadData<Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = maj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Minor Market";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var min = await db_sql.LoadData<Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = min.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lp = await db_sql.LoadData<LOB_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "LOB by PEG Subcat";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lps = await db_sql.LoadData<LOB_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Specialty by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var sp = await db_sql.LoadData<Specialty_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by PEG Subcat";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var sps = await db_sql.LoadData<Spec_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Quality Metric by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmp = await db_sql.LoadData<Quality_Metric_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Qual Metric by PEG Subcat";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmps = await db_sql.LoadData<Quality_Metric_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Spec";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD";
            var ls = await db_sql.LoadData<LOB_Spec_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = ls.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Qual Metric";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var lqm = await db_sql.LoadData<LOB_Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqm.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "LOB by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var lr = await db_sql.LoadData<LOB_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "SELECT case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant FROM VCT_DB.peg.VW_PEG_Final GROUP BY case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var lmaj = await db_sql.LoadData<LOB_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var lmin = await db_sql.LoadData<LOB_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Quality Metric";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var sqm = await db_sql.LoadData<Spec_Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqm.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var sr = await db_sql.LoadData<Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var smaj = await db_sql.LoadData<Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Spec by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var smin = await db_sql.LoadData<Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Qual Metric by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var qmr = await db_sql.LoadData<Quality_Metric_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Qual Metric by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var qmMaj = await db_sql.LoadData<Quality_Metric_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmMaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Qual Metric by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var qmMin = await db_sql.LoadData<Quality_Metric_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmMin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "LOB by Qual Metric by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lqmp = await db_sql.LoadData<LOB_Quality_Metric_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Qual Metric by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lqmsc = await db_sql.LoadData<LOB_Quality_Metric_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmsc.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Qual Metric by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var sqmp = await db_sql.LoadData<Spec_Quality_Metric_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Qual Metric by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var sqmps = await db_sql.LoadData<Spec_Quality_Metric_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Region by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lrp = await db_sql.LoadData<LOB_Region_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lrp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Region by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lrpsc = await db_sql.LoadData<LOB_Region_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lrpsc.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "LOB by Maj Mkt by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lmajp = await db_sql.LoadData<LOB_Major_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmajp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "LOB by Maj Mkt by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lmajps = await db_sql.LoadData<LOB_Major_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmajps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Min Mkt by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lminp = await db_sql.LoadData<LOB_Minor_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lminp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Min Mkt by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lminps = await db_sql.LoadData<LOB_Minor_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lminps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Spec by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var lsr = await db_sql.LoadData<LOB_Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Spec by Maj Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var lsmaj = await db_sql.LoadData<LOB_Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Spec by Min Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var lsmin = await db_sql.LoadData<LOB_Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Qual Metric by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var lqmr = await db_sql.LoadData<LOB_Quality_Metric_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Qual Metric by Maj Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var lqmmaj = await db_sql.LoadData<LOB_Quality_Metric_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Qual Metric by Min Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var lqmin = await db_sql.LoadData<LOB_Quality_Metric_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Spec by Qual Metric";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var lsqm = await db_sql.LoadData<LOB_Spec_Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsqm.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Region by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var srp = await db_sql.LoadData<Spec_Region_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = srp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Region by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var srps = await db_sql.LoadData<Spec_Region_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = srps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Major Mkt by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var smajp = await db_sql.LoadData<Spec_Major_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smajp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Spec by Major Mkt by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var smajps = await db_sql.LoadData<Spec_Major_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smajps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Minor Mkt by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var sminp = await db_sql.LoadData<Spec_Minor_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sminp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Minor Mkt by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var sminps = await db_sql.LoadData<Spec_Minor_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sminps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Qual Metric by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var sqmr = await db_sql.LoadData<Spec_Quality_Metric_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Qual Met by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var sqmmaj = await db_sql.LoadData<Spec_Quality_Metric_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Spec by Qual Met by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var sqmmin = await db_sql.LoadData<Spec_Quality_Metric_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Qual Met by Region by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmrp = await db_sql.LoadData<Quality_Metric_Region_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmrp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Qual Met by Region by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmrps = await db_sql.LoadData<Quality_Metric_Region_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmrps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Qual Met by Major Mkt by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmmajp = await db_sql.LoadData<Quality_Metric_Major_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmmajp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Qual Met by Maj Mkt by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmmajps = await db_sql.LoadData<Quality_Metric_Major_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmmajps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Qual Met by Minor Mkt by PEG";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmminp = await db_sql.LoadData<Quality_Metric_Minor_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmminp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;

            strSheetName = "Qual Met by Min Mkt by PEG Subc";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset();
            _stop_watch.Start();
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmminps = await db_sql.LoadData<Quality_Metric_Minor_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmminps.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();




            Thread.Sleep(100);


            _console_message = "Creating final spreadsheet";
            Console.SetCursorPosition(0, 0);
            Console.Clear();
            _stop_watch.Reset();
            _stop_watch.Start();
            var bytes = await closed_xml.ExportToExcelTemplateAsync(PEGReportTemplatePath, export);
            _stop_watch.Stop();



            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PEG_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


            if (File.Exists(file))
                File.Delete(file);

            _console_message = "Opening final spreadsheet";
            Console.SetCursorPosition(0, 1);
            _stop_watch.Reset();
            _stop_watch.Start();
            await File.WriteAllBytesAsync(file, bytes);
            _stop_watch.Stop();



            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file)
            {
                UseShellExecute = true
            };
            p.Start();


        }


        public async Task generateEBMReportsAsync()
        {

            List<ExcelExport> export = new List<ExcelExport>();

            var closed_xml = new ClosedXMLFunctions();

            string message = "Getting data for";
            int row = 0;

            IRelationalDataAccess db_sql = new SqlDataAccess();
            string strSheetName = "Measure";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            string strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC)";
            var meas = await db_sql.LoadData<Measure_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = meas.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB";
            var lob = await db_sql.LoadData<LOB_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lob.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Specialty";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var spec = await db_sql.LoadData<Specialty_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = spec.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var reg = await db_sql.LoadData<Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = reg.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Major Market";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var maj = await db_sql.LoadData<Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = maj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Minor Market";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var min = await db_sql.LoadData<Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = min.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;




            strSheetName = "Measure by LOB";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB";
            var ml = await db_sql.LoadData<Measure_LOB_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = ml.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Measure by Specialty";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var msp = await db_sql.LoadData<Measure_Specialty_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msp.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Measure by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var mr = await db_sql.LoadData<Measure_Region_Mode>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Measure by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM As Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var mmaj = await db_sql.LoadData<Measure_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Measure by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var mmin = await db_sql.LoadData<Measure_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;




            strSheetName = "LOB by Specialty";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var ls = await db_sql.LoadData<LOB_Spec_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = ls.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var lr = await db_sql.LoadData<LOB_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var lmaj = await db_sql.LoadData<LOB_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var lmin = await db_sql.LoadData<LOB_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;




            strSheetName = "Specialty by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var sr = await db_sql.LoadData<Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;





            strSheetName = "Specialty by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var smaj = await db_sql.LoadData<Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;




            strSheetName = "Specialty by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var  smin = await db_sql.LoadData<Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Measure by LOB by Specialty";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var mls = await db_sql.LoadData<Measure_LOB_Spec_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mls.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Measure by LOB by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var mlr = await db_sql.LoadData<Measure_LOB_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mlr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Measure by LOB by Major Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var mlmaj = await db_sql.LoadData<Measure_LOB_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mlmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "Measure by LOB by Minor Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var mlmin = await db_sql.LoadData<Measure_LOB_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mlmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Measure by Specialty by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var msr = await db_sql.LoadData<Measure_Specialty_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Measure by Specialty by Maj Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var msmaj = await db_sql.LoadData<Measure_Specialty_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "Measure by Specialty by Min Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var msmin = await db_sql.LoadData<Measure_Specialty_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;


            strSheetName = "LOB by Specialty by Region";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var lsr = await db_sql.LoadData<LOB_Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsr.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "LOB by Specialty by Maj Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var lsmaj = await db_sql.LoadData<LOB_Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmaj.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();
            row++;



            strSheetName = "LOB by Specialty by Min Mkt";
            Console.SetCursorPosition(0, row);
            _console_message = message + " " + strSheetName;
            _stop_watch.Reset(); _stop_watch.Start();
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var lsmin = await db_sql.LoadData<LOB_Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmin.ToList<object>(), SheetName = strSheetName });
            _stop_watch.Stop();


            Thread.Sleep(100);


            _console_message = "Creating final spreadsheet";
            Console.SetCursorPosition(0, 0);
            Console.Clear(); 
            _stop_watch.Reset(); _stop_watch.Start();
            var bytes = await closed_xml.ExportToExcelTemplateAsync(EBMReportTemplatePath, export);
            _stop_watch.Stop();

            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EBM_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


            if (File.Exists(file))
                File.Delete(file);


            Thread.Sleep(100);


            _console_message = "Opening final spreadsheet";
            Console.SetCursorPosition(0, 1);
            _stop_watch.Reset(); _stop_watch.Start();
            await File.WriteAllBytesAsync(file, bytes);
            _stop_watch.Stop();


            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file)
            {
                UseShellExecute = true
            };
            p.Start();


        }


        //EBM SOURCE AUTOMATION
        public async Task getEDCSourceDataAsync()
        {
    

            IRelationalDataAccess db_sql = new SqlDataAccess();
            IRelationalDataAccess db_td = new TeraDataAccess();


            Console.WriteLine("Getting ProviderSys from GALAXY...");
            //2 GET MPIN PROV_SYS_ID FROM GALAXY
            string strSQL = "SELECT DISTINCT MPIN,PROV_SYS_ID FROM GALAXY.PROVIDER where MPIN <> 0 and Prov_sys_id > 999999999";
            var mp = await db_td.LoadData<Provider_MPIN>(connectionString: ConnectionStringGalaxy, strSQL);



            Console.WriteLine("Getting MPIN TIN from NDAR...");
            //1 GET MPIN TIN FROM NDAR
            strSQL = "select DISTINCT TaxID as TIN,CorpOwnerName as TIN_Name,mpin as MPIN,FullName as MPIN_Name from dbo.NationalDataAggregation where CorpOwnerID<>0 and ProvType in ('O') and MPIN is not null;";
            var tmp = await db_sql.LoadData<Tin_Mpin_Prov>(connectionString: ConnectionStringNDAR, strSQL);


            Console.WriteLine("MERGING Results...");
            //3 MERGE RESULTS
            var tmp_final = from n in tmp
                         join p in mp on n.MPIN equals p.MPIN into n_p_join
                         from np in n_p_join.DefaultIfEmpty()
                         select new Tin_Mpin_Prov
                         {
                             TIN = n.TIN,
                             MPIN = n.MPIN,
                             PROV_SYS_ID = (np == null ? null :np.PROV_SYS_ID),
                             TIN_Name = n.TIN_Name,
                             MPIN_Name = n.MPIN_Name,
           
       
                         };

            Console.WriteLine("Loading to DB...");
            //4 SAVE FINAL TO DB
            string[] columns = typeof(Tin_Mpin_Prov).GetProperties().Select(p => p.Name).ToArray();
            await db_sql.BulkSave<Tin_Mpin_Prov>(connectionString: ConnectionStringVC, "edcadhoc.Tin_Mpin_Prov_Filters", tmp_final, columns, truncate: true);



        }


        public async Task parseCSV(string filepath,  string fileNamePrefix ="csvg_", string filetype = "csv",char chrDelimiter = '|', string schema = "stg", SearchOption so = SearchOption.TopDirectoryOnly)
        {
            List<string>? strLstColumnNames = null;
            StreamReader? csvreader = null;
            string _strTableName;
            string[] strLstFiles = Directory.GetFiles(filepath, "*." + filetype, so);
            string? strInputLine = "";
            string[] csvArray;
            string strSQL;
            int intBulkSize = 10000;

            IRelationalDataAccess db_dest = new SqlDataAccess();
            System.Data.DataTable dtTransfer = new System.Data.DataTable();
            System.Data.DataRow? drCurrent = null;
            foreach (var strFile in strLstFiles)
            {
                var filename = fileNamePrefix + Path.GetFileName(strFile).Replace("." + filetype, "");

                var table = CommonFunctions.getCleanTableName(filename);
                var tmp_table = table.Substring(0, Math.Min(28, table.Length)) + "_TMP";
                
                
                csvreader = new StreamReader(strFile);
                while ((strInputLine = csvreader.ReadLine()) != null)
                {
                    csvArray = strInputLine.Split(new char[] { chrDelimiter });
                    //FIRST PASS ONLY GETS COLUMNS AND CREATES TABLE SQL
                    if (strLstColumnNames == null)
                    {
                        strLstColumnNames = new List<string>();
                        //GET AND CLEAN COLUMN NAMES FOR TABLE
                        foreach (string c in csvArray)
                        {
                            var colName = c.getSafeFileName();
                            strLstColumnNames.Add(colName.ToUpper());
                        }


                        //SQL FOR TMP TABLE TO STORE ALL VALUES A VARCHAR(MAX)
                        strSQL = CommonFunctions.getCreateTmpTableScript(schema, tmp_table, strLstColumnNames);
                        await db_dest.Execute(connectionString: ConnectionStringMSSQL, strSQL); 

                        

                        strSQL = "SELECT * FROM ["+ schema + "].[" + tmp_table + "]; ";
                        //CREATE TMP TABLE AND COLLECT NEW DB TABLE FOR BULK TRANSFERS
                        dtTransfer = await db_dest.LoadDataTable(ConnectionStringMSSQL, strSQL);

                        dtTransfer.TableName =  schema + "." + tmp_table;

                        //GOT COLUMNS, CREATED TMP TABLE FOR FIRST PASS
                        continue;
                    }
                    //CLONE ROW FOR TRANSFER
                    drCurrent = dtTransfer.NewRow();
                    //POPULATE ALL COLUMNS FOR CURRENT ROW
                    for (int i = 0; i < strLstColumnNames.Count; i++)
                    {
                        drCurrent[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

                    }
                    dtTransfer.Rows.Add(drCurrent);

                    if (dtTransfer.Rows.Count == intBulkSize) //intBulkSize = 10000 DEFAULT
                    {
                        await db_dest.BulkSave(connectionString: ConnectionStringMSSQL, dtTransfer);
                        dtTransfer.Rows.Clear();
                    }


                }

                //CATCH REST OF UPLOADS OUTSIDE CSV LOOP
                if (dtTransfer.Rows.Count > 0)
                    await db_dest.BulkSave(connectionString: ConnectionStringMSSQL, dtTransfer);

                

                strSQL = CommonFunctions.getTableAnalysisScript(schema, tmp_table, strLstColumnNames);
                var dataTypes = (await db_dest.LoadData<DataTypeModel>(connectionString: ConnectionStringMSSQL, strSQL));

                strSQL = CommonFunctions.getCreateFinalTableScript(schema, table, dataTypes);
                await db_dest.Execute(connectionString: ConnectionStringMSSQL, strSQL);

                strSQL = CommonFunctions.getSelectInsertScript(schema, tmp_table, table, strLstColumnNames);
                await db_dest.Execute(connectionString: ConnectionStringMSSQL, strSQL);

                strLstColumnNames = null;
            }
        }

        //PPACA_TAT
        public async Task PPACA_TAT_Email()
        {


            //TWO DBS
            IRelationalDataAccess db_sql = new SqlDataAccess();


            string strSQL = "SELECT [file_month] ,[file_year] ,[num_tat] ,[den_tat] ,[tat_val] ,[rtype] FROM [IL_UCA].[dbo].[VW_PPACA_TAT]";
            StringBuilder sbEmail = new StringBuilder();

            string recipients = "LAlfonso@uhc.com;allyson_k_clark@uhc.com;sanford_p_cohen@uhc.com;laura_fischer@uhc.com;mayrene_hernandez@uhc.com;steve_lumpinski@optum.com;renee_l_struck@uhc.com;jessica_l_tarnowski@uhc.com;heather_vanis@uhc.com;mark_j_newman@uhc.com;Judy.Fujimoto@optum.com;carol_s_winter@uhc.com;inez.bulatao@uhc.com;nancy.morden@uhc.com;christopher_pauwels@uhc.com;roma_adipat@uhc.com;dana.savoie@optum.com;laurie.gianturco@uhc.com;rosamond_e_eschert@uhc.com;loaiello@uhc.com;candace_smith@uhc.com;stacy_v_washington@uhc.com; Carella-lisa.carellaashla@uhc.com;jon_maguire@uhc.com;inna_rudi@uhc.com";

            recipients = "mary_ann_dimartino@uhc.com;hong_gao@uhc.com;chris_giordano@uhc.com";
            recipients = "mary_ann_dimartino@uhc.com";
            string from = "chris_giordano@uhc.com";
            string cc = "chris_giordano@uhc.com;inna_rudi@uhc.com";
            //cc = "chris_giordano@uhc.com";
            //recipients = "chris_giordano@uhc.com";

            string emailFilePath = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\EmailTemplates\PPACA_TAT.txt";


            DataRow dr;
            DataTable dt  = await db_sql.LoadDataTable(ConnectionStringMSSQL, strSQL);
            strSQL = "select top 1 file_month, [file_year],file_date FROM [IL_UCA].[stg].[EviCore_TAT] where file_date = (select max(file_date) from[IL_UCA].[stg].[EviCore_TAT])";
            DataTable dtDate = await db_sql.LoadDataTable(ConnectionStringMSSQL, strSQL);

            string fileSearch = "United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_" + dtDate.Rows[0]["file_year"] + "_" + dtDate.Rows[0]["file_month"] + ".xlsx";
            DateTime fileDate = (DateTime)dtDate.Rows[0]["file_date"];
            string filePath = @"\\NASGWFTP03\Care_Core_FTP_Files\Radiology";
            FileInfo fi = new FileInfo(filePath + "\\" + fileSearch);
            DateTime dtCreateDate = fi.CreationTime;


            int file_month = int.Parse(dt.Rows[0]["file_month"] + "");
            int file_year = int.Parse(dt.Rows[0]["file_year"] + "");
            string strMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(file_month);

            string subject = "United Enterprise Wide PPACA TAT Report - " + strMonthName + " " + file_year; //October 2022


            string body = File.ReadAllText(emailFilePath);
            body = body.Replace("{$month}", strMonthName);
            body = body.Replace("{$year}", file_year.ToString());
            body = body.Replace("{$current_month}", dtCreateDate.ToString("MMMM"));
            body = body.Replace("{$current_year}", dtCreateDate.Year.ToString());
            dr = dt.Select("rtype = 'CS'").FirstOrDefault();
            body = body.Replace("{$tat_val_cs}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_cs}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_cs}", String.Format("{0:n0}", dr["den_tat"]));
            dr = dt.Select("rtype = 'MR'").FirstOrDefault();
            body = body.Replace("{$tat_val_mr}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_mr}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_mr}", String.Format("{0:n0}", dr["den_tat"]));
            dr = dt.Select("rtype = 'COMM'").FirstOrDefault();
            body = body.Replace("{$tat_val_comm}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_comm}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_comm}", String.Format("{0:n0}", dr["den_tat"]));
            dr = dt.Select("rtype = 'OX'").FirstOrDefault();
            body = body.Replace("{$tat_val_ox}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_ox}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_ox}", String.Format("{0:n0}", dr["den_tat"]));


            var manual = @"C:\Users\cgiorda\Desktop\Projects\PPACA_TAT\Archive\United_Enterprise_Wide_Urgent_TAT_UHC_Enterprise_2023_09.xlsx";


            await SharedFunctions.EmailAsync(recipients, from, subject, body, cc, manual,  System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
        }


        //FOR MARY ANN NOT CURRENTLY IN USE
        public async Task runSLAAutomation()
        {
            var date = "03/01/2022";
            var last_thursday = getLastChosenDayOfTheMonth(DateTime.ParseExact(date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture), DayOfWeek.Thursday);

            //DB
            string connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
            IRelationalDataAccess db_sql = new SqlDataAccess();
            var results = await db_sql.LoadData<MonthlySLAReviewModel, dynamic>(connectionString: connectionString, storedProcedure: "dbo.sp_Monthly_SLA_Review", new { Date = date });
            //var results = await db_sql.LoadData<MonthlySLAReviewModel, dynamic>(connectionString: connectionString, storedProcedure: "dbo.sp_Monthly_SLA_Review", new {});


            //WORD
            var fontType = "Times New Roman";
            var fontSize = 12;
            var bold = false;
            string file = "C:\\Users\\cgiorda\\Desktop\\Projects\\Monthly SLA Review Call\\Monthly SLA Review Call_template.docx";
            string file_OUT = "C:\\Users\\cgiorda\\Desktop\\Projects\\Monthly SLA Review Call\\AutomatedMonthlySLASample_" + date.Replace("/", "_") + ".docx";
            var writer = new InteropWordFunctions(file);


            //PROCESS
            var bookmark_name = "";
            var currentModality = "";
            var text = "";
            var color = System.Drawing.Color.Black;
            List<MSWordFormattedText> lst = new List<MSWordFormattedText>();
            foreach (var row in results)
            {
                if (currentModality != row.Modality)
                {
                    //IF LIST IS POPULATED, PROCESS IT
                    if (lst.Count > 0)
                    {
                        writer.addBulletedList(bookmark_name, lst, 2);

                        lst = new List<MSWordFormattedText>();
                    }
                    currentModality = row.Modality;
                }


                bookmark_name = (row.LOB + "_" + row.Modality).Replace("&", "").ToLower();


                if (row.Penalty_SLA != 0)
                {
                    text = row.Miss.Replace("[SLA]", row.SLA.ToString()).Replace("[Percentage]", row.Percentage.ToString());
                    color = System.Drawing.Color.Red;
                }
                else
                {
                    text = row.Hit;
                    color = System.Drawing.Color.Black;
                }

                lst.Add(new MSWordFormattedText() { Text = text, Bold = false, FontType = fontType, FontSize = fontSize, ForeColor = color });
            }
            if (lst.Count > 0)
            {
                writer.addBulletedList(bookmark_name, lst, 2);
            }


            writer.FindAndReplaceInHeader("[Date]", last_thursday.ToString("MMMM") + " " + last_thursday.Day + ", " + last_thursday.Year);

            if (System.IO.File.Exists(file_OUT))
                System.IO.File.Delete(file_OUT);

            writer.Save(file_OUT);

            writer.DisposeWordInstance();

            return;
        }



    }




}
