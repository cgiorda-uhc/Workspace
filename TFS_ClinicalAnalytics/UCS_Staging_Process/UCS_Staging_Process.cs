using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_Staging_Process
{
    class UCS_Staging_Process
    {
        static void Main(string[] args)
        {
            /*DBConnection.getMSSQLDataTable(strConnectionString, "truncate table UCS_DIM_DATA_SOURCE;
                truncate table UCS_DIM_PROJECT;
                truncate table UCS_DIM_PHASE;
                truncate table UCS_DIM_NAME;
                truncate table UCS_DIM_ADDRESS;
                truncate table UCS_DIM_MARKET;
                truncate table UCS_DIM_SPECIALTY;
                truncate table UCS_DIM_PROVIDER_TYPE;
                truncate table UCS_DIM_CREDENTIAL;
                truncate table UCS_DIM_MEMBER;
                truncate table UCS_FACT_PROVIDER_DEMOGRAPHICS; UPDATE dbo.UCS_STAGING_PROV_DEM SET stg_action_completed = NULL;
                truncate table UCS_FACT_MEMBER_ATTRIBUTION;UPDATE dbo.UCS_STAGING_MEM_ATT SET stg_action_completed = NULL;
                truncate table dbo.UCS_DIM_APRDRG;UPDATE dbo.UCS_STAGING_APRDRG SET stg_action_completed = NULL;
                truncate table dbo.UCS_DIM_DIAGNOSIS;UPDATE dbo.UCS_STAGING_DIAGNOSIS SET stg_action_completed = NULL;
                truncate table dbo.UCS_DIM_MEASURE;UPDATE dbo.UCS_STAGING_MEASURE SET stg_action_completed = NULL;
                truncate table dbo.UCS_DIM_PROCEDURE;UPDATE dbo.UCS_STAGING_PROCEDURE SET stg_action_completed = NULL;
                truncate table UCS_FACT_ALL_MEASURE_SUMMARY; UPDATE dbo.UCS_STAGING_ALL_MEASURE_SUMMARY SET stg_action_completed = NULL;");*/


            ////INSERT INTO [UCS_STAGING_PROV_DEM] ( [project_name] ,[phase_name] ,[indv_prov_mpin] ,[indv_prov_tin] ,[indv_prov_first_name] ,[indv_prov_middle_name] ,[indv_prov_last_name] ,[indv_prov_last_name_cleaned_tmp] ,[indv_prov_first_name_cleaned_tmp] ,[indv_prov_street] ,[indv_prov_city] ,[indv_prov_state] ,[indv_prov_zipcode] ,[indv_prov_address_type] ,[indv_prov_mkt_nbr] ,[indv_prov_mkt_nm] ,[indv_prov_maj_mkt_nm] ,[indv_prov_mkt_rllp_nm] ,[indv_prov_rgn_nm] ,[indv_prov_phone] ,[indv_prov_type] ,[indv_prov_specialty_description] ,[indv_prov_commercial_indicator] ,[indv_prov_degree] ,[grp_prov_mpin] ,[grp_prov_tin] ,[grp_prov_first_name] ,[grp_prov_last_name] ,[grp_prov_first_name_cleaned_tmp] ,[grp_prov_last_name_cleaned_tmp] ,[grp_prov_street] ,[grp_prov_city] ,[grp_prov_state] ,[grp_prov_zipcode] ,[group_phone] ,[group_prov_type] ,[corp_owner_name] ,[corp_owner_mpin] ,[corp_owner_prov_type] ,[efficiency_geo_number] ,[unilateral] ,[hierarchy_id] ,[is_outlier] ,[data_source] ,[update_ucs] ) SELECT project_name, phase_name, indv_prov_mpin, indv_prov_tin, indv_prov_first_name, indv_prov_middle_name, indv_prov_last_name, indv_prov_last_name_cleaned_tmp, indv_prov_first_name_cleaned_tmp, indv_prov_street , indv_prov_city, indv_prov_state, indv_prov_zipcode, indv_prov_address_type, indv_prov_mkt_nbr, indv_prov_mkt_nm, indv_prov_maj_mkt_nm, indv_prov_mkt_rllp_nm, indv_prov_rgn_nm, indv_prov_phone, indv_prov_type, indv_prov_specialty_description, indv_prov_commercial_indicator, indv_prov_degree, grp_prov_mpin, grp_prov_tin, grp_prov_first_name, grp_prov_last_name, grp_prov_first_name_cleaned_tmp, grp_prov_last_name_cleaned_tmp, grp_prov_street, grp_prov_city, grp_prov_state, grp_prov_zipcode, group_phone, group_prov_type, corp_owner_name, corp_owner_mpin, corp_owner_prov_type , efficiency_geo_number, unilateral, hierarchy_id, is_outlier, data_source, update_ucs FROM [dbo].[VW_UCS_HISTORY_TMP_PROV_DEM] where phase_name = 'PH13'



            //INSERT INTO [IL_UCA].[dbo].[UCS_STAGING_APRDRG] ([apr_drg_cd] ,[apr_drg_desc] ,[apr_drg_sys_id] ,[dses_st_nm] ,[sens] ,[sens_ob] ,[data_source] ,[update_ucs]) SELECT APR_DRG_CD, APR_DRG_DESC, APR_DRG_SYS_ID, DSES_ST_NM, Sens, Sens_OB, 'IL_UCA_HISTORY' data_source, 'Y' as update_ucs FROM VW_UCS_HISTORY_TMP_APRDRG


            //INSERT INTO [IL_UCA].[dbo].[UCS_STAGING_DIAGNOSIS] ([diag_cd_sys_id] ,[diag_decm_cd] ,[diag_desc] ,[diag_cd] ,[ahrq_diag_dtl_catgy_nm] ,[ahrq_diag_dtl_catgy_desc] ,[ahrq_diag_dtl_catgy_cd] ,[ahrq_diag_catgy] ,[sens] ,[sens_ob] ,[data_source] ,[update_ucs]) SELECT [DIAG_CD_SYS_ID] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[DIAG_CD] ,[AHRQ_DIAG_DTL_CATGY_NM] ,NULL as ahrq_diag_dtl_catgy_desc ,[AHRQ_DIAG_DTL_CATGY_CD] ,[AHRQ_Diagnosis_Category] ,[Sens] ,[Sens_OB] , 'IL_UCA_HISTORY' data_source, 'Y' as update_ucs FROM [IL_UCA].[dbo].[VW_UCS_HISTORY_TMP_DIAGNOSIS]


            //INSERT INTO [IL_UCA].[dbo].[UCS_STAGING_MEASURE] ([measure_id] ,[measure_column] ,[measure_description] ,[unit_measure] ,[unit_measure_add] ,[measure_type] ,[data_source] ,[update_ucs]) SELECT Measure_ID ,[measure_column] ,[measure_desc] ,[unit_measure] ,[unit_measure_add] ,[measure_type] , 'IL_UCA_HISTORY' data_source, 'Y' as update_ucs FROM [IL_UCA].[dbo].[VW_UCS_HISTORY_TMP_MEASURES]

            // INSERT INTO[IL_UCA].[dbo].[UCS_STAGING_PROCEDURE]([proc_cd] ,[proc_desc] ,[ahrq_proc_gen_catgy_desc] ,[ahrq_proc_genl_catgy_cd] ,[ahrq_proc_dtl_catgy_desc] ,[ahrq_proc_dtl_catgy_cd] ,[proc_cd_sys_id] ,[proc_typ_cd] ,[sens] ,[sens_ob] ,[data_source] ,[update_ucs]) SELECT[PROC_CD] ,[PROC_DESC] ,[AHRQ_PROC_GENL_CATGY_DESC] ,[AHRQ_PROC_GENL_CATGY_CD] ,[AHRQ_PROC_DTL_CATGY_DESC] ,[AHRQ_PROC_DTL_CATGY_CD] ,[PROC_CD_SYS_ID] ,[PROC_TYP_CD] ,[Sens] ,[Sens_OB] , 'IL_UCA_HISTORY' data_source, 'Y' as update_ucs FROM[IL_UCA].[dbo].[VW_UCS_HISTORY_TMP_PROCEDURES]



            // INSERT INTO dbo.UCS_STAGING_IP_DETAILS( [project_name] , [phase_name] , [indv_sys_id] ,[indv_prov_mpin],  [hlth_pln_cnfn_id] , [apr_drg_cd] , [apr_drg_desc] , [apr_drg_sys_id] , [dses_st_nm], [sens], [sens_ob] , [svrty_lvl_cd], [admit_dt], [dschrg_dt], [srvc_mpin], [srvc_prov_full_name], [srvc_prov_type], [clm_aud_nbr], [fi_allw_amt], [fi_net_pd_amt], [deriv_allw_amt], [net_pd_amt], [admis_cnt], [stat_day], hlth_pln_fund_cd, [rp_risk_catgy], [data_source], [update_ucs]) select project_name, phase_name, INDV_SYS_ID,[individual mpin], HLTH_PLN_CNFN_ID, APR_DRG_CD, APR_DRG_DESC, APR_DRG_SYS_ID, DSES_ST_NM,  Sens, Sens_OB, SVRTY_LVL_CD, ADMIT_DT, DSCHRG_DT, [srvc_prov_mpin], [srvc_prov_last_name], srvc_prov_type, CLM_AUD_NBR, FI_Allw_AMT, FI_NET_PD_AMT, DERIV_ALLW_AMT, NET_PD_AMT, ADMIS_CNT, STAT_DAY, HLTH_PLN_FUND_CD, RP_RISK_CATGY, 'IL_UCA_HISTORY' as data_source, 'Y' as update_ucs FROM dbo.VW_UCS_HISTORY_TMP_IP_DETAIL WHERE project_name = 'PCR' AND phase_name = 'PH13'


            //INSERT INTO [IL_UCA].[dbo].[UCS_STAGING_ALL_MEASURE_SUMMARY] ([project_name] ,[phase_name] ,[indv_prov_mpin] ,[measure_identifier] ,[actual] ,[expected] ,[oe_ratio] ,[variance] ,[actual_display] ,[expected_display] ,[variance_display] ,[outlier_index] ,[outlier_index_g] ,[sort_id] ,data_source ,[update_ucs]) SELECT [project_name] ,[phase_name] ,[individual_mpin] ,[Measure_ID] ,[act] ,[expected] ,[OE_ratio] ,[variance] ,[act_display] ,[expected_display] ,[var_display] ,[Outl_idx] ,[Outl_idx_g] ,[sort_id] ,'IL_UCA_HISTORY' data_source ,'Y' as update_ucs FROM [IL_UCA].[dbo].[VW_UCS_HISTORY_TMP_ALL_MEASURE_SUMMARY] WHERE [project_name] = 'PCR' AND [phase_name] = 'PH13'



            //INSERT INTO [IL_UCA].[dbo].[UCS_STAGING_ALL_MEASURE_ACTIONABLE] ([project_name] ,[phase_name] ,[indv_prov_mpin] ,[measure_identifier] ,category_name ,[patient_count] ,[visit_count] ,[percentage_cost] ,[amount] ,[total_amount] ,[sort_id] ,[data_source] ,[stg_action] ,[stg_action_completed]) SELECT [project_name] ,[phase_name] ,[attr_MPIN] ,[Measure_ID] ,[Category] ,[Patient_Count] ,[Visit_Count] ,[Pct_Cost] ,[Amt] ,[tot_Amt] ,[sort_order] ,'IL_UCA_HISTORY' as data_source ,'I' as [stg_action] , NULL as [stg_action_completed] FROM [IL_UCA].[dbo].[VW_UCS_HISTORY_TMP_ALL_MEASURE_ACTIONABLE] WHERE [project_name] = 'PCR' AND [phase_name] = 'PH13'


            string strConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            Hashtable htMain = new Hashtable();
            DataTable dtError = new DataTable();

            DateTime startTime;
            DateTime endTime;
            TimeSpan span;

            string strSelectedProjectName = "PCR";
            string strSelectedPhaseName = "PH13";

            try
            {


                Console.WriteLine("-------------------APRDRG-DIAGNOSIS-MEASURE-PROCEDURE----------------------------");

                htMain.Clear();
                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_APRDRG...");
                dtError  = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_APRDRG", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_DIAGNOSIS...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_DIAGNOSIS", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_MEASURE...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_MEASURE", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_PROCEDURE...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_PROCEDURE", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();



                Console.WriteLine("--------------------VW_UCS_STAGE_TO_FACT_PROV_DEM-------------------------------");

                htMain.Clear();
                htMain.Add("@project_name", strSelectedProjectName);
                htMain.Add("@phase_name", strSelectedPhaseName);

                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_PROJECT...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_PROJECT", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_PHASE...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_PHASE", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_DATA_SOURCE...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_DATA_SOURCE", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_CREDENTIAL...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_CREDENTIAL", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_SPECIALTY...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_SPECIALTY", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_PROVIDER_TYPE...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_PROVIDER_TYPE", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_NAME...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_NAME", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_PHONE_NUMBER...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_PHONE_NUMBER", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_ADDRESS...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_ADDRESS", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_MARKET...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_MARKET", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_FACT_PROV_DEM...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_FACT_PROV_DEM", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();



                Console.WriteLine("---------------------VW_UCS_STAGE_TO_FACT_MEM_ATT-------------------------------");

                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_MEMBER...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_MEMBER", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_FACT_MEM_ATT...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_FACT_MEM_ATT", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();




                Console.WriteLine("--------------------VW_UCS_STAGE_TO_FACT_SUMMARY--------------------------------");

                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_FACT_SUMMARY...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_FACT_SUMMARY", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();




                Console.WriteLine("--------------------VW_UCS_STAGE_TO_FACT_ACTIONABLE--------------------------------");

                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_DIM_CATEGORY...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_DIM_CATEGORY", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();


                startTime = DateTime.Now;
                Console.WriteLine("START (" + startTime.ToString("h:mm:ss tt") + ") INSERT SP_UCS_INSERT_FACT_ACTIONABLE...");
                dtError = DBConnection64.getMSSQLDataTableSP(strConnectionString, "SP_UCS_INSERT_FACT_ACTIONABLE", htMain);
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
                Console.WriteLine("END   (" + endTime.ToString("h:mm:ss tt") + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
                if (dtError.Rows.Count > 0) throw new Exception();





            }
            catch (Exception ex)
            {

                Console.WriteLine("---------------------ERRROR ENCOUNTERED!!!-------------------------");
                if (dtError.Rows.Count > 0)
                {
                    Console.WriteLine("ERROR_NUMBER() = " + dtError.Rows[0]["ErrorNumber"]);
                    Console.WriteLine("ERROR_SEVERITY() = " + dtError.Rows[0]["ErrorSeverity"]);
                    Console.WriteLine("ERROR_STATE() = " + dtError.Rows[0]["ErrorState"]);
                    Console.WriteLine("ERROR_PROCEDURE() = " + dtError.Rows[0]["ErrorProcedure"]);
                    Console.WriteLine("ERROR_LINE() = " + dtError.Rows[0]["ErrorLine"]);
                    Console.WriteLine("ERROR_MESSAGE() = " + dtError.Rows[0]["ErrorMessage"]);
                }
                else
                {
                    Console.WriteLine("GENERAL ERROR MESSAGE = " + ex.ToString());
                }


                Console.Beep();
                Console.ReadLine();
            }
            finally
            {

            }

        }

            
    }
}
