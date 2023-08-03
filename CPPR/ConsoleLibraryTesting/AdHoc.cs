using Dapper;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using DataAccessLibrary.Scripts;
using DataAccessLibrary.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using FileParsingLibrary.Models;
using FileParsingLibrary.MSExcel;
using FileParsingLibrary.MSWord;
using MathNet.Numerics.Providers.SparseSolver;
using Org.BouncyCastle.Utilities;
using ProjectManagerLibrary.Models;
using ProjectManagerLibrary.Shared;
using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.DQC_Reporting;
using VCPortal_Models.Models.EBM;
using VCPortal_Models.Models.ETGFactSymmetry.Configs;
using VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
using VCPortal_Models.Models.PEG;
using VCPortal_Models.Models.Shared;
using VCPortal_Models.Parameters.MHP;

namespace ConsoleLibraryTesting
{
    public class AdHoc
    {


        public string PEGReportTemplatePath { get; set; }
        public string EBMReportTemplatePath { get; set; }

        public string ConnectionStringVC { get; set; }

        public string ConnectionStringMSSQL { get; set; }


        public string ConnectionStringUHPD { get; set; }

        public string ConnectionStringPD { get; set; }


        public string ConnectionStringUHN { get; set; }


    public string TableMHP { get; set; }
        public string ConnectionStringTD { get; set; }
        public string TableUGAP { get; set; }
        public int Limit { get; set; }


        public async Task  runSLAAutomation()
        {
            var date = "03/01/2022";
            var last_thursday = AdHoc.GetLastChosenDayOfTheMonth(DateTime.ParseExact(date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture), DayOfWeek.Thursday);

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





        public static  DateTime GetLastChosenDayOfTheMonth(DateTime date, DayOfWeek dayOfWeek)
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
                Console.WriteLine(total + " records found");
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


            //StringBuilder sb = new StringBuilder();

            //foreach (string file in files_loaded)
            //{
            //    sb.Append("'" + file + "',");
            //}

            //string strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Yearly_Universes]  WHERE file_name in (" + sb.ToString().TrimEnd(',') + ");";
            //var mhp = await db_sql.LoadData<MHPUniverseModel>(connectionString: ConnectionStringMSSQL, strSQL);
            //columns = typeof(MHPUniverseModel).GetProperties().Select(p => p.Name).ToArray();
            //await db_sql.BulkSave<MHPUniverseModel>(connectionString: ConnectionStringVCPMSSQL, "mhp.MHP_Yearly_Universes", mhp, columns);

            //strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] WHERE mhp_uni_id in (SELECT [mhp_uni_id] FROM [IL_UCA].[stg].[MHP_Yearly_Universes] WHERE file_name in (" + sb.ToString().TrimEnd(',') + "));";
            //var mhp_ugap = await db_sql.LoadData<MHPMemberDetailsModel>(connectionString: ConnectionStringMSSQL, strSQL);
            //columns = typeof(MHPMemberDetailsModel).GetProperties().Select(p => p.Name).ToArray();
            //await db_sql.BulkSave<MHPMemberDetailsModel>(connectionString: ConnectionStringVCPMSSQL, "mhp.MHP_Yearly_Universes_UGAP", mhp_ugap, columns);


            await db_sql.Execute(ConnectionStringMSSQL, "exec [IL_UCA].[dbo].[sp_mhp_refesh_filter_cache]");


            //strSQL = "SELECT * FROM  [IL_UCA].[dbo].[cs_product_map];";
            //var pm = await db_sql.LoadData<CS_Product_Map>(connectionString: ConnectionStringMSSQL, strSQL);
            //columns = typeof(CS_Product_Map).GetProperties().Select(p => p.Name).ToArray();
            //await db_sql.BulkSave<CS_Product_Map>(connectionString: ConnectionStringVCPMSSQL, "vct.cs_product_map", pm, columns, truncate: true);


            //strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Group_State];";
            //var gs = await db_sql.LoadData<MHP_Group_State_Model>(connectionString: ConnectionStringMSSQL, strSQL);
            //columns = typeof(MHP_Group_State_Model).GetProperties().Select(p => p.Name).ToArray();
            //await db_sql.BulkSave<MHP_Group_State_Model>(connectionString: ConnectionStringVCPMSSQL, "mhp.MHP_Group_State", gs, columns, truncate: true);


            //strSQL = "SELECT * FROM  [IL_UCA].[stg].[MHP_Universes_Filter_Cache];";
            //var fs = await db_sql.LoadData<MHP_Reporting_Filters>(connectionString: ConnectionStringMSSQL, strSQL);
            //columns = typeof(MHP_Reporting_Filters).GetProperties().Select(p => p.Name).ToArray();
            //await db_sql.BulkSave<MHP_Reporting_Filters>(connectionString: ConnectionStringVCPMSSQL, "mhp.MHP_Universes_Filter_Cache", fs, columns, truncate: true);



            //await SharedFunctions.EmailAsync("jon.piotrowski@uhc.com;renee_l_struck@uhc.com;hong_gao@uhc.com", "chris_giordano@uhc.com", "MHPUniverse was refreshed", "MHPUniverse was refreshed", "chris_giordano@uhc.com;laura_fischer@uhc.com;jon_maguire@uhc.com", null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
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


            await SharedFunctions.EmailAsync("jon.piotrowski@uhc.com;renee_l_struck@uhc.com;hong_gao@uhc.com", "chris_giordano@uhc.com", "MHPUniverse was refreshed", "MHPUniverse was refreshed", "chris_giordano@uhc.com;laura_fischer@uhc.com;jon_maguire@uhc.com", null, System.Net.Mail.MailPriority.Normal).ConfigureAwait(false);
        }



        //ETG SYMM SOURCE AUTOMATION
        public async Task getETGSymmSourceDataAsync(float version)
        {
            //ETG DATA LOAD
            //ETG DATA LOAD
            //ETG DATA LOAD

            List<ETGVersion_Model> v = new List<ETGVersion_Model>();
            v.Add(new ETGVersion_Model() { PD_Version = 16, Year = 2022 });
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
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_NRX_AGG];INSERT INTO [etg].[ETG_Dataload_NRX_AGG] ([ETG_Base_Class] ,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX]) SELECT [ETG_Base_Class] ,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX] FROM [etg].[VW_ETG_Dataload_NRX_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);


            //STEP 8 [etg].[ETG_Dataload_EC_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_EC_AGG];INSERT INTO [etg].[ETG_Dataload_EC_AGG] ([Premium_Specialty] ,[ETG_Base_Class] ,[EC_Treatment_Indicator] ,[EC_Episode_Count] ,[EC_Total_Cost] ,[EC_Average_Cost] ,[EC_Coefficients_of_Variation] ,[EC_Normalized_Pricing_Episode_Count] ,[EC_Normalized_Pricing_Total_Cost] ,[EC_Spec_Episode_Count] ,[EC_Spec_Total_Cost] ,[EC_Spec_Average_Cost] ,[EC_Spec_Coefficients_of_Variation] ,[EC_Spec_Percent_of_Episodes] ,[EC_Spec_Normalized_Pricing_Episode_Count] ,[EC_Spec_Normalized_Pricing_Total_Cost] ,[EC_CV3] ,[EC_Spec_Episode_Volume] ,[PD_Mapped]) SELECT [Premium_Specialty] ,[ETG_Base_Class] ,[EC_Treatment_Indicator] ,[EC_Episode_Count] ,[EC_Total_Cost] ,[EC_Average_Cost] ,[EC_Coefficients_of_Variation] ,[EC_Normalized_Pricing_Episode_Count] ,[EC_Normalized_Pricing_Total_Cost] ,[EC_Spec_Episode_Count] ,[EC_Spec_Total_Cost] ,[EC_Spec_Average_Cost] ,[EC_Spec_Coefficients_of_Variation] ,[EC_Spec_Percent_of_Episodes] ,[EC_Spec_Normalized_Pricing_Episode_Count] ,[EC_Spec_Normalized_Pricing_Total_Cost] ,[EC_CV3] ,[EC_Spec_Episode_Volume] ,[PD_Mapped] FROM [etg].[VW_ETG_Dataload_EC_AGG;";
            await db_sql.Execute(ConnectionStringVC, strSQL);


            //STEP 9 [etg].[ETG_Dataload_PC_AGG] CACHE
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_PC_AGG];INSERT INTO [etg].[ETG_Dataload_PC_AGG] ([Premium_Specialty] ,[ETG_Base_Class] ,[PC_Episode_Count] ,[PC_Total_Cost] ,[PC_Average_Cost] ,[PC_Coefficients_of_Variation] ,[PC_Normalized_Pricing_Episode_Count] ,[PC_Normalized_Pricing_Total_Cost] ,[PC_Spec_Episode_Count] ,[PC_Spec_Total_Cost] ,[PC_Spec_Average_Cost] ,[PC_Spec_CV] ,[PC_Spec_Percent_of_Episodes] ,[PC_Spec_Normalized_Pricing_Episode_Count] ,[PC_Spec_Normalized_Pricing_Total_Cost] ,[PC_CV3] ,[PC_Spec_Epsd_Volume]) SELECT [Premium_Specialty] ,[ETG_Base_Class] ,[PC_Episode_Count] ,[PC_Total_Cost] ,[PC_Average_Cost] ,[PC_Coefficients_of_Variation] ,[PC_Normalized_Pricing_Episode_Count] ,[PC_Normalized_Pricing_Total_Cost] ,[PC_Spec_Episode_Count] ,[PC_Spec_Total_Cost] ,[PC_Spec_Average_Cost] ,[PC_Spec_CV] ,[PC_Spec_Percent_of_Episodes] ,[PC_Spec_Normalized_Pricing_Episode_Count] ,[PC_Spec_Normalized_Pricing_Total_Cost] ,[PC_CV3] ,[PC_Spec_Epsd_Volume] FROM [etg].[VW_ETG_Dataload_PC_AGG];";
            await db_sql.Execute(ConnectionStringVC, strSQL);





        }






        public async Task getETGSymmSourceDataOriginalAsync(Int16 year  = 2022)
        {
            //ETG DATA LOAD
            //ETG DATA LOAD
            //ETG DATA LOAD

            IRelationalDataAccess db_sql = new SqlDataAccess();
            IRelationalDataAccess db_td = new TeraDataAccess();

            //STEP 1 etg.NRX_Cost_UGAP_SOURCE
            string strSQL = "select ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD, Count(Distinct ETG_D.INDV_SYS_ID) as MEMBER_COUNT, Count(Distinct ETG_D.EPSD_NBR) as EPSD_COUNT, Sum(ETG_D.TOT_ALLW_AMT) as ETGD_TOT_ALLW_AMT, Sum(ETG_D.RX_ALLW_AMT) as ETGD_RX_ALLW_AMT, case when Sum(ETG_D.TOT_ALLW_AMT) = 0 then 0 else NVL(Sum(ETG_D.RX_ALLW_AMT), 0) / Sum(ETG_D.TOT_ALLW_AMT) end as RX_RATE from ( select ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND as TRT_CD, Sum(ED1.QLTY_INCNT_RDUC_AMT) as TOT_ALLW_AMT, Query1.RX_ALLW_AMT from CLODM001.ETG_DETAIL ED1 inner join CLODM001.ETG_NUMBER EN1 on ED1.ETG_SYS_ID = EN1.ETG_SYS_ID inner join CLODM001.DATE_FST_SRVC DFS1 on ED1.FST_SRVC_DT_SYS_ID = DFS1.FST_SRVC_DT_SYS_ID inner join ( select C.INDV_SYS_ID from ( select B.INDV_SYS_ID, Min(B.PHRM_BEN_FLG) as MIN_PHARMACY_FLG, Sum(B.NUM_DAY) as NUM_DAY from ( select a.INDV_SYS_ID, ( case when a.END_DT > '"+ year + "-12-31' then Cast('"+ year + "-12-31' as Date) else a.END_DT end - case when a.EFF_DT < '"+ year + "-01-01' then Cast('"+ year + "-01-01' as Date) else a.EFF_DT end) + 1 as NUM_DAY, a.PHRM_BEN_FLG from CLODM001.MEMBER_DETAIL_INPUT a where a.EFF_DT <= '"+ year + "-12-31' and a.END_DT >= '"+ year + "-01-01') as B group by B.INDV_SYS_ID ) C where C.MIN_PHARMACY_FLG = 'Y' and C.NUM_DAY >= 210 ) as MT on ED1.INDV_SYS_ID = MT.INDV_SYS_ID left join ( select ED2.INDV_SYS_ID, ED2.EPSD_NBR, Sum(ED2.QLTY_INCNT_RDUC_AMT) as RX_ALLW_AMT from CLODM001.ETG_DETAIL ED2 inner join CLODM001.DATE_FST_SRVC DFS2 on ED2.FST_SRVC_DT_SYS_ID = DFS2.FST_SRVC_DT_SYS_ID inner join CLODM001.HP_SERVICE_TYPE_CODE HSTC2 on ED2.HLTH_PLN_SRVC_TYP_CD_SYS_ID = HSTC2.HLTH_PLN_SRVC_TYP_CD_SYS_ID where DFS2.FST_SRVC_DT Between '"+ year + "-01-01'and '"+ year + "-12-31'  and ED2.QLTY_INCNT_RDUC_AMT > 0 and HSTC2.HLTH_PLN_SRVC_TYP_LVL_1_NM = 'PHARMACY' group by ED2.INDV_SYS_ID, ED2.EPSD_NBR ) Query1 on ED1.INDV_SYS_ID = Query1.INDV_SYS_ID and ED1.EPSD_NBR = Query1.EPSD_NBR where ED1.EPSD_NBR not in (0, -1) and DFS1.FST_SRVC_DT Between '"+ year + "-01-01' and '"+ year + "-12-31' and ED1.QLTY_INCNT_RDUC_AMT > 0 group by ED1.INDV_SYS_ID, ED1.EPSD_NBR, EN1.ETG_BAS_CLSS_NBR, EN1.ETG_TX_IND, Query1.RX_ALLW_AMT ) as ETG_D group by ETG_D.ETG_BAS_CLSS_NBR, ETG_D.TRT_CD";

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
            strSQL = "TRUNCATE TABLE [etg].[ETG_Dataload_NRX_AGG];INSERT INTO [etg].[ETG_Dataload_NRX_AGG] ([ETG_Base_Class] ,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX]) SELECT [ETG_Base_Class] ,[RX_NRX] ,[Has_RX] ,[Has_NRX] ,[RX_RATE] ,[RX] ,[NRX] FROM [etg].[VW_ETG_Dataload_NRX_AGG];";
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

            string[]  columns = typeof(DQC_DATA_EBM_UHPD_SOURCE_Model).GetProperties().Select(p => p.Name).ToArray();
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
            var connectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
            var tdConnectionString = "Data Source=UDWPROD;User ID=cgiorda;Password=BooWooDooFoo2023!!;Authentication Mechanism=LDAP;Session Mode=TERADATA;Session Character Set=ASCII;Persist Security Info=true;Connection Timeout=99999;";
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
                        strSQL = CommonFunctions.getCreateTmpTableScript("stg", tmp_table, strLstColumnNames);
                        await db_sql.Execute(connectionString: connectionString, strSQL);

                        strSQL = "SELECT * FROM [stg].[" + tmp_table + "]; ";
                        //CREATE TMP TABLE AND COLLECT NEW DB TABLE FOR BULK TRANSFERS
                        dtTransfer = await db_sql.LoadDataTable(connectionString, strSQL);
                        dtTransfer.TableName = "stg." + tmp_table;

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
                        await db_sql.BulkSave(connectionString: connectionString, dtTransfer);
                        dtTransfer.Rows.Clear();
                    }


                }

                //CATCH REST OF UPLOADS OUTSIDE CSV LOOP
                if (dtTransfer.Rows.Count > 0)
                    await db_sql.BulkSave(connectionString: connectionString, dtTransfer);



                strSQL = CommonFunctions.getTableAnalysisScript("stg", tmp_table, strLstColumnNames);
                var dataTypes = (await db_sql.LoadData<DataTypeModel>(connectionString: connectionString, strSQL));

                strSQL = CommonFunctions.getCreateFinalTableScript("stg", table, dataTypes);
                await db_sql.Execute(connectionString: connectionString, strSQL);

                strSQL = CommonFunctions.getSelectInsertScript("stg", tmp_table, table, strLstColumnNames);
                await db_sql.Execute(connectionString: connectionString, strSQL);

                strLstColumnNames = null;
            }

            //2 GENERTATE FINAL OUTPUT
            strSQL = "Select distinct ETG_BAS_CLSS_NBR, MPC_NBR from CLODM001.ETG_NUMBER";
            var mcp = await db_td.LoadData<UGAPMPCNBRModel>(connectionString: tdConnectionString, strSQL);



            strSQL = "SELECT [MPC_NBR] ,[ETG_BAS_CLSS_NBR] ,[ALWAYS] ,[ATTRIBUTED] ,[ERG_SPCL_CATGY_CD] ,[TRT_CD] ,[RX] ,[NRX] ,[RISK_Model] ,[LOW_MONTH] ,[HIGH_MONTH] FROM [IL_UCA].[dbo].[VW_UGAPCFG_FINAL]";

            var etg = await db_sql.LoadData<UGAPETGModel>(connectionString: connectionString, strSQL);


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


        public async Task generatePEGReportsAsync()
        {
 
            List<ExcelExport> export = new List<ExcelExport>();

            var closed_xml = new ClosedXMLFunctions();


            IRelationalDataAccess db_sql = new SqlDataAccess();
            string strSheetName = "PEG";
            string strSQL = "select Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var peg = await db_sql.LoadData<PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = peg.ToList<object>(), SheetName = strSheetName });




            strSheetName = "PEG Subcat";
            strSQL = "select Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ',VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var pegsub = await db_sql.LoadData<PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = pegsub.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end";
            var lob = await db_sql.LoadData<LOB_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lob.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Specialty";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD";
            var spec = await db_sql.LoadData<Specialty_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = spec.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Quality Metric";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var qm = await db_sql.LoadData<Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qm.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Region";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var r = await db_sql.LoadData<Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = r.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Major Market";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var maj = await db_sql.LoadData<Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = maj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Minor Market";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var min = await db_sql.LoadData<Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = min.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB by PEG";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lp = await db_sql.LoadData<LOB_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lp.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB by PEG Subcat";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lps = await db_sql.LoadData<LOB_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lps.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Specialty by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var sp = await db_sql.LoadData<Specialty_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by PEG Subcat";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var sps = await db_sql.LoadData<Spec_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sps.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Quality Metric by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmp = await db_sql.LoadData<Quality_Metric_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmp.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Qual Metric by PEG Subcat";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmps = await db_sql.LoadData<Quality_Metric_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmps.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Spec";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD";
            var ls = await db_sql.LoadData<LOB_Spec_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = ls.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Qual Metric";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var lqm = await db_sql.LoadData<LOB_Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqm.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Region";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var lr = await db_sql.LoadData<LOB_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lr.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Major Mkt";
            strSQL = "SELECT case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant FROM VCT_DB.peg.VW_PEG_Final GROUP BY case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var lmaj = await db_sql.LoadData<LOB_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmaj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Minor Mkt";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var lmin = await db_sql.LoadData<LOB_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmin.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Spec by Quality Metric";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var sqm = await db_sql.LoadData<Spec_Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqm.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Region";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var sr = await db_sql.LoadData<Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sr.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Major Mkt";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var smaj = await db_sql.LoadData<Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smaj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Minor Mkt";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var smin = await db_sql.LoadData<Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smin.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Qual Metric by Region";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var qmr = await db_sql.LoadData<Quality_Metric_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmr.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Qual Metric by Major Mkt";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var qmMaj = await db_sql.LoadData<Quality_Metric_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmMaj.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Qual Metric by Minor Mkt";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var qmMin = await db_sql.LoadData<Quality_Metric_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmMin.ToList<object>(), SheetName = strSheetName });




            strSheetName = "LOB by Qual Metric by PEG";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lqmp = await db_sql.LoadData<LOB_Quality_Metric_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Qual Metric by PEG Subc";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lqmsc = await db_sql.LoadData<LOB_Quality_Metric_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmsc.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Qual Metric by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var sqmp = await db_sql.LoadData<Spec_Quality_Metric_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Qual Metric by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var sqmps = await db_sql.LoadData<Spec_Quality_Metric_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmps.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Region by PEG";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lrp = await db_sql.LoadData<LOB_Region_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lrp.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Region by PEG Subc";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lrpsc = await db_sql.LoadData<LOB_Region_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lrpsc.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Maj Mkt by PEG";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lmajp = await db_sql.LoadData<LOB_Major_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmajp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Maj Mkt by PEG Subc";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lmajps = await db_sql.LoadData<LOB_Major_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmajps.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Min Mkt by PEG";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var lminp = await db_sql.LoadData<LOB_Minor_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lminp.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Min Mkt by PEG Subc";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var lminps = await db_sql.LoadData<LOB_Minor_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lminps.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Spec by Region";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var lsr = await db_sql.LoadData<LOB_Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsr.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Spec by Maj Mkt";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var lsmaj = await db_sql.LoadData<LOB_Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmaj.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Spec by Min Mkt";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var lsmin = await db_sql.LoadData<LOB_Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmin.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Qual Metric by Region";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var lqmr = await db_sql.LoadData<LOB_Quality_Metric_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmr.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Qual Metric by Maj Mkt";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var lqmmaj = await db_sql.LoadData<LOB_Quality_Metric_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmmaj.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Qual Metric by Min Mkt";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var lqmin = await db_sql.LoadData<LOB_Quality_Metric_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lqmin.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Spec by Qual Metric";
            strSQL = "select case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end as LOB, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by case when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 1 then 'COMMERCIAL' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 2 then 'MEDICARE' when VCT_DB.peg.VW_PEG_Final.CNFG_POP_SYS_ID = 3 then 'MEDICAID' else '' end, VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM";
            var lsqm = await db_sql.LoadData<LOB_Spec_Quality_Metric_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsqm.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Spec by Region by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var srp = await db_sql.LoadData<Spec_Region_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = srp.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Spec by Region by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var srps = await db_sql.LoadData<Spec_Region_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = srps.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Spec by Major Mkt by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var smajp = await db_sql.LoadData<Spec_Major_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smajp.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Spec by Major Mkt by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var smajps = await db_sql.LoadData<Spec_Major_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smajps.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Minor Mkt by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var sminp = await db_sql.LoadData<Spec_Minor_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sminp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Minor Mkt by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var sminps = await db_sql.LoadData<Spec_Minor_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sminps.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Qual Metric by Region";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM";
            var sqmr = await db_sql.LoadData<Spec_Quality_Metric_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmr.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Spec by Qual Met by Major Mkt";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM";
            var sqmmaj = await db_sql.LoadData<Spec_Quality_Metric_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmmaj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Spec by Qual Met by Minor Mkt";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD as Specialty, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.PREM_SPCL_CD, VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM";
            var sqmmin = await db_sql.LoadData<Spec_Quality_Metric_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sqmmin.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Qual Met by Region by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmrp = await db_sql.LoadData<Quality_Metric_Region_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmrp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Qual Met by Region by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.RGN_NM as Region, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.RGN_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmrps = await db_sql.LoadData<Quality_Metric_Region_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmrps.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Qual Met by Major Mkt by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmmajp = await db_sql.LoadData<Quality_Metric_Major_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmmajp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Qual Met by Maj Mkt by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM as Major_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MAJ_MKT_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmmajps = await db_sql.LoadData<Quality_Metric_Major_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmmajps.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Qual Met by Minor Mkt by PEG";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC) as PEG, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_DESC)";
            var qmminp = await db_sql.LoadData<Quality_Metric_Minor_Market_PEG_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmminp.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Qual Met by Min Mkt by PEG Subc";
            strSQL = "select VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM as Quality_Metric, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM as Minor_Market, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC) as PEG_with_Subcategory, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Opportunity) as Previous_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.peg.VW_PEG_Final.Current_Market_Compliant) as Current_Compliant from VCT_DB.peg.VW_PEG_Final group by VCT_DB.peg.VW_PEG_Final.QLTY_MSR_NM, VCT_DB.peg.VW_PEG_Final.MKT_RLLP_NM, Concat(VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY_ID, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_CATGY, ': ', VCT_DB.peg.VW_PEG_Final.PEG_ANCH_SBCATGY_DESC)";
            var qmminps = await db_sql.LoadData<Quality_Metric_Minor_Market_PEG_Subcat_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = qmminps.ToList<object>(), SheetName = strSheetName });

            var bytes = await closed_xml.ExportToExcelTemplateAsync(PEGReportTemplatePath, export);


            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PEG_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


            if (File.Exists(file))
                File.Delete(file);

            await File.WriteAllBytesAsync(file, bytes);



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


            IRelationalDataAccess db_sql = new SqlDataAccess();
            string strSheetName = "Measure";
            string strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC)";
            var meas = await db_sql.LoadData<Measure_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = meas.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB";
            var lob = await db_sql.LoadData<LOB_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lob.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Specialty";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var spec = await db_sql.LoadData<Specialty_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = spec.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Region";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var reg = await db_sql.LoadData<Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = reg.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Major Market";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var maj = await db_sql.LoadData<Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = maj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Minor Market";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var min = await db_sql.LoadData<Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = min.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Measure by LOB";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB";
            var ml = await db_sql.LoadData<Measure_LOB_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = ml.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Measure by Specialty";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var msp = await db_sql.LoadData<Measure_Specialty_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msp.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Measure by Region";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var mr = await db_sql.LoadData<Measure_Region_Mode>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mr.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Measure by Major Mkt";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM As Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var mmaj = await db_sql.LoadData<Measure_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mmaj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Measure by Minor Mkt";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var mmin = await db_sql.LoadData<Measure_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mmin.ToList<object>(), SheetName = strSheetName });



            strSheetName = "LOB by Specialty";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var ls = await db_sql.LoadData<LOB_Spec_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = ls.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB by Region";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var lr = await db_sql.LoadData<LOB_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lr.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB by Major Mkt";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var lmaj = await db_sql.LoadData<LOB_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmaj.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB by Minor Mkt";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var lmin = await db_sql.LoadData<LOB_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lmin.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Specialty by Region";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var sr = await db_sql.LoadData<Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = sr.ToList<object>(), SheetName = strSheetName });




            strSheetName = "Specialty by Major Mkt";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var smaj = await db_sql.LoadData<Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smaj.ToList<object>(), SheetName = strSheetName });



            strSheetName = "Specialty by Minor Mkt";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var  smin = await db_sql.LoadData<Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = smin.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Measure by LOB by Specialty";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD";
            var mls = await db_sql.LoadData<Measure_LOB_Spec_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mls.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Measure by LOB by Region";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var mlr = await db_sql.LoadData<Measure_LOB_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mlr.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Measure by LOB by Major Mkt";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var mlmaj = await db_sql.LoadData<Measure_LOB_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mlmaj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "Measure by LOB by Minor Mkt";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var mlmin = await db_sql.LoadData<Measure_LOB_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = mlmin.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Measure by Specialty by Region";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var msr = await db_sql.LoadData<Measure_Specialty_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msr.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Measure by Specialty by Maj Mkt";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var msmaj = await db_sql.LoadData<Measure_Specialty_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msmaj.ToList<object>(), SheetName = strSheetName });

            strSheetName = "Measure by Specialty by Min Mkt";
            strSQL = "select Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC) as Measure, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by Concat(VCT_DB.ebm.VW_EBM_Final.REPORT_CASE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.REPORT_RULE_ID, ': ', VCT_DB.ebm.VW_EBM_Final.COND_NM, ': ', VCT_DB.ebm.VW_EBM_Final.RULE_DESC), VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var msmin = await db_sql.LoadData<Measure_Specialty_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = msmin.ToList<object>(), SheetName = strSheetName });

            strSheetName = "LOB by Specialty by Region";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.RGN_NM as Region, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.RGN_NM";
            var lsr = await db_sql.LoadData<LOB_Spec_Region_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsr.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Specialty by Maj Mkt";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM as Major_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MAJ_MKT_NM";
            var lsmaj = await db_sql.LoadData<LOB_Spec_Major_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmaj.ToList<object>(), SheetName = strSheetName });


            strSheetName = "LOB by Specialty by Min Mkt";
            strSQL = "select VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD as Specialty, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM as Minor_Market, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Compliant) as Current_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Current_Market_Opportunity) as Current_Opportunity, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Compliant) as Previous_Compliant, Sum(VCT_DB.ebm.VW_EBM_Final.Previous_Market_Opportunity) as Previous_Opportunity from VCT_DB.ebm.VW_EBM_Final group by VCT_DB.ebm.VW_EBM_Final.LOB, VCT_DB.ebm.VW_EBM_Final.PREM_SPCL_CD, VCT_DB.ebm.VW_EBM_Final.MKT_RLLP_NM";
            var lsmin = await db_sql.LoadData<LOB_Spec_Minor_Market_Model>(connectionString: ConnectionStringVC, strSQL);
            export.Add(new ExcelExport() { ExportList = lsmin.ToList<object>(), SheetName = strSheetName });



            var bytes = await closed_xml.ExportToExcelTemplateAsync(EBMReportTemplatePath, export);


            var file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EBM_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";


            if (File.Exists(file))
                File.Delete(file);

            await File.WriteAllBytesAsync(file, bytes);



            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file)
            {
                UseShellExecute = true
            };
            p.Start();


        }


    }




}
