using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintDataTransfer
{
    class ComplaintDataTransfer
    {
        static void Main(string[] args)
        {
            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];

            IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];


            StringBuilder sbSQL = new StringBuilder();


            string strPDVersion = "13";

            IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


            sbSQL.Append("proc sql;");
            sbSQL.Append("create table pd_incl as SELECT PREM_SPCL_CD ,PREM_DESG_VER_NBR ,NDB_SPCL_TYP_CD FROM PDPRODC.CNFG_PREM_SPCL_MAP where PREM_DESG_VER_NBR = "  + strPDVersion + " and PREM_SPCL_CD not in('INTMD', 'PEDS', 'FAMED'); ");

            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);
            



            /*to get all sub specialties from UHN reporting DB*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table prov_spec as SELECT distinct p.MPIN ,p.LastName ,p.FirstName ,p.ProvDegree ,p.provtype ,spec_id ,CommercialInd ,COSMOSIND ,p.primspec as SpecTypeCd ,st.LongDesc ,s.Spec_Desc as NDB_Specialty ,ATTR_SPCL_CATGY_SYS_ID as SPCL_CATGY_SYS_ID FROM UHN.PROVIDER as p inner join pd_incl as ps on p.primspec = NDB_SPCL_TYP_CD INNER JOIN IL_UCA.PBP_dim_Spec as s on s.PREM_SPCL_CD = ps.PREM_SPCL_CD INNER JOIN UHN.SPECIALTY_TYPES as ST ON st.SpecTypeCd = p.primspec;");
            sbSQL.Append("quit;");
            /*340,601*/

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);


            /*to create a table for SQL Server*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table smr as select PEG_EPSD_NBR, PEG_ANCH_CATGY_ID as surg_id, INDV_SYS_ID, case when population = 'COMMERCIAL' then 1 when population = 'MEDICARE' then 2 else 3 end as lob_id, PROV_MPIN, Prov_Name, UNET_MKT_NBR, MKT_RLLP_NM as UNET_MKT_NM, TOT_PEG_ALLW_AMT, MGT_ALLW_AMT, SURG_ALLW_AMT, FACL_ALLW_AMT, PHRM_ALLW_AMT, IPTNT_ALLW_AMT, OPTNT_ALLW_AMT, PEG_ANCH_DT, PEG_ANCH_YR, SVRTY_LVL, SVRTY_SCOR, PROC_CD, PROC_DESC, DIAG1, DX1, AHRQ_DIAG_DTL_CATGY_CD, QLTY_PEG_ANCH_CATGY_SYS_ID, ETG_DESC, claim_tin, AMA_PL_OF_SRVC_CD, AMA_PL_OF_SRVC_DESC, s.HCCC_CD, s.HCCC_DESC, SRVC_LOC, s.Svrty, SEPSIS, PNEUM, UTI, CLINFC, PE, AIREMB, DVT, MI, ARF, PULM, FB, HEM, IA, PERF, UlC, CVA, SSI, TRANS, WOUND, CMPLCTN_IND, adm_idx, adm_QLTY_PEG_EPSD_NBR, admits_num, min_anch_adm_diff, Total_adm_allw, adm_match_type, ed_idx, ed_QLTY_PEG_EPSD_NBR, eds_num, min_anch_ed_diff, ed_match_type, com_adm, adm, ed, com, b.spec_id, fac_type, Facl_TIN, TAXID_name as Fac_TIN_Name from postopms.peg_cmplctnq as a inner join postopms.POS as s on a.PEG_EPSD_NBR = anch_PEG_EPSD_NBR inner join prov_spec as d on a.prov_mpin = d.mpin inner join IL_UCA.dim_peg_spec as b on a.peg_anch_catgy = b.peg_anch_catgy and d.spec_id = b.spec_id where a.Svrty <> '0';");
            /*3,365,605*/

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);


            /*to add Practice TIN NAME*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table tin_name_op as select distinct d.claim_tin, input(d.claim_tin, 9.) as claim_tin_num,t.tin_name from smr as d left join(select taxid, max(corpownername) as tin_name from UHN.taxid group by taxid) as t on input(d.claim_tin,9.)= t.taxid where claim_tin not in('-1', '0');");
            /*25,653*/

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);


            /*to add practice name*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table smr as select a.*, p.tin_name as Pract_Name, claim_tin_num from smr as a left join tin_name_op as p on p.claim_tin = a.claim_tin;");

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);

            /*Benchmark by lob,srvc_loc,svrty,spec_id,surg_id*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table bmk_postOp as select lob_id,srvc_loc,svrty,spec_id,surg_id,327 as measure_id, count(*) as denom, sum(com_adm) as num, calculated denom-calculated num as not_adv, calculated num/ calculated denom as rate from smr as c group by lob_id,srvc_loc,svrty,spec_id,surg_id having calculated num<>0 UNION select lob_id,srvc_loc,svrty,spec_id,surg_id,325 as measure_id, count(*) as denom, sum(adm) as num, calculated denom-calculated num as not_adv, calculated num/ calculated denom as rate from smr as c group by lob_id,srvc_loc,svrty,spec_id,surg_id having calculated num<>0 UNION select lob_id,srvc_loc,svrty,spec_id,surg_id,326 as measure_id, count(*) as denom, sum(ed) as num, calculated denom-calculated num as not_adv, calculated num/ calculated denom as rate from smr as c group by lob_id,srvc_loc,svrty,spec_id,surg_id having calculated num<>0 UNION select lob_id,srvc_loc,svrty,spec_id,surg_id,324 as measure_id, count(*) as denom, sum(com) as num, calculated denom-calculated num as not_adv, calculated num/ calculated denom as rate from smr as c group by lob_id,srvc_loc,svrty,spec_id,surg_id having calculated num<>0;");
            sbSQL.Append("quit;");
            /*1306*/

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);

            /*to add expected numbers by line*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table bmk_postOp_trsp as select distinct lob_id, srvc_loc, svrty, spec_id, surg_id, max(case when measure_id = 327 then rate else 0 end) as com_adm_rate ,max(case when measure_id = 325 then rate else 0 end) as adm_rate ,max(case when measure_id = 326 then rate else 0 end) as ed_rate ,max(case when measure_id = 324 then rate else 0 end) as com_rate from bmk_postOp group by lob_id,srvc_loc,svrty,spec_id,surg_id;");

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);

            /* some rates are 0*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table smr as select a.*, b.com_adm_rate as exp_com_adm, b.adm_rate as exp_adm,b.ed_rate as exp_ed, b.com_rate as exp_com from smr as a inner join bmk_postOp_trsp as b on a.lob_id = b.lob_id and a.spec_id = b.spec_id and a.srvc_loc = b.srvc_loc and a.svrty = b.svrty and a.surg_id = b.surg_id;");

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);

            /*933022*/
            /*to add lst_run_qtr and PD status*/
            /*to identify T1 PD physicians*/
            sbSQL.Append("proc sql;");
            sbSQL.Append("create table pd_stat as Select PREM_DESG_VER_NBR ,MPIN ,DSPL_DESC_CD ,REAL_DSPL_DESC_CD ,PREM_SPCL_CD ,PREM_SUB_SPCL_CD From PDPRODC.DESG_DSCLOS_INFO where CNFG_POP_SYS_ID = 1/*Commercial*/ and lst_cyc_id in(select max(lst_cyc_id) from PDPRODC.DESG_DSCLOS_INFO where PREM_DESG_VER_NBR = "  + strPDVersion + ");");
            sbSQL.Append("quit;"); /*421,130 */

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);

            sbSQL.Append("proc sql;");
            sbSQL.Append("create table smr as select s.*,lst_run_qrt, case when REAL_DSPL_DESC_CD is null then 'NotEvaluated' when REAL_DSPL_DESC_CD not in('QandE', 'Quality', 'NotDesignated') then 'Insufficient' else REAL_DSPL_DESC_CD end as pd_status from smr as s left join pd_stat as p on p.mpin = s.prov_mpin Inner join(select put(max(PEG_ANCH_YR),4.)|| '0' || put(qtr(max(PEG_ANCH_DT)), 1.) as lst_run_qrt from smr) as a on 1 = 1;");

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);

            sbSQL.Append("proc sql;");
            sbSQL.Append("create table IL_UCA.compl_app_csg_test as select* from smr;");
            //TRUNCATE IL_UCA.compl_app_stage
            //CHANGE TO INSTER INTO IL_UCA.compl_app_stage

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            Console.WriteLine(sbSQL.ToString());
            IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());
            Console.WriteLine(IR_SAS_Connect.strProcSQLResults);
            sbSQL.Remove(0, sbSQL.Length);


          //  IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString());

        

          //Console.WriteLine(IR_SAS_Connect.strProcSQLResults);




            //DataTable dtMain = DBConnection.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
            ////HADLE BULK INSERTS FOR CONSOLE FEEDBACK
            //dtMain.TableName = "qa_link_request_phy_sas_cache";
            //DBConnection.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE " + dtMain.TableName + ";");
            //DBConnection.SQLServerBulkImportDT(dtMain, strConnectionString);
            //DBConnection.getOleDbDataTableGlobalClose();
            IR_SAS_Connect.destroy_SAS_instance();


        }
    }
}
