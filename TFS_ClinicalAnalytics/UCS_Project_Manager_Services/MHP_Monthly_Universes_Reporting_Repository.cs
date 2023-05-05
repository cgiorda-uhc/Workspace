using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS_Project_Manager;
using UCS_Project_Manager_Services.Helpers;

namespace UCS_Project_Manager_Services
{
    public interface IMHP_Yearly_Universes_Reporting_Repository
    {

        Task<List<MHPCS_Yearly_Universes_Reporting_Model>> GetMHPCSDataAsync(string strState, string strStartDate, string strEndDate, string strCS_TADM_PRDCT_MAP_CSV,string strGroupNumbers, CancellationToken token);

        Task<List<MHPCS_Yearly_Universes_Details_Model>> GetMHPCSDetailsAsync(string strState, string strStartDate, string strEndDate, string strCS_TADM_PRDCT_MAP, string strGroupNumbers, CancellationToken token);

        Task<List<MHP_Yearly_Universes_Reporting_Model>> GetMHPDataAsync(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_CD, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, string strMKT_TYP_DESC, string strCUST_SEG, bool blIsIFP, CancellationToken token);
        List<MHP_Yearly_Universes_Reporting_Model> GetMHPData(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_CD, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities);

        Task<List<MHP_Yearly_Universes_Details_Model>> GetMHPDetailsAsync(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_CD, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, string strMKT_TYP_DESC, string strCUST_SEG, bool blIsIFP, CancellationToken token);


        Task<List<MHPIFP_Yearly_Universes_Details_Model>> GetMHPIFPDetailsAsync(string strState, string strStartDate, string strEndDate, List<string> lstProductCode, CancellationToken token);
        Task<List<MHPIFP_Yearly_Universes_Reporting_Model>> GetMHPIFPDataAsync(string strState, string strStartDate, string strEndDate, List<string> lstProductCode, CancellationToken token);


        List<string> GetLEG_ENTY(bool isCS = false);
        List<string> GetFINC_ARNG_DESC(bool isCS = false);
        List<string> GetMKT_SEG_RLLP_DESC(bool isCS = false);
        List<string> GetStates(bool isCS = false);

        List<string> GetMKT_TYP_DESC(bool isCS = false);

        List<string> GetCS_TADM_PRDCT_MAP(bool isCS = true);

        List<string> GetCUST_SEG(bool isCS = true);

        List<Group_State_Model> GetGroupState();

        List<string> GetProductCode();
    }


    public class MHP_Yearly_Universes_Reporting_Repository : IMHP_Yearly_Universes_Reporting_Repository
    {
        private _EFMainContextMHP _context = new _EFMainContextMHP();

        public MHP_Yearly_Universes_Reporting_Repository()
        {


        }

        public List<MHP_Yearly_Universes_Reporting_Model> GetMHPData(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_CD, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities)
        {

            StringBuilder sbSQL = new StringBuilder();

            string strWhere = null;
            string strExcelRow = null;

            foreach(string strLegalEntity in lstLegalEntities)
            {
                var legalNbr = strLegalEntity.Split('-')[0].Trim();
                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        case 0:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD') ";
                            strExcelRow = "4";
                            break;
                        case 1:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF')  AND [Decision_Reason] = 'Medically Necessary'   ";
                            strExcelRow = "5";
                            break;
                        case 2:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD') AND  [Decision_Reason]  <> 'Medically Necessary' AND [Decision_Reason] not like '054%' AND [Decision_Reason] not like '067%' ";
                            strExcelRow = "6";
                            break;
                        case 3:
                            strWhere = "AND [Authorization_Type]  in ( 'PS')  ";
                            strExcelRow = "56";
                            break;
                        case 4:
                            strWhere = "AND  [Authorization_Type]  in ('PS')  AND [Decision_Reason] = 'Medically Necessary'  ";
                            strExcelRow = "57";
                            break;
                        case 5:
                            strWhere = "AND [Authorization_Type]  in ('PS')  AND  [Decision_Reason] <> 'Medically Necessary' AND [Decision_Reason] not like '054%'  AND [Decision_Reason] not like '067%' ";
                            strExcelRow = "58";
                            break;
                        default:
                            break;
                    }

                    sbSQL.Append("SELECT ");
                    sbSQL.Append(strExcelRow + " as ExcelRow, ");//4 AND
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ( 'Site is PAR', 'Par') AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'Site is PAR' AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op, ");
                    sbSQL.Append("'" + strState.Replace("'", "") + "' as State , ");
                    sbSQL.Append("'" + strStartDate + "' as StartDate, ");
                    sbSQL.Append("'" + strEndDate + "' as EndDate, ");
                    sbSQL.Append("'" + strLegalEntity + "' as LegalEntity ");
                    sbSQL.Append("FROM( ");
                    sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                    sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                    sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                    sbSQL.Append("WHERE u.[State_of_Issue] = '" + strState + "' AND u.[Par_NonPar_Site] <> 'N/A' AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                    sbSQL.Append("AND c.[MKT_SEG_RLLP_DESC] in (" + strMKT_SEG_RLLP_DESC + ") AND  c.[FINC_ARNG_CD] in (" + strFINC_ARNG_CD + ")  AND [Authorization] IS NOT NULL "); //
                    sbSQL.Append("AND c.[LEG_ENTY_NBR] = '" + legalNbr + "' "); //
                    sbSQL.Append(strWhere);
                    sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                    sbSQL.Append(") tmp ");
                    sbSQL.Append("UNION ALL ");

                }


               

            }


            var list = _context.Database.SqlQuery<MHP_Yearly_Universes_Reporting_Model>(sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' ')).ToList<MHP_Yearly_Universes_Reporting_Model>();
            return list.OrderBy(o => o.LegalEntity).OrderBy(o => o.ExcelRow).ToList();

        }


        public async Task<List<MHPCS_Yearly_Universes_Reporting_Model>> GetMHPCSDataAsync(string strState, string strStartDate, string strEndDate, string strCS_TADM_PRDCT_MAP, string strGroupNumbers, CancellationToken token)
        {


            StringBuilder sbSQL = new StringBuilder();

            string strWhere = null;
            string strExcelRow = null;


            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        strWhere = " AND [Authorization_Type] in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD')  ";
                        strExcelRow = "4";
                        break;
                    case 1:
                        strWhere = "AND [Authorization_Type] in ('S', 'U') AND [Request_Decision] in ('FF', 'PF')   ";
                        strExcelRow = "5";
                        break;
                    case 2:
                        strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ( 'AD') AND  ([Decision_Reason] is null OR [Decision_Reason] like '010%')   ";
                        strExcelRow = "6";
                        break;
                    case 3:
                        strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('AD') AND  ([Decision_Reason] is not null AND [Decision_Reason] not like '010%')  ";
                        strExcelRow = "7";
                        break;
                    case 4:
                        strWhere = "AND [Authorization_Type]  in ( 'PS')  ";
                        strExcelRow = "26";
                        break;
                    case 5:
                        strWhere = "AND  [Authorization_Type]  in ('PS')   ";
                        strExcelRow = "27";
                        break;
                    case 6:
                        strWhere = "AND [Authorization_Type]  in ('PS')  AND  [Request_Decision] = 'AD' AND  ([Decision_Reason] is null OR [Decision_Reason] like '010%')   ";
                        strExcelRow = "28";
                        break;
                    case 7:
                        strWhere = "AND [Authorization_Type]  in ('PS')  AND  [Request_Decision] = 'AD' AND  ([Decision_Reason] is not null AND [Decision_Reason] not like '010%') ";
                        strExcelRow = "29";
                        break; ;
                    default:
                        break;
                }

                sbSQL.Append("SELECT ");
                sbSQL.Append(strExcelRow + " as ExcelRow, ");//4 AND
                sbSQL.Append("MAX(CASE WHEN  tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_ip, ");
                sbSQL.Append("MAX(CASE WHEN  tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_op, ");
                sbSQL.Append("'" + strState.Replace("'", "") + "' as State , ");
                sbSQL.Append("'" + strStartDate + "' as StartDate, ");
                sbSQL.Append("'" + strEndDate + "' as EndDate ");
                //sbSQL.Append("" + strCS_TADM_PRDCT_MAP + " as CS_TADM_PRDCT_MAP ");
                sbSQL.Append("FROM( ");
                sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                sbSQL.Append("INNER JOIN [IL_UCA].[dbo].[CS_PRODUCT_MAP] m ON m.PLAN_ST = c.CS_CO_CD_ST AND m.PRDCT_SYS_ID = c.PRDCT_SYS_ID AND m.CS_PRDCT_CD_SYS_ID = c.CS_PRDCT_CD_SYS_ID AND m.CS_CO_CD = c.CS_CO_CD ");
                sbSQL.Append("WHERE u.[State_of_Issue]  in (" + strState + ") AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                sbSQL.Append("AND [Authorization] IS NOT NULL  AND [Classification] = 'CS' AND m.CS_TADM_PRDCT_MAP  in (" + strCS_TADM_PRDCT_MAP + ") "); //
                if (!string.IsNullOrEmpty(strGroupNumbers))
                    sbSQL.Append("AND c.[PRDCT_CD_DESC] in (" + strGroupNumbers + ") ");
                sbSQL.Append(strWhere); 
                 sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                sbSQL.Append(") tmp ");
                sbSQL.Append("UNION ALL ");

            }

            //throw new Exception("Oh nooooooo!!!");

            return await FillCSDataTableAsync(_context.Database.Connection.ConnectionString, sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token);

        }



        public async Task<List<MHP_Yearly_Universes_Reporting_Model>> GetMHPDataAsync(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_DESC, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, string strMKT_TYP_DESC, string strCUST_SEG, bool blIsIFP, CancellationToken token)
        {


            StringBuilder sbSQL = new StringBuilder();

            string strWhere = null;
            string strExcelRow = null;

            foreach (string strLegalEntity in lstLegalEntities)
            {
                var legalNbr = strLegalEntity.Split('-')[0].Trim();
                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        case 0:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD') ";
                            strExcelRow = "4";
                            break;
                        case 1:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF')  AND [Decision_Reason] = 'Medically Necessary'   ";
                            strExcelRow = "5";
                            break;
                        case 2:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD') AND  [Decision_Reason]  <> 'Medically Necessary' AND [Decision_Reason] not like '054%' AND [Decision_Reason] not like '067%' ";
                            strExcelRow = "6";
                            break;
                        case 3:
                            strWhere = "AND [Authorization_Type]  in ( 'PS')  ";
                            strExcelRow = "56";
                            break;
                        case 4:
                            strWhere = "AND  [Authorization_Type]  in ('PS')  AND [Decision_Reason] = 'Medically Necessary'  ";
                            strExcelRow = "57";
                            break;
                        case 5:
                            strWhere = "AND [Authorization_Type]  in ('PS')  AND  [Decision_Reason] <> 'Medically Necessary' AND [Decision_Reason] not like '054%'  AND [Decision_Reason] not like '067%' ";
                            strExcelRow = "58";
                            break;
                        default:
                            break;
                    }


                    //AND (file_name LIKE 'Oxford%') AND [sheet_name]<> 'U12 


                    sbSQL.Append("SELECT ");
                    sbSQL.Append(strExcelRow + " as ExcelRow, ");//4 AND
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR','Par', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('NonPar Site','Non-Par') AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR','Par', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('NonPar Site','Non-Par') AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op, ");
                    sbSQL.Append("'" + strState.Replace("'","") + "' as State , ");
                    sbSQL.Append("'" + strStartDate + "' as StartDate, ");
                    sbSQL.Append("'" + strEndDate + "' as EndDate, ");
                    sbSQL.Append("'" + strLegalEntity + "' as LegalEntity ");
                    sbSQL.Append("FROM( ");
                    sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                    sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                    sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                    sbSQL.Append("WHERE u.[State_of_Issue]  in (" + strState + ")  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                    sbSQL.Append("AND c.[MKT_SEG_RLLP_DESC] in (" + strMKT_SEG_RLLP_DESC + ") AND  c.[FINC_ARNG_DESC] in (" + strFINC_ARNG_DESC + ")  AND [Authorization] IS NOT NULL  AND [Classification]  IN ('EI','EI_OX')  "); //
                    sbSQL.Append("AND c.[LEG_ENTY_NBR] = '" + legalNbr + "' "); //

                    //sbSQL.Append(" AND [sheet_name] " + (blIsIFP ? "=" : "<>") + " 'U12' ");

                    if (!string.IsNullOrEmpty(strMKT_TYP_DESC))
                        sbSQL.Append("AND c.[MKT_TYP_DESC] in (" + strMKT_TYP_DESC + ") ");
                    if (!string.IsNullOrEmpty(strCUST_SEG))
                        sbSQL.Append("AND c.[CUST_SEG_NBR] in (" + strCUST_SEG + ") ");
                    sbSQL.Append(strWhere);
                    sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                    sbSQL.Append(") tmp ");
                    sbSQL.Append("UNION ALL ");

                }




            }
            //throw new Exception("Oh nooooooo!!!");

            return await FillDataTableAsync(_context.Database.Connection.ConnectionString, sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token);


        }


        public async Task<List<MHPIFP_Yearly_Universes_Reporting_Model>> GetMHPIFPDataAsync(string strState, string strStartDate, string strEndDate, List<string> lstProductCode, CancellationToken token)
        {


            StringBuilder sbSQL = new StringBuilder();

            string strWhere = null;
            string strExcelRow = null;

            foreach (string strProd in lstProductCode)
            {
                var prod = strProd.Split('-')[0].Trim();
                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        case 0:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD') ";
                            strExcelRow = "4";
                            break;
                        case 1:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF')  AND [Decision_Reason] = 'Medically Necessary'   ";
                            strExcelRow = "5";
                            break;
                        case 2:
                            strWhere = "AND [Authorization_Type]  in ('S', 'U') AND [Request_Decision] in ('FF', 'PF', 'AD') AND  [Decision_Reason]  <> 'Medically Necessary' AND [Decision_Reason] not like '054%' AND [Decision_Reason] not like '067%' ";
                            strExcelRow = "6";
                            break;
                        case 3:
                            strWhere = "AND [Authorization_Type]  in ( 'PS')  ";
                            strExcelRow = "56";
                            break;
                        case 4:
                            strWhere = "AND  [Authorization_Type]  in ('PS')  AND [Decision_Reason] = 'Medically Necessary'  ";
                            strExcelRow = "57";
                            break;
                        case 5:
                            strWhere = "AND [Authorization_Type]  in ('PS')  AND  [Decision_Reason] <> 'Medically Necessary' AND [Decision_Reason] not like '054%'  AND [Decision_Reason] not like '067%' ";
                            strExcelRow = "58";
                            break;
                        default:
                            break;
                    }


                    //AND (file_name LIKE 'Oxford%') AND [sheet_name]<> 'U12 


                    sbSQL.Append("SELECT ");
                    sbSQL.Append(strExcelRow + " as ExcelRow, ");//4 AND
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR','Par', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('NonPar Site','Non-Par') AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR','Par', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                    sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('NonPar Site','Non-Par') AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op, ");
                    sbSQL.Append("'" + strState.Replace("'", "") + "' as State , ");
                    sbSQL.Append("'" + strStartDate + "' as StartDate, ");
                    sbSQL.Append("'" + strEndDate + "' as EndDate, ");
                    sbSQL.Append("'" + strProd + "' as Product ");
                    sbSQL.Append("FROM( ");
                    sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                    sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                    sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                    sbSQL.Append("WHERE u.[State_of_Issue]  in (" + strState + ")  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                    sbSQL.Append("AND [Authorization] IS NOT NULL AND  [Classification]= 'IFP' "); //

                    sbSQL.Append("AND c.[PRDCT_CD] = '" + prod + "' "); //
                    sbSQL.Append(strWhere);
                    sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                    sbSQL.Append(") tmp ");
                    sbSQL.Append("UNION ALL ");

                }




            }
            //throw new Exception("Oh nooooooo!!!");

            return await FillIFPDataTableAsync(_context.Database.Connection.ConnectionString, sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token);


        }



        public async Task<List<MHP_Yearly_Universes_Details_Model>> GetMHPDetailsAsync(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_DESC, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, string strMKT_TYP_DESC, string strCUST_SEG, bool blIsIFP, CancellationToken token)
        {

            StringBuilder sbSQL = new StringBuilder();

            foreach (string strLegalEntity in lstLegalEntities)
            {
                var legalNbr = strLegalEntity.Split('-')[0].Trim();


                sbSQL.Append("SELECT u.[Authorization], ");
                sbSQL.Append("u.[Request_Decision], ");
                sbSQL.Append("u.[Authorization_Type], ");
                sbSQL.Append("u.[Par_NonPar_Site], ");
                sbSQL.Append("u.[Inpatient_Outpatient], ");
                sbSQL.Append("CONVERT(VARCHAR(10), u.[Request_Date], 101) as Request_Date, ");
                sbSQL.Append("u.[State_of_Issue], ");
                sbSQL.Append("c.[FINC_ARNG_DESC], ");
                sbSQL.Append("u.[Decision_Reason], ");
                sbSQL.Append("c.[CUST_SEG_NBR], ");
                sbSQL.Append("c.[CUST_SEG_NM], ");
                sbSQL.Append("c.[MKT_SEG_RLLP_DESC], ");
                sbSQL.Append("c.[MKT_TYP_DESC], ");
                //sbSQL.Append("c.[LEG_ENTY_NBR] + '-' + c.[LEG_ENTY_FULL_NM] as LegalEntity, ");
                sbSQL.Append("c.[LEG_ENTY_NBR], ");
                sbSQL.Append("c.[LEG_ENTY_FULL_NM], ");
                //sbSQL.Append("'" + strLegalEntity.Replace("'","''") + "' as LegalEntity, ");
                sbSQL.Append("u.[Enrollee_First_Name],");
                sbSQL.Append("u.[Enrollee_Last_Name], ");
                sbSQL.Append("u.[Cardholder_ID], ");
                sbSQL.Append("CONVERT(VARCHAR(10), u.[Member_Date_of_Birth], 101) as Member_Date_of_Birth, ");
                sbSQL.Append("u.[Procedure_Code_Description], ");
                sbSQL.Append("u.[Primary_Procedure_Code_Req] , ");
                sbSQL.Append("u.[Primary_Diagnosis_Code] ");
                //sbSQL.Append("u.[Diagnosis_Code_Description]");
                sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                sbSQL.Append("WHERE u.[State_of_Issue] in (" + strState + ")  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                sbSQL.Append("AND c.[MKT_SEG_RLLP_DESC] in (" + strMKT_SEG_RLLP_DESC + ") AND  c.[FINC_ARNG_DESC] in (" + strFINC_ARNG_DESC + ")  AND [Authorization] IS NOT NULL   AND [Classification]  IN ('EI','EI_OX')   "); //
                sbSQL.Append("AND c.[LEG_ENTY_NBR] = '" + legalNbr + "' "); //
                //sbSQL.Append(" AND [sheet_name] " + (blIsIFP ? "=" : "<>") + " 'U12' ");
                if (!string.IsNullOrEmpty(strMKT_TYP_DESC))
                    sbSQL.Append("AND c.[MKT_TYP_DESC] in (" + strMKT_TYP_DESC + ") ");
                if (!string.IsNullOrEmpty(strCUST_SEG))
                    sbSQL.Append("AND c.[CUST_SEG_NBR] in (" + strCUST_SEG + ") ");
                sbSQL.Append("UNION ALL ");


            }

            return await FillDataTableDetailsAsync(_context.Database.Connection.ConnectionString, sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token);


        }

        public async Task<List<MHPIFP_Yearly_Universes_Details_Model>> GetMHPIFPDetailsAsync(string strState, string strStartDate, string strEndDate, List<string> lstProductCode, CancellationToken token)
        {

            StringBuilder sbSQL = new StringBuilder();

            foreach (string strProd in lstProductCode)
            {
                var prod = strProd.Split('-')[0].Trim();


                sbSQL.Append("SELECT u.[Authorization], ");
                sbSQL.Append("u.[Request_Decision], ");
                sbSQL.Append("u.[Authorization_Type], ");
                sbSQL.Append("u.[Par_NonPar_Site], ");
                sbSQL.Append("u.[Inpatient_Outpatient], ");
                sbSQL.Append("CONVERT(VARCHAR(10), u.[Request_Date], 101) as Request_Date, ");
                sbSQL.Append("u.[State_of_Issue], ");
                sbSQL.Append("c.[PRDCT_CD], ");
                sbSQL.Append("c.[PRDCT_CD_DESC], ");
                sbSQL.Append("u.[Decision_Reason], ");
                sbSQL.Append("u.[Enrollee_First_Name],");
                sbSQL.Append("u.[Enrollee_Last_Name], ");
                sbSQL.Append("u.[Cardholder_ID], ");
                sbSQL.Append("CONVERT(VARCHAR(10), u.[Member_Date_of_Birth], 101) as Member_Date_of_Birth, ");
                sbSQL.Append("u.[Procedure_Code_Description], ");
                sbSQL.Append("u.[Primary_Procedure_Code_Req] , ");
                sbSQL.Append("u.[Primary_Diagnosis_Code] ");
                sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                sbSQL.Append("WHERE u.[State_of_Issue]  in (" + strState + ")  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                sbSQL.Append("AND [Authorization] IS NOT NULL AND [Classification] = 'IFP'  "); //
                sbSQL.Append("AND c.[PRDCT_CD] = '" + prod + "' "); //
                sbSQL.Append("UNION ALL ");


            }

            return await FillIFPDataTableDetailsAsync(_context.Database.Connection.ConnectionString, sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token);


        }

        public async Task<List<MHPCS_Yearly_Universes_Details_Model>> GetMHPCSDetailsAsync(string strState, string strStartDate, string strEndDate, string strCS_TADM_PRDCT_MAP, string strGroupNumbers, CancellationToken token)
        {

            StringBuilder sbSQL = new StringBuilder();

           
  

                sbSQL.Append("SELECT u.[Authorization], ");
                sbSQL.Append("u.[Request_Decision], ");
                sbSQL.Append("u.[Authorization_Type], ");
                sbSQL.Append("u.[Par_NonPar_Site], ");
                sbSQL.Append("u.[Inpatient_Outpatient], ");
                sbSQL.Append("CONVERT(VARCHAR(10), u.[Request_Date], 101) as Request_Date, ");
                sbSQL.Append("u.[State_of_Issue], ");
                sbSQL.Append("c.[FINC_ARNG_DESC], ");
                sbSQL.Append("u.[Decision_Reason], ");

                sbSQL.Append("MIN(m.[CS_TADM_PRDCT_MAP]) AS CS_TADM_PRDCT_MAP, ");

                sbSQL.Append("u.[Enrollee_First_Name],");
                sbSQL.Append("u.[Enrollee_Last_Name], ");
                sbSQL.Append("u.[Cardholder_ID], ");
                sbSQL.Append("CONVERT(VARCHAR(10), u.[Member_Date_of_Birth], 101) as Member_Date_of_Birth, ");
                sbSQL.Append("u.[Procedure_Code_Description], ");
            sbSQL.Append("u.[Primary_Procedure_Code_Req] , ");
            sbSQL.Append("u.[Primary_Diagnosis_Code], ");
            sbSQL.Append("u.[Group_Number], ");
            sbSQL.Append("c.[PRDCT_CD_DESC] ");
            //sbSQL.Append("u.[Diagnosis_Code_Description]");
            sbSQL.Append("FROM[IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                sbSQL.Append("INNER JOIN [IL_UCA].[dbo].[CS_PRODUCT_MAP] m ON m.PLAN_ST = c.CS_CO_CD_ST AND m.PRDCT_SYS_ID = c.PRDCT_SYS_ID AND m.CS_PRDCT_CD_SYS_ID = c.CS_PRDCT_CD_SYS_ID AND m.CS_CO_CD = c.CS_CO_CD ");
                sbSQL.Append("WHERE u.[State_of_Issue] in (" + strState + ")  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                sbSQL.Append("AND [Authorization] IS NOT NULL   AND [Classification] = 'CS'  AND m.CS_TADM_PRDCT_MAP  in (" + strCS_TADM_PRDCT_MAP + ") "); //
                if (!string.IsNullOrEmpty(strGroupNumbers))
                    sbSQL.Append("AND c.[PRDCT_CD_DESC] in (" + strGroupNumbers + ") ");


            sbSQL.Append("GROUP BY ");
            sbSQL.Append("u.[Authorization], ");
            sbSQL.Append("u.[Request_Decision], ");
            sbSQL.Append("u.[Authorization_Type], ");
            sbSQL.Append("u.[Par_NonPar_Site], ");
            sbSQL.Append("u.[Inpatient_Outpatient], ");
            sbSQL.Append("u.[Request_Date], ");
            sbSQL.Append("u.[State_of_Issue], ");
            sbSQL.Append("c.[FINC_ARNG_DESC], ");
            sbSQL.Append("u.[Decision_Reason], ");
            sbSQL.Append("u.[Enrollee_First_Name], ");
            sbSQL.Append("u.[Enrollee_Last_Name], ");
            sbSQL.Append("u.[Cardholder_ID], ");
            sbSQL.Append("u.[Member_Date_of_Birth], ");
            sbSQL.Append("u.[Procedure_Code_Description], ");
            sbSQL.Append("u.[Primary_Procedure_Code_Req] , ");
            sbSQL.Append("u.[Primary_Diagnosis_Code], ");
            sbSQL.Append("u.[Group_Number], ");
            sbSQL.Append("c.[PRDCT_CD_DESC] ");
            return await FillDataTableCSDetailsAsync(_context.Database.Connection.ConnectionString, sbSQL.ToString(), token);


        }



        public static async Task<List<MHP_Yearly_Universes_Reporting_Model>> FillDataTableAsync(string strConnectionString, string strSQL, CancellationToken token)
        {
            List<MHP_Yearly_Universes_Reporting_Model> lstMHP = new List<MHP_Yearly_Universes_Reporting_Model>();


            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    cmd.CommandTimeout = 9999999;
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
                    {

                        while (await r.ReadAsync())
                        {

                            if (token.IsCancellationRequested)
                            {
                                return null;
                            }


                            lstMHP.Add(new MHP_Yearly_Universes_Reporting_Model
                            {
                                ExcelRow = (int)r.GetValue(r.GetOrdinal("ExcelRow")),
                                cnt_in_ip = (r.GetValue(r.GetOrdinal("cnt_in_ip")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_in_ip")) + "")),
                                cnt_on_ip = (r.GetValue(r.GetOrdinal("cnt_on_ip")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_on_ip")) + "")),
                                cnt_in_op = (r.GetValue(r.GetOrdinal("cnt_in_op")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_in_op")) + "")),
                                cnt_on_op = (r.GetValue(r.GetOrdinal("cnt_on_op")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_on_op")) + "")),
                                State = (string)r.GetValue(r.GetOrdinal("State")),
                                StartDate = (string)r.GetValue(r.GetOrdinal("StartDate")),
                                EndDate = (string)r.GetValue(r.GetOrdinal("EndDate")),
                                LegalEntity = (string)r.GetValue(r.GetOrdinal("LegalEntity"))
                            });
                        }

                    }
                }
                connection.Close() ;
            }

            return lstMHP;
        }

        public static async Task<List<MHPIFP_Yearly_Universes_Reporting_Model>> FillIFPDataTableAsync(string strConnectionString, string strSQL, CancellationToken token)
        {
            List<MHPIFP_Yearly_Universes_Reporting_Model> lstMHP = new List<MHPIFP_Yearly_Universes_Reporting_Model>();


            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    cmd.CommandTimeout = 9999999;
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
                    {

                        while (await r.ReadAsync())
                        {

                            if (token.IsCancellationRequested)
                            {
                                return null;
                            }


                            lstMHP.Add(new MHPIFP_Yearly_Universes_Reporting_Model
                            {
                                ExcelRow = (int)r.GetValue(r.GetOrdinal("ExcelRow")),
                                cnt_in_ip = (r.GetValue(r.GetOrdinal("cnt_in_ip")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_in_ip")) + "")),
                                cnt_on_ip = (r.GetValue(r.GetOrdinal("cnt_on_ip")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_on_ip")) + "")),
                                cnt_in_op = (r.GetValue(r.GetOrdinal("cnt_in_op")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_in_op")) + "")),
                                cnt_on_op = (r.GetValue(r.GetOrdinal("cnt_on_op")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_on_op")) + "")),
                                State = (string)r.GetValue(r.GetOrdinal("State")),
                                StartDate = (string)r.GetValue(r.GetOrdinal("StartDate")),
                                EndDate = (string)r.GetValue(r.GetOrdinal("EndDate")),
                                Product = (string)r.GetValue(r.GetOrdinal("Product"))
                            });
                        }

                    }
                }
                connection.Close();
            }

            return lstMHP;
        }

        public static async Task<List<MHPCS_Yearly_Universes_Reporting_Model>> FillCSDataTableAsync(string strConnectionString, string strSQL, CancellationToken token)
        {
            List<MHPCS_Yearly_Universes_Reporting_Model> lstMHP = new List<MHPCS_Yearly_Universes_Reporting_Model>();


            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    cmd.CommandTimeout = 9999999;
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
                    {

                        while (await r.ReadAsync())
                        {

                            if (token.IsCancellationRequested)
                            {
                                return null;
                            }


                            lstMHP.Add(new MHPCS_Yearly_Universes_Reporting_Model
                            {
                                ExcelRow = (int)r.GetValue(r.GetOrdinal("ExcelRow")),
                                cnt_ip = (r.GetValue(r.GetOrdinal("cnt_ip")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_ip")) + "")),
                                cnt_op = (r.GetValue(r.GetOrdinal("cnt_op")) == DBNull.Value ? null : (int?)int.Parse(r.GetValue(r.GetOrdinal("cnt_op")) + "")),
                                State = (string)r.GetValue(r.GetOrdinal("State")),
                                StartDate = (string)r.GetValue(r.GetOrdinal("StartDate")),
                                EndDate = (string)r.GetValue(r.GetOrdinal("EndDate"))
                                //CS_TADM_PRDCT_MAP = (string)r.GetValue(r.GetOrdinal("CS_TADM_PRDCT_MAP"))
                            });
                        }

                    }
                }
                connection.Close();
            }

            return lstMHP;
        }


        public static async Task<List<MHP_Yearly_Universes_Details_Model>> FillDataTableDetailsAsync(string strConnectionString, string strSQL, CancellationToken token)
        {
            List<MHP_Yearly_Universes_Details_Model> lstMHP = new List<MHP_Yearly_Universes_Details_Model>();


            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    cmd.CommandTimeout = 9999999;
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
                    {

                        try
                        {
                            while (await r.ReadAsync())
                            {

                                if (token.IsCancellationRequested)
                                {
                                    return null;
                                }

                                lstMHP.Add(new MHP_Yearly_Universes_Details_Model
                                {

                                    Authorization = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Authorization"))),
                                    Request_Decision = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Request_Decision"))),
                                    Authorization_Type = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Authorization_Type"))),
                                    Par_NonPar_Site = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Par_NonPar_Site"))),
                                    Inpatient_Outpatient = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Inpatient_Outpatient"))),
                                    Request_Date = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Request_Date"))),
                                    State_of_Issue = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("State_of_Issue"))),
                                    FINC_ARNG_DESC = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("FINC_ARNG_DESC"))),
                                    Decision_Reason = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Decision_Reason"))),
                                    CUST_SEG_NBR = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("CUST_SEG_NBR"))),
                                    CUST_SEG_NM = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("CUST_SEG_NM"))),
                                    //LegalEntity = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("LegalEntity"))), 
                                    LEG_ENTY_NBR = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("LEG_ENTY_NBR"))),
                                    LEG_ENTY_FULL_NM = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("LEG_ENTY_FULL_NM"))),
                                    MKT_SEG_RLLP_DESC = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("MKT_SEG_RLLP_DESC"))),
                                    MKT_TYP_DESC = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("MKT_TYP_DESC"))),
                                    Enrollee_First_Name = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Enrollee_First_Name"))),
                                    Enrollee_Last_Name = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Enrollee_Last_Name"))),
                                    Cardholder_ID = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Cardholder_ID"))),
                                    Member_Date_of_Birth = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Member_Date_of_Birth"))),
                                    Procedure_Code_Description = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Procedure_Code_Description"))),
                                    Primary_Procedure_Code_Req = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Primary_Procedure_Code_Req"))),
                                    Primary_Diagnosis_Code = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Primary_Diagnosis_Code")))
                                    //Diagnosis_Code_Description = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Diagnosis_Code_Description")))
     
                                });
                            }
                        }
                        catch(Exception e)
                        {
                            var ex = e;
                        }

                        

                    }
                }
                connection.Close();
            }

            return lstMHP;
        }

        public static async Task<List<MHPIFP_Yearly_Universes_Details_Model>> FillIFPDataTableDetailsAsync(string strConnectionString, string strSQL, CancellationToken token)
        {
            List<MHPIFP_Yearly_Universes_Details_Model> lstMHP = new List<MHPIFP_Yearly_Universes_Details_Model>();


            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    cmd.CommandTimeout = 9999999;
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
                    {

                        try
                        {
                            while (await r.ReadAsync())
                            {

                                if (token.IsCancellationRequested)
                                {
                                    return null;
                                }

                                lstMHP.Add(new MHPIFP_Yearly_Universes_Details_Model
                                {

                                    Authorization = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Authorization"))),
                                    Request_Decision = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Request_Decision"))),
                                    Authorization_Type = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Authorization_Type"))),
                                    Par_NonPar_Site = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Par_NonPar_Site"))),
                                    Inpatient_Outpatient = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Inpatient_Outpatient"))),
                                    Request_Date = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Request_Date"))),
                                    State_of_Issue = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("State_of_Issue"))),
                                    Decision_Reason = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Decision_Reason"))),

                                    PRDCT_CD = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("PRDCT_CD"))),
                                    PRDCT_CD_DESC = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("PRDCT_CD_DESC"))),

                                    Enrollee_First_Name = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Enrollee_First_Name"))),
                                    Enrollee_Last_Name = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Enrollee_Last_Name"))),
                                    Cardholder_ID = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Cardholder_ID"))),
                                    Member_Date_of_Birth = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Member_Date_of_Birth"))),
                                    Procedure_Code_Description = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Procedure_Code_Description"))),
                                    Primary_Procedure_Code_Req = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Primary_Procedure_Code_Req"))),
                                    Primary_Diagnosis_Code = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Primary_Diagnosis_Code")))
                                    //Diagnosis_Code_Description = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Diagnosis_Code_Description")))

                                });
                            }
                        }
                        catch (Exception e)
                        {
                            var ex = e;
                        }



                    }
                }
                connection.Close();
            }

            return lstMHP;
        }

        public static async Task<List<MHPCS_Yearly_Universes_Details_Model>> FillDataTableCSDetailsAsync(string strConnectionString, string strSQL, CancellationToken token)
        {
            List<MHPCS_Yearly_Universes_Details_Model> lstMHP = new List<MHPCS_Yearly_Universes_Details_Model>();


            using (SqlConnection connection = new SqlConnection(strConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    cmd.CommandTimeout = 9999999;
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
                    {

                        try
                        {
                            while (await r.ReadAsync())
                            {

                                if (token.IsCancellationRequested)
                                {
                                    return null;
                                }

                                lstMHP.Add(new MHPCS_Yearly_Universes_Details_Model
                                {

                                    Authorization = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Authorization"))),
                                    Request_Decision = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Request_Decision"))),
                                    Authorization_Type = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Authorization_Type"))),
                                    Par_NonPar_Site = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Par_NonPar_Site"))),
                                    Inpatient_Outpatient = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Inpatient_Outpatient"))),
                                    Request_Date = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Request_Date"))),
                                    State_of_Issue = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("State_of_Issue"))),
                                    CS_TADM_PRDCT_MAP = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("CS_TADM_PRDCT_MAP"))),
                                    Decision_Reason = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Decision_Reason"))),

                                    Enrollee_First_Name = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Enrollee_First_Name"))),
                                    Enrollee_Last_Name = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Enrollee_Last_Name"))),
                                    Cardholder_ID = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Cardholder_ID"))),
                                    Member_Date_of_Birth = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Member_Date_of_Birth"))),
                                    Procedure_Code_Description = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Procedure_Code_Description"))),
                                    Primary_Procedure_Code_Req = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Primary_Procedure_Code_Req"))),
                                    Primary_Diagnosis_Code = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Primary_Diagnosis_Code"))),
                                    Group_Number = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Group_Number"))),
                                    PRDCT_CD_DESC = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("PRDCT_CD_DESC")))
                                    //Diagnosis_Code_Description = Shared.ConvertFromDBVal<string>(r.GetValue(r.GetOrdinal("Diagnosis_Code_Description")))

                                });
                            }
                        }
                        catch (Exception e)
                        {
                            var ex = e;
                        }



                    }
                }
                connection.Close();
            }

            return lstMHP;
        }



        public List<string> GetStates(bool isCS = false)
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT Filter_Value as State_of_Issue FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'State_of_Issue' AND Report_Type in ('EI','ALL') ORDER BY State_of_Issue ");


            //sbSQL.Append("SELECT distinct State_of_Issue FROM [stg].[MHP_Yearly_Universes]  ");
            //sbSQL.Append("WHERE State_of_Issue IS NOT NULL AND [file_name] "+ (isCS ? "": "NOT") + " LIKE 'C&S%'    AND [Authorization] IS NOT NULL   ORDER BY State_of_Issue; ");

            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }


        public List<string> GetMKT_SEG_RLLP_DESC(bool isCS = false)
        {
  
            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT Filter_Value as MKT_SEG_RLLP_DESC FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'MKT_SEG_RLLP_DESC' AND Report_Type in ('EI','ALL') ORDER BY MKT_SEG_RLLP_DESC ");

            //sbSQL.Append(" SELECT distinct c.MKT_SEG_RLLP_DESC FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.MKT_SEG_RLLP_DESC IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'    AND [Authorization] IS NOT NULL  ORDER BY c.MKT_SEG_RLLP_DESC ; ");

            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }



        public List<string> GetCS_TADM_PRDCT_MAP(bool isCS = true)
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT Filter_Value as CS_TADM_PRDCT_MAP FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'CS_TADM_PRDCT_MAP' AND Report_Type in ('C&S','ALL') ORDER BY CS_TADM_PRDCT_MAP");

            //sbSQL.Append("select distinct CS_TADM_PRDCT_MAP FROM[IL_UCA].[dbo].[CS_PRODUCT_MAP] ORDER BY CS_TADM_PRDCT_MAP ");

            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }




        public List<string> GetFINC_ARNG_DESC(bool isCS = false)
        {

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("SELECT Filter_Value as FINC_ARNG_DESC FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type  = 'FINC_ARNG_DESC' AND Report_Type in ('EI','ALL') ORDER BY FINC_ARNG_DESC ");

            //sbSQL.Append(" SELECT distinct c.FINC_ARNG_DESC as FINC_ARNG_DESC FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.FINC_ARNG_CD IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'   AND [Authorization] IS NOT NULL   ORDER BY c.FINC_ARNG_DESC ; ");

            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }

        public List<string> GetLEG_ENTY(bool isCS = false)
        {

            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("SELECT Filter_Value as LEG_ENTY FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'LEG_ENTY' AND Report_Type in ('EI','ALL') ORDER BY LEG_ENTY");


            //sbSQL.Append(" SELECT distinct c.[LEG_ENTY_NBR] + ' - ' + MAX(c.LEG_ENTY_FULL_NM) as LEG_ENTY FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.LEG_ENTY_FULL_NM IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'   AND [Authorization] IS NOT NULL   ");
            //sbSQL.Append("GROUP BY c.[LEG_ENTY_NBR] ");
            //sbSQL.Append("ORDER BY LEG_ENTY ; ");
            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }



        public List<string> GetMKT_TYP_DESC(bool isCS = false)
        {

            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("SELECT Filter_Value as MKT_TYP_DESC FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'MKT_TYP_DESC' AND Report_Type in ('EI','ALL') ORDER BY MKT_TYP_DESC");


            //sbSQL.Append(" SELECT distinct c.MKT_TYP_DESC FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.MKT_TYP_DESC IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'   AND [Authorization] IS NOT NULL    ORDER BY c.MKT_TYP_DESC ; ");

            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }

        public List<string> GetCUST_SEG(bool isCS = false)
        {

            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("SELECT Filter_Value as CUST_SEG FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'CUST_SEG' ORDER BY CUST_SEG");


            //sbSQL.Append(" SELECT distinct c.[LEG_ENTY_NBR] + ' - ' + MAX(c.LEG_ENTY_FULL_NM) as LEG_ENTY FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.LEG_ENTY_FULL_NM IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'   AND [Authorization] IS NOT NULL   ");
            //sbSQL.Append("GROUP BY c.[LEG_ENTY_NBR] ");
            //sbSQL.Append("ORDER BY LEG_ENTY ; ");
            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }

        public List<Group_State_Model> GetGroupState()
        {

            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("select distinct  [State_of_Issue], Group_Number from [stg].[MHP_Group_State] ");
            sbSQL.Append("ORDER BY State_of_Issue, Group_Number");


            //sbSQL.Append(" SELECT distinct c.[LEG_ENTY_NBR] + ' - ' + MAX(c.LEG_ENTY_FULL_NM) as LEG_ENTY FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.LEG_ENTY_FULL_NM IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'   AND [Authorization] IS NOT NULL   ");
            //sbSQL.Append("GROUP BY c.[LEG_ENTY_NBR] ");
            //sbSQL.Append("ORDER BY LEG_ENTY ; ");
            return _context.Database.SqlQuery<Group_State_Model>(sbSQL.ToString()).ToList<Group_State_Model>();


        }


        public List<string> GetProductCode()
        {

            StringBuilder sbSQL = new StringBuilder();


            sbSQL.Append("SELECT Filter_Value as PRDCT_CD FROM stg.MHP_Universes_Filter_Cache  WHERE Filter_Type = 'PRDCT_CD' AND Report_Type = 'IFP' ORDER BY PRDCT_CD");


            //sbSQL.Append(" SELECT distinct c.[LEG_ENTY_NBR] + ' - ' + MAX(c.LEG_ENTY_FULL_NM) as LEG_ENTY FROM [stg].[MHP_Yearly_Universes] u ");
            //sbSQL.Append("INNER JOIN[IL_UCA].[stg].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            //sbSQL.Append("WHERE c.LEG_ENTY_FULL_NM IS NOT NULL AND[file_name] " + (isCS ? "" : "NOT") + " LIKE 'C&S%'   AND [Authorization] IS NOT NULL   ");
            //sbSQL.Append("GROUP BY c.[LEG_ENTY_NBR] ");
            //sbSQL.Append("ORDER BY LEG_ENTY ; ");
            return _context.Database.SqlQuery<string>(sbSQL.ToString()).ToList<string>();


        }


    }
}
