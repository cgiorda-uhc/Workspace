using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class IR_DataScrubber
{
    //PUBLIC PARAMETERS
    public static string strCurrentLOB = null;
    public static string strCurrentMainTable = null;
    public static string strPrimarySpecs = null;
    public static List<string> lstStrDegree = null;
    public static List<string> lstStrSuffix = null;

    //REUSABLE ILUCA SCRIPTS
    //REUSABLE ILUCA SCRIPTS
    //REUSABLE ILUCA SCRIPTS
    //MEMBER CACHE IS VERY GENERIC SO REUSE IT
    public static string str_ILUCA_ACO_Member_Cache_Batch_Update = "DECLARE @Rows int = 0; IF OBJECT_ID('tempdb..#Member_Cache_Updated') IS NOT NULL DROP TABLE #Member_Cache_Updated; CREATE TABLE #Member_Cache_Updated ( {$mem_cache_columns}); IF OBJECT_ID('tempdb..#Member_Missing') IS NOT NULL DROP TABLE #Member_Missing; CREATE TABLE #Member_Missing ([INDV_SYS_ID] [int] NULL, [SBSCR_MEDCD_RCIP_NBR] [varchar](50) NULL, [SBSCR_NBR] [varchar](50) NULL, [hicnbr] [varchar](50) NULL, [member_ID] [varchar](20) NULL, [MBR_FST_NM] [varchar](50) NULL, [MBR_LST_NM] [varchar](150) NULL, [BTH_DT] [date] NULL, [EFF_DT] [date] NULL, [END_DT] [date] NULL); INSERT INTO #Member_Missing (INDV_SYS_ID,SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR, hicnbr, member_ID, MBR_FST_NM, MBR_LST_NM, BTH_DT, EFF_DT, END_DT) SELECT DISTINCT a.INDV_SYS_ID, a.SBSCR_MEDCD_RCIP_NBR, a.SBSCR_NBR, a.hicnbr,a.member_ID,a.MBR_FST_Nm, a.MBR_LST_NM, a.BTH_DT, a.EFF_DT, a.END_DT from ( SELECT t1.MBR_FST_Nm as MBR_FST_Nm, t1.MBR_LST_NM as MBR_LST_NM, t1.BTH_DT, t1.SBSCR_MEDCD_RCIP_NBR as SBSCR_MEDCD_RCIP_NBR, t1.SBSCR_NBR as SBSCR_NBR, t1.hicnbr,t1.member_ID, t2.INDV_SYS_ID, t1.EFF_DT, t2.END_DT AS END_DT FROM ( select MAX(EFF_DT) as EFF_DT, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR, m.hicnbr,m.member_ID, m.MBR_FST_Nm,m.MBR_LST_NM,m.BTH_DT from ACO_Member_Cache m where m.is_matched = 0 and m.LOB = '{$LOB}' and m.data_source in ({$data_source}) GROUP BY m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR,m.hicnbr,m.member_ID, m.MBR_FST_Nm,m.MBR_LST_NM,m.BTH_DT ) t1 inner join ( select /*distinct*/ m.INDV_SYS_ID, EFF_DT, END_DT, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR, m.hicnbr,m.member_ID, m.MBR_FST_Nm,m.MBR_LST_NM,m.BTH_DT from ACO_Member_Cache m where m.is_matched = 0 and m.LOB = '{$LOB}' and m.data_source in ({$data_source}) ) t2 on (t2.EFF_DT = t1.EFF_DT OR (t2.EFF_DT IS NULL AND t1.EFF_DT IS NULL)) AND (t2.SBSCR_MEDCD_RCIP_NBR = t1.SBSCR_MEDCD_RCIP_NBR OR (t2.SBSCR_MEDCD_RCIP_NBR IS NULL AND t1.SBSCR_MEDCD_RCIP_NBR IS NULL )) AND (t2.SBSCR_NBR = t1.SBSCR_NBR OR (t2.SBSCR_NBR IS NULL AND t1.SBSCR_NBR IS NULL)) AND (t2.MBR_FST_Nm = t1.MBR_FST_Nm OR (t2.MBR_FST_Nm IS NULL AND t1.MBR_FST_Nm IS NULL )) AND (t2.MBR_LST_NM = t1.MBR_LST_NM OR (t2.MBR_LST_NM IS NULL AND t1.MBR_LST_NM IS NULL)) AND (t2.BTH_DT = t1.BTH_DT OR (t2.BTH_DT IS NULL AND t1.BTH_DT IS NULL)) AND (t2.hicnbr = t1.hicnbr OR (t2.hicnbr IS NULL AND t1.hicnbr IS NULL)) AND (t2.member_ID = t1.member_ID OR (t2.member_ID IS NULL AND t1.member_ID IS NULL)) ) as a; Create NonClustered Index TMP_INDX_Member_ACO_Missing_All On #Member_Missing (INDV_SYS_ID,SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR, hicnbr, member_ID, MBR_FST_NM, MBR_LST_NM, BTH_DT, EFF_DT, END_DT);update a set a.is_matched = 1, a.update_date = getdate(){$update_insert} INTO #Member_Cache_Updated from dbo.ACO_Member_Cache as a {$table_to_update} inner join ( SELECT MBR_FST_Nm, MBR_LST_NM, BTH_DT, SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR SBSCR_NBR,hicnbr, member_ID, INDV_SYS_ID, EFF_DT, MAX(END_DT) AS END_DT FROM #Member_Missing GROUP BY MBR_FST_Nm, MBR_LST_NM, BTH_DT, SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR,hicnbr, member_ID, INDV_SYS_ID, EFF_DT ) as c on (c.EFF_DT = a.EFF_DT OR (c.EFF_DT IS NULL AND a.EFF_DT IS NULL)) AND (c.END_DT = a.END_DT OR (c.END_DT IS NULL AND a.END_DT IS NULL)) AND (c.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR OR (c.SBSCR_MEDCD_RCIP_NBR IS NULL AND a.SBSCR_MEDCD_RCIP_NBR IS NULL)) AND (c.SBSCR_NBR = a.SBSCR_NBR OR (c.SBSCR_NBR IS NULL AND a.SBSCR_NBR IS NULL)) AND (c.MBR_FST_Nm = a.MBR_FST_Nm OR (c.MBR_FST_Nm IS NULL AND a.MBR_FST_Nm IS NULL )) AND (c.MBR_LST_NM = a.MBR_LST_NM OR (c.MBR_LST_NM IS NULL AND a.MBR_LST_NM IS NULL)) AND (c.BTH_DT = a.BTH_DT OR (c.BTH_DT IS NULL AND a.BTH_DT IS NULL)) AND (c.hicnbr = a.hicnbr OR (c.hicnbr IS NULL AND a.hicnbr IS NULL)) AND (c.member_ID = a.member_ID OR (c.member_ID IS NULL AND a.member_ID IS NULL)) where a.is_matched = 0 and a.LOB = '{$LOB}' and data_source in ({$data_source});update b set b.INDV_SYS_ID = a.INDV_SYS_ID from {$table_to_update2} where b.INDV_SYS_ID is null; SET @Rows = @Rows + @@ROWCOUNT;  DELETE FROM ACO_Member_Cache WHERE is_matched = 0  and LOB = '{$LOB}' and data_source in ({$data_source}); SELECT @Rows;";


    //public static string str_ILUCA_ACO_Member_Cache_Batch_Update = "DECLARE @Rows int = 0; IF OBJECT_ID('tempdb..#Member_Cache_Updated') IS NOT NULL DROP TABLE #Member_Cache_Updated; CREATE TABLE #Member_Cache_Updated ( {$mem_cache_columns}); CREATE TABLE #Member_Missing ([INDV_SYS_ID] [int] NULL, [SBSCR_MEDCD_RCIP_NBR] [varchar](50) NULL, [SBSCR_NBR] [varchar](50) NULL, [hicnbr] [varchar](50) NULL, [member_ID] [varchar](20) NULL, [MBR_FST_NM] [varchar](50) NULL, [MBR_LST_NM] [varchar](150) NULL, [BTH_DT] [date] NULL, [EFF_DT] [date] NULL, [END_DT] [date] NULL); INSERT INTO #Member_Missing (INDV_SYS_ID,SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR, hicnbr, member_ID, MBR_FST_NM, MBR_LST_NM, BTH_DT, EFF_DT, END_DT) SELECT /*DISTINCT*/ a.INDV_SYS_ID, a.SBSCR_MEDCD_RCIP_NBR, a.SBSCR_NBR, a.hicnbr,a.member_ID,a.MBR_FST_Nm, a.MBR_LST_NM, a.BTH_DT, a.EFF_DT, a.END_DT from ( SELECT t1.MBR_FST_Nm as MBR_FST_Nm, t1.MBR_LST_NM as MBR_LST_NM, t1.BTH_DT, t1.SBSCR_MEDCD_RCIP_NBR as SBSCR_MEDCD_RCIP_NBR, t1.SBSCR_NBR as SBSCR_NBR, t1.hicnbr,t1.member_ID, t2.INDV_SYS_ID, t1.EFF_DT, t2.END_DT AS END_DT FROM ( select MAX(EFF_DT) as EFF_DT, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR, m.hicnbr,m.member_ID, m.MBR_FST_Nm,m.MBR_LST_NM,m.BTH_DT from ACO_Member_Cache m where m.is_matched = 0 and m.LOB = '{$LOB}' and m.data_source in ({$data_source}) GROUP BY m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR,m.hicnbr,m.member_ID, m.MBR_FST_Nm,m.MBR_LST_NM,m.BTH_DT ) t1 inner join ( select /*distinct*/ m.INDV_SYS_ID, EFF_DT, END_DT, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR, m.hicnbr,m.member_ID, m.MBR_FST_Nm,m.MBR_LST_NM,m.BTH_DT from ACO_Member_Cache m where m.is_matched = 0 and m.LOB = '{$LOB}' and m.data_source in ({$data_source}) ) t2 on (t2.EFF_DT = t1.EFF_DT OR (t2.EFF_DT IS NULL AND t1.EFF_DT IS NULL)) AND (t2.SBSCR_MEDCD_RCIP_NBR = t1.SBSCR_MEDCD_RCIP_NBR OR (t2.SBSCR_MEDCD_RCIP_NBR = t1.SBSCR_MEDCD_RCIP_NBR )) AND (t2.SBSCR_NBR = t1.SBSCR_NBR OR (t2.SBSCR_NBR IS NULL AND t1.SBSCR_NBR IS NULL)) AND (t2.MBR_FST_Nm = t1.MBR_FST_Nm OR (t2.MBR_FST_Nm IS NULL AND t1.MBR_FST_Nm IS NULL )) AND (t2.MBR_LST_NM = t1.MBR_LST_NM OR (t2.MBR_LST_NM IS NULL AND t1.MBR_LST_NM IS NULL)) AND (t2.BTH_DT = t1.BTH_DT OR (t2.BTH_DT IS NULL AND t1.BTH_DT IS NULL)) AND (t2.hicnbr = t1.hicnbr OR (t2.hicnbr IS NULL AND t1.hicnbr IS NULL)) AND (t2.member_ID = t1.member_ID OR (t2.member_ID IS NULL AND t1.member_ID IS NULL)) ) as a; Create NonClustered Index TMP_INDX_Member_ACO_Missing_All On #Member_Missing (INDV_SYS_ID,SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR, hicnbr, member_ID, MBR_FST_NM, MBR_LST_NM, BTH_DT, EFF_DT, END_DT);update a set a.is_matched = 1, a.update_date = getdate(){$update_insert} INTO #Member_Cache_Updated from dbo.ACO_Member_Cache as a {$table_to_update} inner join ( SELECT MBR_FST_Nm, MBR_LST_NM, BTH_DT, SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR SBSCR_NBR,hicnbr, member_ID, INDV_SYS_ID, EFF_DT, MAX(END_DT) AS END_DT FROM #Member_Missing GROUP BY MBR_FST_Nm, MBR_LST_NM, BTH_DT, SBSCR_MEDCD_RCIP_NBR, SBSCR_NBR,hicnbr, member_ID, INDV_SYS_ID, EFF_DT ) as c on (c.EFF_DT = a.EFF_DT OR (c.EFF_DT IS NULL AND a.EFF_DT IS NULL)) AND (c.END_DT = a.END_DT OR (c.END_DT IS NULL AND a.END_DT IS NULL)) AND (c.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR OR (c.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR )) AND (c.SBSCR_NBR = a.SBSCR_NBR OR (c.SBSCR_NBR IS NULL AND a.SBSCR_NBR IS NULL)) AND (c.MBR_FST_Nm = a.MBR_FST_Nm OR (c.MBR_FST_Nm IS NULL AND a.MBR_FST_Nm IS NULL )) AND (c.MBR_LST_NM = a.MBR_LST_NM OR (c.MBR_LST_NM IS NULL AND a.MBR_LST_NM IS NULL)) AND (c.BTH_DT = a.BTH_DT OR (c.BTH_DT IS NULL AND a.BTH_DT IS NULL)) AND (c.hicnbr = a.hicnbr OR (c.hicnbr IS NULL AND a.hicnbr IS NULL)) AND (c.member_ID = a.member_ID OR (c.member_ID IS NULL AND a.member_ID IS NULL)) where a.is_matched = 0 and a.LOB = '{$LOB}' and data_source in ({$data_source});update b set b.INDV_SYS_ID = a.INDV_SYS_ID from {$table_to_update2} where b.INDV_SYS_ID is null; SET @Rows = @Rows + @@ROWCOUNT; SELECT @Rows;";


    //PROVIDER CACHE IS TRICKIER BUT REUSE IT ANYWAY :)
    public static string str_ILUCA_ACO_Provider_Cache_Batch_Update = "DECLARE @Rows int = 0;IF OBJECT_ID('tempdb..#Provider_Cache_Updated') IS NOT NULL DROP TABLE #Provider_Cache_Updated; CREATE TABLE #Provider_Cache_Updated ([MPIN] [int] NULL {$tmp_create_upt_columns}); IF OBJECT_ID('tempdb..#Provider_Missing') IS NOT NULL DROP TABLE #Provider_Missing; CREATE TABLE #Provider_Missing ([MPIN] [int] NULL {$tmp_create_miss_columns}); INSERT INTO #Provider_Missing (MPIN {$tmp_columns} ) SELECT distinct MPIN {$tmp_insert_columns} FROM dbo.{$main_table_name} WHERE MPIN IS NULL AND  {$missing_table_filters}; Create NonClustered Index TMP_INDX_Provider_ACO_Missing_All On #Provider_Missing (MPIN {$tmp_columns});UPDATE dbo.ACO_Provider_Cache SET PROV_FST_NM_CLN = dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'), PROV_LST_NM_CLN = dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]') WHERE is_matched = 0  and LOB = '{$LOB}' and data_source in ({$data_source}); {$main_updates_here} DELETE FROM ACO_Provider_Cache WHERE is_matched = 0  and LOB = '{$LOB}' and data_source in ({$data_source}); SELECT @Rows;";

    //public static string str_ILUCA_ACO_Provider_Cache_Batch_Update = "DECLARE @Rows int = 0;IF OBJECT_ID('tempdb..#Provider_Cache_Updated') IS NOT NULL DROP TABLE #Provider_Cache_Updated; CREATE TABLE #Provider_Cache_Updated ([MPIN] [int] NULL {$tmp_create_upt_columns}); IF OBJECT_ID('tempdb..#Provider_Missing') IS NOT NULL DROP TABLE #Provider_Missing; CREATE TABLE #Provider_Missing ([MPIN] [int] NULL, [data_source] [varchar](50) NULL {$tmp_create_miss_columns}); INSERT INTO #Provider_Missing (MPIN,data_source {$tmp_columns} ) SELECT MPIN,data_source {$tmp_insert_columns} FROM dbo.ACO_Provider_Cache WHERE LOB = '{$LOB}' AND data_source in ({$data_source}) and is_matched = 0; Create NonClustered Index TMP_INDX_Provider_ACO_Missing_All On #Provider_Missing (MPIN,data_source {$tmp_columns}); {$main_updates_here}  SELECT @Rows;";


    public static string str_ILUCA_ACO_Provider_MissingCache_Update_Statement = "update a set a.is_matched = 1, a.update_date = getdate(), a.MPIN = a.MPIN {$update_output_columns} OUTPUT INSERTED.MPIN {$insert_output_columns} INTO #Provider_Cache_Updated from dbo.ACO_Provider_Cache as a inner join #Provider_Missing as b on {$tmp_missing_join} where a.is_matched = 0 and a.LOB = '{$LOB}' and a.data_source in ({$data_source}) ;";
    public static string str_ILUCA_ACO_Provider_MissingACO_Update_Statement = "update a set a.mpin = b.mpin from dbo.{$main_table_name} as a inner join #Provider_Cache_Updated as b on {$tmp_missing_join} where a.mpin is null;";
    public static string str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement = "Create NonClustered Index TMP_INDX_Provider_Cache_Updated_{$tmp_index_name} On #Provider_Cache_Updated ({$tmp_index_columns});";
    public static string str_ILUCA_ACO_Provider_Missing_Cleanup_Statement = "SET @Rows = @Rows + @@ROWCOUNT; DELETE FROM #Provider_Cache_Updated;";

    //REUSEABLE TSQL TEMPLATES
    //REUSEABLE TSQL TEMPLATES
    //TSQL TRANSACTION/ERROR HANDLING USING {$tsql_here}
    public static string strTSQL_ErrorHandlingTemplate = "BEGIN TRANSACTION;BEGIN TRY {$tsql_here} END TRY BEGIN CATCH SELECT ERROR_NUMBER() AS ErrorNumber ,ERROR_SEVERITY() AS ErrorSeverity ,ERROR_STATE() AS ErrorState ,ERROR_PROCEDURE() AS ErrorProcedure ,ERROR_LINE() AS ErrorLine ,ERROR_MESSAGE() AS ErrorMessage;IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;END CATCH; IF @@TRANCOUNT > 0 COMMIT TRANSACTION;";

    //REUSABLE UHN SEARCHES UTILIZING <filter_{$x}_loop>
    //REUSABLE UHN SEARCHES UTILIZING <filter_{$x}_loop>
    //REUSABLE UHN SEARCHES UTILIZING <filter_{$x}_loop>
    public static string strSQL_UHN_GENERIC_PROVIDER_SEARCH = "CREATE TABLE #MissingProvidersTmp ( ID int IDENTITY(1,1), {$tmp_create_upt_columns} ) <filter_1_loop> INSERT INTO #MissingProvidersTmp({$tmp_columns}) VALUES ({$tmp_insert_values})</filter_1_loop> CREATE CLUSTERED INDEX IDX_MissingProvidersTmp_All ON #MissingProvidersTmp({$tmp_columns}) SELECT  DISTINCT p.MPIN, p.FirstName, p.LastName, '{$LOB}' as LOB,  {$data_source_case} as data_source, '{$notes}' as notes {$extra_cols} FROM (SELECT * FROM dbo.PROVIDER {$PrSpecs}) as p {$joins}";


    //REUSABLE UGAP SEARCHES UTILIZING <filter_{$x}_loop>
    //REUSABLE UGAP SEARCHES UTILIZING <filter_{$x}_loop>
    //REUSABLE UGAP SEARCHES UTILIZING <filter_{$x}_loop>
    public static string strSQL_UGAP_GENERIC_MEMBER_SEARCH = "CREATE MULTISET VOLATILE TABLE MissingMembersTmp ({$vtt_create_columns}) PRIMARY INDEX ({$vtt_columns}) ON COMMIT PRESERVE ROWS;{$vti} {$vtt_insert} {$vtc} COLLECT STATS COLUMN({$vtt_columns}) ON MissingMembersTmp;{$vts} SELECT m.INDV_SYS_ID, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR,m.HICN as hicnbr,m.mbr_ssn as member_id, m.MBR_FST_Nm, m.MBR_LST_NM, m.BTH_DT, coalesce (CAST ( de.EFF_DT as date) , CAST( '0001-01-01' as date)) AS EFF_DT, coalesce (CAST ( dn.END_DT as date) , CAST( '0001-01-01' as date))AS END_DT, coalesce (CAST ( m.LOAD_DT as date) , CAST( '0001-01-01' as date)) AS DATA_SOURCE_LOAD_DT, coalesce (CAST ( m.UPDT_DT as date) , CAST( '0001-01-01' as date)) AS DATA_SOURCE_UPDT_DT, '{$notes}' as notes, '{$data_source}' data_source, '{$LOB}' as LOB FROM (SELECT * FROM UHCDM001.HP_member WHERE INDV_SYS_ID > 0  ) as m left join UHCDM001.CS_ENROLLMENT as e on m.MBR_SYS_ID=e.MBR_SYS_ID left join DATE_EFF as de on de.EFF_DT_SYS_ID=e.EFF_DT_SYS_ID left join DATE_END as dn on dn.END_DT_SYS_ID=e.END_DT_SYS_ID {$final_filter};{$dvt} drop table MissingMembersTmp;";

    public static string strSQL_UGAP_GENERIC_PROVIDER_SEARCH = "CREATE MULTISET VOLATILE TABLE PotentialProvidersTmp ({$vtt_create_columns}) PRIMARY INDEX ({$vtt_columns}) ON COMMIT PRESERVE ROWS;{$vti} {$vtt_insert} {$vtc} COLLECT STATS COLUMN({$vtt_columns}) ON PotentialProvidersTmp;{$vts} SELECT p.MPIN,p.PROV_FST_NM, p.PROV_LST_NM, '{$LOB}' as LOB, {$data_source_case} as data_source, '{$notes}' as notes {$extra_cols} {$main_query} {$final_filter};{$dvt} drop table PotentialProvidersTmp;";

    public static string strSQL_UGAP_PROVIDER_TABLE_BY_MEMBER = "FROM UHCDM001.HP_MEMBER as m inner join UHCDM001.CS_ENROLLMENT as e on m.MBR_SYS_ID=e.MBR_SYS_ID inner join ( SELECT distinct MBR_PRI_PROV_MPIN as mpin, MBR_PRI_PROV_LST_NM as PROV_LST_NM, MBR_PRI_PROV_FST_NM as PROV_FST_NM, MBR_PRI_PROV_DEA_NBR as DEA_NBR, MBR_PRI_PROV_TIN as TIN, MBR_PRI_UNIQ_PROV_SYS_ID as PROV_SYS_ID, CAST(NULL AS VARCHAR(11)) AS PROV_ID FROM UHCDM001.UNIQUE_PROVIDER_MBR_PRI WHERE mpin <> 0 UNION SELECT distinct mpin, PROV_LST_NM, PROV_FST_NM, DEA_NBR, CAST(TIN as INT) as TIN, PROV_SYS_ID as PROV_SYS_ID , PROV_ID FROM UHCDM001.PROVIDER WHERE mpin <> 0 ) as p on e.MBR_MED_PRI_PHYSN_PROV_SYS_ID= p.PROV_SYS_ID";

    public static string strSQL_UGAP_PROVIDER_TABLE_BY_PROVIDER = "FROM UHCDM001.PROVIDER p";
    

    //USED TO IDENTIFY DB VIA KEYWORDS IN CONNECTIONSTRINGS PASSED IN VIA processQuerySet(DataTable dtQueries
    private const string strNDBCSIdentifier = "UHN_Reporting";
    private const string strILUCACSIdentifier = "IL_UCA";
    private const string strUGAPCSIdentifier = "UDWPROD";


    //FOR CONSOLE FEEDBACK
    static string strMessageGlobal = null;
    //MAIN FUNCTION USED TO PROCESS SERACH AND MATCH MISSING ELEMENTS
    public static void processQuerySet(DataTable dtQueries, string strFilterTagGeneric = "<filter_{$x}_loop>")
    {
        string strFilterTag = null;
        string strSQLTagValue = null;
        string strFilterSQLFinal = null;
        string strNullMarker = null;
        int intSQLTagStart = 0;
        int intSQLTagEnd = 0;
        DataTable dtResults = new DataTable();
        DataTable dtMissing = new DataTable();
        DataTable dtDistinct = new DataTable();
        int intFilterLimit;
        int intFilterCount = 0;
        StringBuilder sbFilterSQL = new StringBuilder();
        string strFinalSQL = null;
        bool blFirstPass = true;
        int intTotalResultCnt = 0;
        int intTotalMissingRowCnt;
        int intQueryCounter = 0;
        string strSQL = null;
        int intCnt = 0;
        int intResultCnt = 0;
        DateTime dtTotalStartTime, dtTotalEndTime, dtQueryStartTime, dtQueryEndTime;
        TimeSpan tsTimeSpanFinal;

        dtTotalStartTime = DateTime.Now;


        DataSet ds_RowsToDelete_GLOBAL = new DataSet();
        foreach (DataRow drQ in dtQueries.Select("Exclude=False"))
        {
            strFinalSQL = drQ["SearchSQL"].ToString(); //USE THIS + FILTERS FOR DB EXECUTION
            intFilterLimit = int.Parse(drQ["SearchLimit"].ToString());
            blFirstPass = true;
            intQueryCounter++;

            dtQueryStartTime = DateTime.Now;
            //COLLECT MISSING MPINS VIA MISSING SQL
            Console.WriteLine("--Query {0:n0} : Search for missing {1}...", intQueryCounter, drQ["MissingSearchDesc"].ToString());
            strSQL = drQ["MissingSQL"].ToString();
            if (drQ["MissingConnectionString"].ToString().Contains(strILUCACSIdentifier))
                dtMissing = DBConnection32.getMSSQLDataTable(drQ["MissingConnectionString"].ToString(), strSQL);
            else if (drQ["MissingConnectionString"].ToString().Contains(strNDBCSIdentifier))
                dtMissing = DBConnection32.getMSSQLDataTable(drQ["MissingConnectionString"].ToString(), strSQL);
            else if (drQ["MissingConnectionString"].ToString().Contains(strUGAPCSIdentifier))
                dtMissing = DBConnection32.getTeraDataDataTable(drQ["MissingConnectionString"].ToString(), strSQL);

            intTotalMissingRowCnt = dtMissing.Rows.Count;//GET TOTAL MISSING BEFORE THIS TABLE IS DIMINISHED

            //if(intTotalMissingRowCnt < 10000)
            //{
            //    continue;
            //}
            //else
            //{
            //    string s = "";
            //}


            intTotalResultCnt = 0;

            while (dtMissing.Rows.Count > 0)
            {
                //if(dtMissing.Rows.Count > 95000)
                //{
                //    //CLEAN OUT ROWS FROM dtMissing -- CLOSER TO while (dtMissing.Rows.Count > 0)
                //    deleteMissingDataTable(ref dtMissing, intFilterLimit);
                //    continue;
                //}


                //break; //SKIP SEARCH TO JUST RUN BATCH UPDATES

                for (int i = 1; i < 100000; i++)//VIRTUALLY UNLIMITED UNTIL NO MATCH FOUND ON <filter_?_loop>
                {
                    //CONVERT TO <filter_{$x}_loop> TO <filter_1..._loop> AND SEE IF EXISTS, IF NOT WERE DONE!
                    strFilterTag = strFilterTagGeneric.Replace("{$x}", i.ToString());
                    if (!drQ["SearchSQL"].ToString().Contains(strFilterTag))
                        break;

                    intSQLTagStart = drQ["SearchSQL"].ToString().IndexOf(strFilterTag) + strFilterTag.Length;//FIND OPENING TAG
                    intSQLTagEnd = drQ["SearchSQL"].ToString().LastIndexOf(strFilterTag.Replace("<", "</")); //FIND CLOSING TAG
                    strSQLTagValue = drQ["SearchSQL"].ToString().Substring(intSQLTagStart, intSQLTagEnd - intSQLTagStart);//GET FILTER WITH PLACEHOLDERS
                    strFilterSQLFinal = strSQLTagValue;//PREP FOR FIRST ROUND

                    //GET DISTINCT ROWS FOR FILTERS
                    dtDistinct = getDistinctColumnsForFilter(strFilterSQLFinal, dtMissing, intFilterLimit);

                    //FOR EACH FILTER TAG IN SQL ABOVE LOOP THE FILTERS VIA ACO_CS.MISSING
                    foreach (DataRow dr in dtDistinct.Rows)
                    {
                        //REPLACE PLACEHOLDERS WITH COLUMN VALUES
                        foreach (DataColumn dc in dr.Table.Columns)
                        {
                            //NO TICKS IN SQL CHARS FOR NULL VALUES
                            if (dr[dc.ColumnName] == DBNull.Value && (dc.DataType == System.Type.GetType("System.String") || dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.Date")))
                                strNullMarker = "1900-01-01";//THIS *SHOULD BE VALID AND FAIL ACROSS THE BOARD FOR STINGS - '?%'
                            else
                                strNullMarker = "NULL";

                            strFilterSQLFinal = strFilterSQLFinal.Replace("{$" + dc.ColumnName + "}", (dr[dc.ColumnName] != DBNull.Value ? dr[dc.ColumnName].ToString().Replace("'", "''") : strNullMarker));

                        }
                        //ADD TO SQL FILTER LIST
                        sbFilterSQL.Append(strFilterSQLFinal); //ADD TO FILTER COLLECTION
                        strFilterSQLFinal = strSQLTagValue;//RESET FOR ANOTHER ROUND
                        intFilterCount++;

                    }
                    //REPLACE TAG x WITH FILTERS
                    strFinalSQL = strFinalSQL.Replace(strFilterTag + strSQLTagValue + strFilterTag.Replace("<", "</"), sbFilterSQL.ToString().TrimEnd(" UNION ALL ".ToCharArray()).TrimEnd(' ', 'O', 'R').TrimEnd(' ', 'A', 'N', 'D').TrimEnd(','));
                    sbFilterSQL.Remove(0, sbFilterSQL.Length);//CLEAR FILTERS NOW THAT ADDED TO FINAL SQL
                    if (blFirstPass)
                    {
                        intCnt += intFilterCount;
                        blFirstPass = false;
                    }

                    intFilterCount = 0;//RESET FILTERS SO WE CAN MOVE TO NEXT TAG 

                }// for (int i = 1; i < 100000; i++)

                //COLLECT RETRIEVED MPINS VIA SEARCH SQL
                Console.Write("\r----Searching matches for rows {0:n0} out of {1:n0} - Cached Total = {2:n0}                                                                           ", intCnt, intTotalMissingRowCnt, intTotalResultCnt);
                strSQL = strFinalSQL;
                if (drQ["SearchConnectionString"].ToString().Contains(strILUCACSIdentifier))
                    dtResults = DBConnection32.getMSSQLDataTable(drQ["SearchConnectionString"].ToString(), strSQL.Replace("{$LOB}", strCurrentLOB).Replace("{$PrSpecs}", strPrimarySpecs));
                else if (drQ["SearchConnectionString"].ToString().Contains(strNDBCSIdentifier))
                    dtResults = DBConnection32.getMSSQLDataTable(drQ["SearchConnectionString"].ToString(), strSQL.Replace("{$LOB}", strCurrentLOB).Replace("{$PrSpecs}", strPrimarySpecs));
                else if (drQ["SearchConnectionString"].ToString().Contains(strUGAPCSIdentifier))
                    dtResults = DBConnection32.getTeraDataDataTable(drQ["SearchConnectionString"].ToString(), strSQL.Replace("{$LOB}", strCurrentLOB).Replace("{$PrSpecs}", strPrimarySpecs));


                //HADLE BULK INSERTS FOR CONSOLE FEEDBACK
                if(dtResults.Rows.Count > 0)
                {
                    strMessageGlobal = "\r----Caching potential matches for rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + "                                  ";
                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    dtResults.TableName = drQ["CachedTableName"].ToString();
                    DBConnection32.SQLServerBulkImportDT(dtResults, drQ["CachedInsertConnectionString"].ToString(), 500, false);
                    intTotalResultCnt += dtResults.Rows.Count;
                }


                Console.Write("\r----Prepping missing datatable...                                                                                                                       ");
                strFinalSQL = drQ["SearchSQL"].ToString(); //RESET FOR NEXT BATCH
                blFirstPass = true;

                //CLEAN OUT ROWS FROM dtMissing -- CLOSER TO while (dtMissing.Rows.Count > 0)
                deleteMissingDataTable(ref dtMissing, intFilterLimit);



            }//while (dtMissing.Rows.Count > 0)
            

            //INSERT RETRIEVED MPINS VIA CACHE INSERT SQL
            Console.WriteLine(""); //'RESET' CONSOLE
            Console.WriteLine("--Batch updating missing records...");
            strSQL = drQ["BatchUpdateSQL"].ToString();
            if (drQ["BatchUpdateConnectionString"].ToString().Contains(strILUCACSIdentifier))
                intResultCnt = DBConnection32.ExecuteMSSQLWithResults(drQ["BatchUpdateConnectionString"].ToString(), strSQL);
            else if (drQ["BatchUpdateConnectionString"].ToString().Contains(strNDBCSIdentifier))
                intResultCnt = DBConnection32.ExecuteMSSQLWithResults(drQ["BatchUpdateConnectionString"].ToString(), strSQL);
            else if (drQ["BatchUpdateConnectionString"].ToString().Contains(strUGAPCSIdentifier))//NEED TERADATA EXECUTE SOMEDAY!!!!!!!!!!!!!!!!!
                intResultCnt = DBConnection32.ExecuteTeraData(drQ["BatchUpdateConnectionString"].ToString(), strSQL);


            intCnt = 0;//RESET MISSING COUNTER
            intTotalResultCnt = 0; //RESET TOTAL CACHED RESULT COUNTER
            dtQueryEndTime = DateTime.Now;
            tsTimeSpanFinal = dtQueryEndTime.Subtract(dtQueryStartTime);
            Console.WriteLine("{0} missing rows updated. Process Completed in {1} ", intResultCnt, (tsTimeSpanFinal.Hours == 0 ? "" : tsTimeSpanFinal.Hours + "hr:") + (tsTimeSpanFinal.Minutes == 0 ? "" : tsTimeSpanFinal.Minutes + "min:") + (tsTimeSpanFinal.Seconds == 0 ? "" : tsTimeSpanFinal.Seconds + "sec"));


        }//foreach(DataRow drQ in dtQueries.Select("Exclude=False"))
        dtTotalEndTime = DateTime.Now;

        tsTimeSpanFinal = dtTotalEndTime.Subtract(dtTotalStartTime);
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine("Process completed in {0} ", (tsTimeSpanFinal.Hours == 0 ? "" : tsTimeSpanFinal.Hours + "hr:") + (tsTimeSpanFinal.Minutes == 0 ? "" : tsTimeSpanFinal.Minutes + "min:") + (tsTimeSpanFinal.Seconds == 0 ? "" : tsTimeSpanFinal.Seconds + "sec"));

    }




    private static DataTable getDistinctColumnsForFilter(string strFilters, DataTable dtFull, int intLimit = 10000000)
    {
        DataTable dtDistinct = dtFull.Rows.Cast<System.Data.DataRow>().Take(intLimit).CopyToDataTable();
        foreach (DataColumn dc in dtFull.Columns)
        {
            if (!strFilters.Contains("{$" + dc.ColumnName + "}"))
                dtDistinct.Columns.Remove(dc.ColumnName);
        }

        DataView view = new DataView(dtDistinct);
        return view.ToTable(true);
    }

   

    private static void deleteMissingDataTable(ref DataTable dtMissing, int intLimit = 10000000)
    {

        int intRowCnt = 0;
        List<DataRow> lDr = new List<DataRow>();
        foreach (DataRow drM in dtMissing.Rows)
        {
            lDr.Add(drM);
            intRowCnt++;
            if (intRowCnt == intLimit)
                break;
        }

        foreach(DataRow dr in lDr)
        {
            dr.Delete();
        }
        dtMissing.AcceptChanges();
    }


    //GLOBAL VARIABLES USED FOR NAME CLEANUP
    static string strProvFirstNameGlobal = null;
    static string strProvLastNameGlobal = null;
    static string strProvLastName2Global = null;
    static string strProvLastName3Global = null;
    static string strProvMiddleNameGlobal = null;
    static string strProvMiddleName2Global = null;
    static string strProvDegreeGlobal = null;
    static string strSuffixGlobal = null;
    //TAKES COLUMN OF NAMES AND SCRUBS EACH INTO PROPER BUCKETS FOR DEEPER SEARCHES
    public static void cleanupProviderNamesTmpTblUpdate(string strSQLSelect, string strSQLUpdate, string strConnectionString, int intUpdateLimit = 10000)
    {

        DateTime dtTotalStartTime, dtTotalEndTime;
        TimeSpan tsTimeSpanFinal;
        int intResultCnt = 0;
        int intTotalResultCnt = 0;
        StringBuilder sbSQL = new StringBuilder();
        DataTable dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQLSelect);
        int intTotalRows = dt.Rows.Count;
        int intTotalCnt = 0;
        int intUpdateCnt = 0;
        dtTotalStartTime = DateTime.Now;
        foreach (DataRow dr in dt.Rows)
        {
            //PARSE EACH NAME
            parseOutProviderName(dr[0].ToString());
            //BUILD SQL SELECT VIA PARSED NAME PARTS
            sbSQL.Append("SELECT ");
            sbSQL.Append("'" + dr[0].ToString().Replace("'", "''") + "' AS provider_name, ");
            sbSQL.Append((strProvFirstNameGlobal != null ? "'" + strProvFirstNameGlobal.Replace("'", "''") + "'" : "NULL") + " AS fn, ");
            sbSQL.Append((strProvLastNameGlobal != null ? "'" + strProvLastNameGlobal.Replace("'", "''") + "'" : "NULL") + " AS ln, ");
            sbSQL.Append((strProvLastName2Global != null ? "'" + strProvLastName2Global.Replace("'", "''") + "'" : "NULL") + " AS ln2, ");
            sbSQL.Append((strProvLastName3Global != null ? "'" + strProvLastName3Global.Replace("'", "''") + "'" : "NULL") + " AS ln3, ");
            sbSQL.Append((strProvMiddleNameGlobal != null ? "'" + strProvMiddleNameGlobal.Replace("'", "''") + "'" : "NULL") + " AS mn, ");
            sbSQL.Append((strProvMiddleName2Global != null ? "'" + strProvMiddleName2Global.Replace("'", "''") + "'" : "NULL") + " AS mn2, ");
            sbSQL.Append((strProvDegreeGlobal != null ? "'" + strProvDegreeGlobal.Replace("'", "''") + "'" : "NULL") + " AS provdgr, ");
            sbSQL.Append((strSuffixGlobal != null ? "'" + strSuffixGlobal.Replace("'", "''") + "'" : "NULL") + " AS suffix ");

            sbSQL.Append(" UNION ALL ");

            intTotalCnt++;
            intUpdateCnt++;
            //IF WE HIT THE UPDATE LIMIT OR WERE DONE, EXECUTE UPDATE
            if (intUpdateCnt == intUpdateLimit || intTotalCnt == intTotalRows)
            {
                //REPLACE UPDATE.{$table}" WITH SQL SELECT CREATED ABOVE AND EXECUTE...
                Console.Write("--Updating names for rows {0:n0} out of {1:n0} - Update Total = {2:n0}                                                                        \r", intTotalCnt, intTotalRows, intTotalResultCnt);
                intResultCnt = DBConnection32.ExecuteMSSQLWithResults(strConnectionString, strSQLUpdate.Replace("{$table}", "SELECT DISTINCT tmp.* FROM ("+ sbSQL.ToString().TrimEnd(" UNION ALL ".ToCharArray()) + ") as tmp"));

                if (intResultCnt > 0)//TALLY FOR CONSOLE FEEDBACK
                    intTotalResultCnt += intResultCnt;

                sbSQL.Remove(0, sbSQL.Length);
                intUpdateCnt = 0;
            }

        }
        dtTotalEndTime = DateTime.Now;
        tsTimeSpanFinal = dtTotalEndTime.Subtract(dtTotalStartTime);
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine("{0} rows updated. Process Completed in {1} ", intTotalResultCnt, (tsTimeSpanFinal.Hours == 0 ? "" : tsTimeSpanFinal.Hours + "hr:") + (tsTimeSpanFinal.Minutes == 0 ? "" : tsTimeSpanFinal.Minutes + "min:") + (tsTimeSpanFinal.Seconds == 0 ? "" : tsTimeSpanFinal.Seconds + "sec"));

    }


    //USED TO PARSE PAIR.ACO_Exec_REGISTRY_MR_UHC.provider_name
    //EX:ABALOS, M.D., ANNA TZEITEL PASCUAL
    //EX2:ASHRAF, ZUBAIR
    //EX3:AGUIRRE-KAIAMA, N.P., GLENDA LAGAZO
    //EX4:ANDERSON, J CHRISTOPHER
    //EX5:ALSAMARAI, M.D., SUSAN
    //EX6:ANDERSON-FOWLER, M.D., MARGO K.
    //EX7:ASHBAUGH, EMILY B

    //GET ALL POSSIBLE NAME PARTS
    static Regex rgxGlobal = new Regex("[^a-zA-Z0-9 -]");
    private static void parseOutProviderName(string strProviderName)
    {
        //SPLIT THE NAME VIA ' ' BLANKS
        string[] strArrName = rgxGlobal.Replace(strProviderName, "").Trim().Split(' ');
        string[] strArrNameCheck = null;
        strProvFirstNameGlobal = null;
        strProvLastNameGlobal = null;
        strProvLastName2Global = null;
        strProvLastName3Global = null;
        strProvMiddleNameGlobal = null;
        strProvMiddleName2Global = null;
        strProvDegreeGlobal = null;
        strSuffixGlobal = null;
        //DEPENDING ON LENGTH WE DROP INTO BUCKETS
        //lstStrSuffix AND lstStrDegree HELP WITH MIDDLE NAMES VS DEGREES/SUFFIXES
        switch (strArrName.Length)
        {
            case 1:
                strArrNameCheck = strProviderName.Split(',');
                if(strArrNameCheck.Length > 1)
                {
                    strProvFirstNameGlobal = strArrNameCheck[0];
                    strProvLastNameGlobal = strArrNameCheck[1];
                }
                else
                    strProvLastNameGlobal = strArrName[0];
                break;
            case 2:
                strProvFirstNameGlobal = strArrName[1];
                strProvLastNameGlobal = strArrName[0];
                break;
            case 3:
                strProvLastNameGlobal = strArrName[0];

                if (lstStrSuffix.Contains(strArrName[1].ToUpper()))
                    strSuffixGlobal = strArrName[1];
                else if (lstStrDegree.Contains(strArrName[1].ToUpper()))
                    strProvDegreeGlobal = strArrName[1];
                else
                    strProvFirstNameGlobal = strArrName[1];


                if (lstStrSuffix.Contains(strArrName[2].ToUpper()))
                    strSuffixGlobal = strArrName[2];
                else if (lstStrDegree.Contains(strArrName[2].ToUpper()))
                    strProvDegreeGlobal = strArrName[2];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[2];
                    else
                        strProvMiddleNameGlobal = strArrName[2];
                }
                break;
            case 4:
                strProvLastNameGlobal = strArrName[0];

                if (lstStrSuffix.Contains(strArrName[1].ToUpper()))
                    strSuffixGlobal = strArrName[1];
                else if (lstStrDegree.Contains(strArrName[1].ToUpper()))
                    strProvDegreeGlobal = strArrName[1];
                else
                    strProvLastName2Global = strArrName[1];


                if (lstStrSuffix.Contains(strArrName[2].ToUpper()))
                    strSuffixGlobal = strArrName[2];
                else if (lstStrDegree.Contains(strArrName[2].ToUpper()))
                    strProvDegreeGlobal = strArrName[2];
                else
                    strProvFirstNameGlobal = strArrName[2];


                if (lstStrSuffix.Contains(strArrName[3].ToUpper()))
                    strSuffixGlobal = strArrName[3];
                else if (lstStrDegree.Contains(strArrName[3].ToUpper()))
                    strProvDegreeGlobal = strArrName[3];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[3];
                    else
                        strProvMiddleNameGlobal = strArrName[3];
                }

                break;
            case 5:
                strProvLastNameGlobal = strArrName[0];

                if (lstStrSuffix.Contains(strArrName[1].ToUpper()))
                    strSuffixGlobal = strArrName[1];
                else if (lstStrDegree.Contains(strArrName[1].ToUpper()))
                    strProvDegreeGlobal = strArrName[1];
                else
                    strProvLastName2Global = strArrName[1];


                if (lstStrSuffix.Contains(strArrName[2].ToUpper()))
                    strSuffixGlobal = strArrName[2];
                else if (lstStrDegree.Contains(strArrName[2].ToUpper()))
                    strProvDegreeGlobal = strArrName[2];
                else
                    strProvFirstNameGlobal = strArrName[2];


                if (lstStrSuffix.Contains(strArrName[3].ToUpper()))
                    strSuffixGlobal = strArrName[3];
                else if (lstStrDegree.Contains(strArrName[3].ToUpper()))
                    strProvDegreeGlobal = strArrName[3];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[3];
                    else
                        strProvMiddleNameGlobal = strArrName[3];
                }

                if (lstStrSuffix.Contains(strArrName[4].ToUpper()))
                    strSuffixGlobal = strArrName[4];
                else if (lstStrDegree.Contains(strArrName[4].ToUpper()))
                    strProvDegreeGlobal = strArrName[4];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[4];
                    else if (strProvMiddleNameGlobal == null)
                        strProvMiddleNameGlobal = strArrName[4];
                    else
                        strProvMiddleName2Global = strArrName[4];
                }

                break;
            case 6:
                strProvLastNameGlobal = strArrName[0];

                if (lstStrSuffix.Contains(strArrName[1].ToUpper()))
                    strSuffixGlobal = strArrName[1];
                else if (lstStrDegree.Contains(strArrName[1].ToUpper()))
                    strProvDegreeGlobal = strArrName[1];
                else
                    strProvLastName2Global = strArrName[1];


                if (lstStrSuffix.Contains(strArrName[2].ToUpper()))
                    strSuffixGlobal = strArrName[2];
                else if (lstStrDegree.Contains(strArrName[2].ToUpper()))
                    strProvDegreeGlobal = strArrName[2];
                else
                    strProvFirstNameGlobal = strArrName[2];


                if (lstStrSuffix.Contains(strArrName[3].ToUpper()))
                    strSuffixGlobal = strArrName[3];
                else if (lstStrDegree.Contains(strArrName[3].ToUpper()))
                    strProvDegreeGlobal = strArrName[3];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[3];
                    else
                        strProvMiddleNameGlobal = strArrName[3];
                }

                if (lstStrSuffix.Contains(strArrName[4].ToUpper()))
                    strSuffixGlobal = strArrName[4];
                else if (lstStrDegree.Contains(strArrName[4].ToUpper()))
                    strProvDegreeGlobal = strArrName[4];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[4];
                    else if (strProvMiddleNameGlobal == null)
                        strProvMiddleNameGlobal = strArrName[4];
                    else
                        strProvMiddleName2Global = strArrName[4];
                }


                if (strProvMiddleName2Global == null)
                    strProvMiddleName2Global = strArrName[5];

                break;
            case 7:
                strProvLastNameGlobal = strArrName[0];

                if (lstStrSuffix.Contains(strArrName[1].ToUpper()))
                    strSuffixGlobal = strArrName[1];
                else if (lstStrDegree.Contains(strArrName[1].ToUpper()))
                    strProvDegreeGlobal = strArrName[1];
                else
                    strProvLastName2Global = strArrName[1];


                if (lstStrSuffix.Contains(strArrName[2].ToUpper()))
                    strSuffixGlobal = strArrName[2];
                else if (lstStrDegree.Contains(strArrName[2].ToUpper()))
                    strProvDegreeGlobal = strArrName[2];
                else
                    strProvLastName3Global = strArrName[2];



                if (lstStrSuffix.Contains(strArrName[3].ToUpper()))
                    strSuffixGlobal = strArrName[3];
                else if (lstStrDegree.Contains(strArrName[3].ToUpper()))
                    strProvDegreeGlobal = strArrName[3];
                else
                    strProvFirstNameGlobal = strArrName[3];


                if (lstStrSuffix.Contains(strArrName[4].ToUpper()))
                    strSuffixGlobal = strArrName[4];
                else if (lstStrDegree.Contains(strArrName[4].ToUpper()))
                    strProvDegreeGlobal = strArrName[4];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[4];
                    else
                        strProvMiddleNameGlobal = strArrName[4];
                }

                if (lstStrSuffix.Contains(strArrName[5].ToUpper()))
                    strSuffixGlobal = strArrName[5];
                else if (lstStrDegree.Contains(strArrName[5].ToUpper()))
                    strProvDegreeGlobal = strArrName[5];
                else
                {
                    if (strProvFirstNameGlobal == null)
                        strProvFirstNameGlobal = strArrName[5];
                    else if (strProvMiddleNameGlobal == null)
                        strProvMiddleNameGlobal = strArrName[5];
                    else
                        strProvMiddleName2Global = strArrName[5];
                }


                if (strProvMiddleName2Global == null)
                    strProvMiddleName2Global = strArrName[6];

                break;
            default:
                Console.WriteLine("YIKES! NEVER SEEN A NAME LIKE THIS BEFORE: " + strProviderName);
                Console.ReadKey();
                break;
        }
    }

    private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        Console.Write( strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
    }












}
