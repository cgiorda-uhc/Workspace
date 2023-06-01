
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Text;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Concrete.MHP;

public class MHPUniverse_Repo : IMHPUniverse_Repo
{

    private readonly IRelationalDataAccess _db;

    public MHPUniverse_Repo(IRelationalDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<MHP_EI_Model>> GetMHP_EI_Async(List<string> strState, string strStartDate, string strEndDate, List<string> strFINC_ARNG_DESC, List<string> strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, List<string> strMKT_TYP_DESC, CancellationToken token)

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


                sbSQL.Append("SELECT ");
                sbSQL.Append(strExcelRow + " as ExcelRow, ");//4 AND
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op, ");
                sbSQL.Append("'" + String.Join(",", strState) + "' as State , ");
                sbSQL.Append("'" + strStartDate + "' as StartDate, ");
                sbSQL.Append("'" + strEndDate + "' as EndDate, ");
                sbSQL.Append("'" + strLegalEntity + "' as LegalEntity ");
                sbSQL.Append("FROM( ");
                sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                sbSQL.Append("FROM[VCT_DB].[mhp].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [VCT_DB].[mhp].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                sbSQL.Append("WHERE u.[State_of_Issue]  in ('" + String.Join(",", strState).Replace(",", "','") + "')  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                sbSQL.Append("AND c.[MKT_SEG_RLLP_DESC] in ('" + String.Join(",", strMKT_SEG_RLLP_DESC).Replace(",", "','") + "') AND  c.[FINC_ARNG_DESC] in ('" + String.Join(",", strFINC_ARNG_DESC).Replace(",", "','") + "')  AND [Authorization] IS NOT NULL AND (file_name LIKE 'UnitedPCP%') AND [sheet_name]<> 'U12'  "); //
                sbSQL.Append("AND c.[LEG_ENTY_NBR] = '" + legalNbr + "' "); //
                if (strMKT_TYP_DESC != null)
                    sbSQL.Append("AND c.[MKT_TYP_DESC] in ('" + String.Join(",", strMKT_TYP_DESC).Replace(",", "','") + "') ");
                sbSQL.Append(strWhere);
                sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                sbSQL.Append(") tmp ");
                sbSQL.Append("UNION ALL ");

            }

        }


        var results = _db.LoadData<MHP_EI_Model>(sql: sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token, connectionId: "VCT_DB");

        return results;
    }



    public Task<IEnumerable<MHP_CS_Model>> GetMHP_CS_Async(List<string> strState, string strStartDate, string strEndDate, List<string> strCS_TADM_PRDCT_MAP, CancellationToken token)

    {

        StringBuilder sbSQL = new StringBuilder();

        string strWhere = null;
        string strExcelRow = null;



        for (int i = 0; i < 12; i++)
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
            sbSQL.Append("MAX(CASE WHEN  tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
            sbSQL.Append("MAX(CASE WHEN  tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
            sbSQL.Append("MAX(CASE WHEN  tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
            sbSQL.Append("MAX(CASE WHEN  tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op, ");
            sbSQL.Append("'" + String.Join(",", strState) + "' as State , ");
            sbSQL.Append("'" + strStartDate + "' as StartDate, ");
            sbSQL.Append("'" + strEndDate + "' as EndDate ");
            sbSQL.Append("FROM( ");
            sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
            sbSQL.Append("FROM[VCT_DB].[mhp].[MHP_Yearly_Universes] u ");
            sbSQL.Append("INNER JOIN [VCT_DB].[mhp].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            sbSQL.Append("INNER JOIN [IL_UCA].[dbo].[CS_PRODUCT_MAP] m ON m.PLAN_ST = c.CS_CO_CD_ST AND m.PRDCT_SYS_ID = c.PRDCT_SYS_ID AND m.CS_PRDCT_CD_SYS_ID = c.CS_PRDCT_CD_SYS_ID AND m.CS_CO_CD = c.CS_CO_CD ");
            sbSQL.Append("WHERE u.[State_of_Issue]  in ('" + String.Join(",", strState).Replace(",", "','") + "') AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
            sbSQL.Append("AND [Authorization] IS NOT NULL  AND [file_name] LIKE 'C&S%' AND m.CS_TADM_PRDCT_MAP  in ('" + String.Join(",", strCS_TADM_PRDCT_MAP).Replace(",", "','") + "') "); //
            sbSQL.Append(strWhere);
            sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
            sbSQL.Append(") tmp ");
            sbSQL.Append("UNION ALL ");

        }

        var results = _db.LoadData<MHP_CS_Model>(sql: sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token, connectionId: "VCT_DB");

        return results;
    }



    public Task<IEnumerable<MHP_IFP_Model>> GetMHP_IFP_Async(List<string> strState, string strStartDate, string strEndDate, List<string> ProductCodes, CancellationToken token)

    {

        StringBuilder sbSQL = new StringBuilder();

        string strWhere = null;
        string strExcelRow = null;

        foreach (string strLegalEntity in ProductCodes)
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
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] in ('Site is PAR', 'N/A')  AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op, ");
                sbSQL.Append("'" + String.Join(",", strState) + "' as State , ");
                sbSQL.Append("'" + strStartDate + "' as StartDate, ");
                sbSQL.Append("'" + strEndDate + "' as EndDate, ");
                sbSQL.Append("'" + strLegalEntity + "' as LegalEntity ");
                sbSQL.Append("FROM( ");
                sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt, u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                sbSQL.Append("FROM[VCT_DB].[mhp].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [VCT_DB].[mhp].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
                sbSQL.Append("WHERE u.[State_of_Issue]  in ('" + String.Join(",", strState).Replace(",", "','") + "')  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
                //sbSQL.Append("AND c.[MKT_SEG_RLLP_DESC] in ('" + String.Join(",", strMKT_SEG_RLLP_DESC).Replace(",", "','") + "') AND  c.[FINC_ARNG_DESC] in ('" + String.Join(",", strFINC_ARNG_DESC).Replace(",", "','") + "')  AND [Authorization] IS NOT NULL AND (file_name LIKE 'UnitedPCP%') AND [sheet_name] = 'U12'  "); //
                //sbSQL.Append("AND c.[LEG_ENTY_NBR] = '" + legalNbr + "' "); //
                //if (strMKT_TYP_DESC != null)
                //    sbSQL.Append("AND c.[MKT_TYP_DESC] in ('" + String.Join(",", strMKT_TYP_DESC).Replace(",", "','") + "') ");
                //sbSQL.Append(strWhere);
                sbSQL.Append("GROUP BY [State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                sbSQL.Append(") tmp ");
                sbSQL.Append("UNION ALL ");


            }

        }


        var results = _db.LoadData<MHP_IFP_Model>(sql: sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token, connectionId: "VCT_DB");

        return results;
    }

    public Task<IEnumerable<MPHUniverseDetails_Model>> GetMHPEIDetailsAsync(List<string> strState, string strStartDate, string strEndDate, List<string> strFINC_ARNG_DESC, List<string> strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, List<string> strMKT_TYP_DESC, CancellationToken token)
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
            sbSQL.Append("c.[MKT_SEG_RLLP_DESC], ");
            sbSQL.Append("c.[MKT_TYP_DESC], ");
            sbSQL.Append("c.[LEG_ENTY_NBR], ");
            sbSQL.Append("c.[LEG_ENTY_FULL_NM], ");
            sbSQL.Append("u.[Enrollee_First_Name],");
            sbSQL.Append("u.[Enrollee_Last_Name], ");
            sbSQL.Append("u.[Cardholder_ID], ");
            sbSQL.Append("CONVERT(VARCHAR(10), u.[Member_Date_of_Birth], 101) as Member_Date_of_Birth, ");
            sbSQL.Append("u.[Procedure_Code_Description], ");
            sbSQL.Append("u.[Primary_Diagnosis_Code] ");
            sbSQL.Append("FROM[VCT_DB].[mhp].[MHP_Yearly_Universes] u ");
            sbSQL.Append("INNER JOIN [VCT_DB].[mhp].[MHP_Yearly_Universes_UGAP] c ON c.[mhp_uni_id] = u.[mhp_uni_id] ");
            sbSQL.Append("WHERE u.[State_of_Issue]  in ('" + String.Join(",", strState).Replace(",", "','") + "')  AND u.[Request_Date] >= '" + strStartDate + "' AND  u.[Request_Date] <= '" + strEndDate + "' "); //
            sbSQL.Append("AND c.[MKT_SEG_RLLP_DESC] in ('" + String.Join(",", strMKT_SEG_RLLP_DESC).Replace(",", "','") + "') AND  c.[FINC_ARNG_DESC] in ('" + String.Join(",", strFINC_ARNG_DESC).Replace(",", "','") + "')  AND [Authorization] IS NOT NULL AND (file_name LIKE 'UnitedPCP%') AND [sheet_name]<> 'U12'  "); //
            sbSQL.Append("AND c.[LEG_ENTY_NBR] = '" + legalNbr + "' "); //
            if (strMKT_TYP_DESC != null)
                sbSQL.Append("AND c.[MKT_TYP_DESC] in ('" + String.Join(",", strMKT_TYP_DESC).Replace(",", "','") + "') ");
            sbSQL.Append("UNION ALL ");
        }

        var results = _db.LoadData<MPHUniverseDetails_Model>(sql: sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '), token, connectionId: "VCT_DB");

        return results;
    }



    public Task<IEnumerable<MHP_Reporting_Filters>> GetMHP_Filters_Async(CancellationToken token)
    {

        string strSQL = "SELECT [Filter_Value],[Filter_Type],[Report_Type] FROM [VCT_DB].[mhp].[MHP_Universes_Filter_Cache];";

        var results = _db.LoadData<MHP_Reporting_Filters>(sql: strSQL, token, connectionId: "VCT_DB");

        return results;
    }

    public Task<IEnumerable<MHP_Group_State_Model>> GetMHP_Group_State_Async(CancellationToken token)
    {

        string strSQL = "SELECT [State_of_Issue],[Group_Number] FROM [VCT_DB].[mhp].[MHP_Group_State];";

        var results = _db.LoadData<MHP_Group_State_Model>(sql: strSQL, token, connectionId: "VCT_DB");

        return results;
    }

}