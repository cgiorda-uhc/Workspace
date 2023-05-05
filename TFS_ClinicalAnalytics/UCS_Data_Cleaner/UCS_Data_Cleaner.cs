using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_Data_Cleaner
{
    class UCS_Data_Cleaner
    {



        static void Main(string[] args)
        {
            string strConnectionStringFinalDatabase = ConfigurationManager.AppSettings["FinalDatabase"];
            string strDataFolderPath = ConfigurationManager.AppSettings["DataFolderPath"];
            string strMappingDataFile = ConfigurationManager.AppSettings["MappingDataFile"];

            Console.WriteLine("Open Mapping file...");
            SpreadsheetDocument wbDataMappingFile = SpreadsheetDocument.Open(strMappingDataFile, false);
            SpreadsheetDocument wbCurrentExcelFile;

            //truncate table[dbo].[UCS_KPI_FACT_DATA] ;
            //truncate table[dbo].[UCS_KPI_DIM_NAME];
            //truncate table[dbo].[UCS_KPI_DIM_BUSINESS_SEGMENT];
            //truncate table[dbo].[UCS_KPI_DIM_CASH_PROJECT] ;
            //truncate table[dbo].[UCS_KPI_DIM_ER_LEVEL];
            //truncate table[dbo].[UCS_KPI_DIM_FINC_ARNG];
            //truncate table[dbo].[UCS_KPI_DIM_Market];
            //truncate table[dbo].[UCS_KPI_DIM_Region];
            //truncate table[dbo].[UCS_KPI_DIM_PROGRAM];
            //truncate table[dbo].[UCS_KPI_DIM_STATE];


            DataTable dtStaging = DBConnection32.getMSSQLDataTable(strConnectionStringFinalDatabase, "SELECT * FROM UCS_KPI_STAGING WHERE 1=2");
            dtStaging.TableName = "UCS_KPI_STAGING";
            DataRow drStaging;
            DataTable dtCurrentDataMap;
            DataTable dtCurrentDataTable;
            List<DataRow> drCurrentDataMap;

            string strSheetName;
            string strFactTable;
            string strCurrentProgram;
            string strBusinessSegment;
            int intStartingDataRow;
            int intColumnRow;
            string[] strExcelColumnsArr;
            string[] strDBColumnsArr;

            string[] strNullableValues = new string[] { "#N/A", "N/A", "", null };

            Int64 intMainCnt = 1;
            Int64 intTotalCnt;
            int iCol = 0;
            string strCurrentDBColumn;
            string strCurrentValue;
            object objFinalValue;

            Console.WriteLine("Reading Directories...");
            foreach (var d in Directory.GetDirectories(strDataFolderPath))
            {
                strCurrentProgram = new DirectoryInfo(d).Name;

                dtCurrentDataMap = OpenXML.ReadAsDataTable(wbDataMappingFile, strCurrentProgram);

                Console.WriteLine("Current Program: " + strCurrentProgram);
                foreach (string fileName in Directory.GetFiles(d, "*.xls*"))
                {
                    Console.WriteLine("-------------------------------------------------------------------------------------------");
                    Console.WriteLine("Reading " + fileName + "...");
                    drCurrentDataMap = new List<DataRow>();
                    wbCurrentExcelFile = SpreadsheetDocument.Open(fileName, false);



                    foreach (DataRow row in dtCurrentDataMap.Rows)
                    {
                        if (new FileInfo(fileName).Name.Contains(row["File Name Prefix"].ToString()))
                        {
                            drCurrentDataMap.Add(row);
                        }
                    }

                    foreach (DataRow rows in drCurrentDataMap)
                    {


                        strSheetName = rows["Data Sheet Name"].ToString().Trim();
                        strFactTable = rows["DB Fact Table"].ToString().Trim();
                        strBusinessSegment = rows["Business Segment (LOB)"].ToString().Trim();
                        intColumnRow = int.Parse(rows["Column Name Row"].ToString());
                        intStartingDataRow = int.Parse(rows["Starting Data Row"].ToString());
                        strExcelColumnsArr = rows["Excle Columns to Include"].ToString().Split(',');
                        strDBColumnsArr = rows["Mapped DB Column Names (Map to col F)"].ToString().Split(',');


                        //FIRST OF MANY SPOT CHECKS!!!!!
                        if (strExcelColumnsArr.Count() != strDBColumnsArr.Count())
                            continue;

                        //MAIN DATA
                        Console.WriteLine("Converting sheet " + strSheetName + " into DataTable...");
                        dtCurrentDataTable = OpenXML.ReadAsDataTable(wbCurrentExcelFile, strSheetName, intColumnRow, intStartingDataRow);
                        intTotalCnt = dtCurrentDataTable.Rows.Count;
                        intMainCnt = 1;
                        foreach (DataRow dr in dtCurrentDataTable.Rows)
                        {
                            drStaging = dtStaging.NewRow();

                            //MAGIC HERE!!!!!
                            iCol = 0;
                           
                            drStaging["business_segment"] = strBusinessSegment;
                            drStaging["kpi_program"] = strCurrentProgram;
                            foreach (string strExcelCol in strExcelColumnsArr)
                            {
                                strCurrentDBColumn = strDBColumnsArr[iCol].Trim();
                                switch (strCurrentDBColumn)
                                {
                                    case "mpin":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (Int64.TryParse(strCurrentValue, out long number))
                                        {
                                            if (strCurrentValue == "0")
                                                objFinalValue = DBNull.Value;
                                            else
                                                objFinalValue = strCurrentValue;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }

                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "first_name":
                                    case "last_name":
                                        //SEARCH FROM "first_name" FOR COMPOSITE CHECK
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;

                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "market":
                                        //SEARCH FROM "first_name" FOR COMPOSITE CHECK
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;

                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "region":
                                        //SEARCH FROM "first_name" FOR COMPOSITE CHECK
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;

                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "state":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue) || strCurrentValue.Length != 2)
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;

                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "finc_arng":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "er_level":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "cash_project":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            objFinalValue = strCurrentValue;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;

                                    case "adj_claims":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (Int16.TryParse(strCurrentValue, out Int16 number))
                                        {
                                            objFinalValue = strCurrentValue;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "temp_count":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (int.TryParse(strCurrentValue, out int number))
                                        {
                                            objFinalValue = strCurrentValue;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "paid_year":
                                    case "paid_year_dos":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (int.TryParse(strCurrentValue, out int number))
                                        {
                                            if (strCurrentValue.Length != 4)
                                                objFinalValue = DBNull.Value;
                                            else
                                                objFinalValue = strCurrentValue;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "paid_month":
                                    case "paid_month_dos":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (Int16.TryParse(strCurrentValue, out Int16 number))
                                        {
                                            if (strCurrentValue.Length > 2)
                                                objFinalValue = DBNull.Value;
                                            else
                                                objFinalValue = strCurrentValue;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "rptg_eff_dt":
                                    case "rptg_eff_dt_place_hold":
                                    case "rptg_eff_rmv_dt":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else
                                        {
                                            if (int.TryParse(strCurrentValue, out int intVal))
                                            {
                                                objFinalValue = CleanerFunctions.FromExcelSerialDate(intVal).ToShortDateString();
                                            }
                                            else
                                            {
                                                objFinalValue = DBNull.Value;
                                            }
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "obsclm":
                                    case "inpclm":
                                    case "surgclm":
                                    case "edc_review":
                                    case "edc_adjust":
                                    case "down_adjust":
                                    case "inscope":
                                    case "free_standing_ind":
                                    case "obs_ind_for_savings":
                                    case "covid_clm_ind":
                                    case "ppr_ind":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (strCurrentValue == "Y" || strCurrentValue == "Yes" || strCurrentValue == "T" || strCurrentValue == "True" || strCurrentValue == "1")
                                        {
                                            objFinalValue = true;
                                        }
                                        else if (strCurrentValue == "N" || strCurrentValue == "No" || strCurrentValue == "F" || strCurrentValue == "False" || strCurrentValue == "0")
                                        {
                                            objFinalValue = false;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    case "savings_amt":
                                        strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
                                        if (strNullableValues.Contains(strCurrentValue))
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        else if (float.TryParse(strCurrentValue, out float number))
                                        {
                                            objFinalValue = strCurrentValue;
                                        }
                                        else
                                        {
                                            objFinalValue = DBNull.Value;
                                        }
                                        drStaging[strCurrentDBColumn] = objFinalValue;
                                        break;
                                    
                                    default:
                                        //ERROR EMAIL ???????
                                        break;
                                }

                                iCol++;


                            }
                            Console.Write("\rFormatting rows " + String.Format("{0:n0}", intMainCnt) + " out of " + String.Format("{0:n0}", intTotalCnt) + " ...");
                            //Console.WriteLine("Inserting " + intMainCnt + " out of " + intTotalCnt + " rows...");
                            dtStaging.Rows.Add(drStaging);

                          
                            intMainCnt++;

                        }

                        //BULK LOAD
                        Console.WriteLine("");
                        strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", intTotalCnt) + " into Staging...";
                        DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                        DBConnection32.ExecuteMSSQL(strConnectionStringFinalDatabase, "TRUNCATE TABLE dbo.UCS_KPI_STAGING;");
                        DBConnection32.SQLServerBulkImportDT(dtStaging, strConnectionStringFinalDatabase);

                        Console.WriteLine("");
                        Console.WriteLine("Insert new DIM rows...");
                        DBConnection32.ExecuteMSSQL(strConnectionStringFinalDatabase, "exec dbo.SP_UCS_KPI_STAGE_TO_DIM;");

                        Console.WriteLine("Insert new FACT rows...");
                        if (strFactTable.ToLower().Equals("data"))
                            DBConnection32.ExecuteMSSQL(strConnectionStringFinalDatabase, "exec dbo.SP_UCS_KPI_STAGE_TO_FACT_DATA;");
                        else
                            DBConnection32.ExecuteMSSQL(strConnectionStringFinalDatabase, "exec dbo.SP_UCS_KPI_STAGE_TO_FACT_SAVINGS;");

                        Console.WriteLine("***Finished processing " + strCurrentProgram +" Worksheet '" + strSheetName +"'");
                        dtStaging.Rows.Clear();

                    }
                }
            }

        }

        static string strMessageGlobal;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }

















        //static void Main(string[] args)
        //{
        //    string strConnectionStringFinalDatabase = ConfigurationManager.AppSettings["FinalDatabase"];
        //    string strDataFolderPath = ConfigurationManager.AppSettings["DataFolderPath"];
        //    string strMappingDataFile = ConfigurationManager.AppSettings["MappingDataFile"];

        //    Console.WriteLine("Open Mapping file...");
        //    SpreadsheetDocument wbDataMappingFile = SpreadsheetDocument.Open(strMappingDataFile, false);
        //    SpreadsheetDocument wbCurrentExcelFile;

        //    //truncate table[dbo].[UCS_KPI_FACT_DATA] ;
        //    //truncate table[dbo].[UCS_KPI_DIM_NAME];
        //    //truncate table[dbo].[UCS_KPI_DIM_BUSINESS_SEGMENT];
        //    //truncate table[dbo].[UCS_KPI_DIM_CASH_PROJECT] ;
        //    //truncate table[dbo].[UCS_KPI_DIM_ER_LEVEL];
        //    //truncate table[dbo].[UCS_KPI_DIM_FINC_ARNG];
        //    //truncate table[dbo].[UCS_KPI_DIM_Market];
        //    //truncate table[dbo].[UCS_KPI_DIM_Region];
        //    //truncate table[dbo].[UCS_KPI_DIM_PROGRAM];
        //    //truncate table[dbo].[UCS_KPI_DIM_STATE];


        //    DataTable dtCurrentDataMap;
        //    DataTable dtCurrentDataTable;
        //    List<DataRow> drCurrentDataMap;
        //    string strSheetName;
        //    string strFactTable;
        //    string strCurrentProgram;
        //    string strBusinessSegment;
        //    int intStartingDataRow;
        //    int intColumnRow;
        //    string[] strExcelColumnsArr;
        //    string[] strDBColumnsArr;

        //    string[] strNullableValues = new string[] { "#N/A", "N/A", "", null };

        //    Int16 intExecuteLimit = 1000;
        //    Int16 intExecuteCnt = 1;
        //    Int64 intMainCnt = 1;
        //    Int64 intTotalCnt;
        //    int iCol = 0;
        //    string strCurrentDBColumn;
        //    string strCurrentValue;
        //    string strSecondValue;
        //    string strFinalValue;
        //    string strInsert = "INSERT INTO UCS_KPI_FACT_{$table_name} ({$columns}) VALUES ({$values});";

        //    StringBuilder sbFinalSQL = new StringBuilder();
        //    StringBuilder sbFactRowInsertColumns = new StringBuilder();
        //    StringBuilder sbFactRowInsertValues = new StringBuilder();
        //    StringBuilder sbSQLDimInserts = new StringBuilder();
        //    StringBuilder sbSQLDeclare = new StringBuilder();

        //    sbSQLDeclare.Append("DECLARE @iMarket_Id SMALLINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iRegion_Id SMALLINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iName_Id BIGINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iBS_Id SMALLINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iFinc_Arng_Id SMALLINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iCash_Project_Id INT;  ");
        //    sbSQLDeclare.Append("DECLARE @iProgram_Id SMALLINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iState_Id SMALLINT;  ");
        //    sbSQLDeclare.Append("DECLARE @iER_Level_Id SMALLINT;  ");

        //    StringBuilder sbSQLDimNameInsert = new StringBuilder();
        //    sbSQLDimNameInsert.Append("SET @iName_Id  = (SELECT name_id FROM UCS_KPI_DIM_NAME {$name_set}); ");
        //    sbSQLDimNameInsert.Append("if @iName_Id IS NULL ");
        //    sbSQLDimNameInsert.Append("BEGIN ");
        //    sbSQLDimNameInsert.Append("INSERT INTO UCS_KPI_DIM_NAME ({$name_cols}) VALUES ({$name_vals}); ");
        //    sbSQLDimNameInsert.Append("SET @iName_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimNameInsert.Append("END ");

        //    StringBuilder sbSQLDimMarketInsert = new StringBuilder();
        //    sbSQLDimMarketInsert.Append("SET @iMarket_Id   = (SELECT market_id FROM UCS_KPI_DIM_Market WHERE  market = {$market_val}); ");
        //    sbSQLDimMarketInsert.Append("if @iMarket_Id IS NULL ");
        //    sbSQLDimMarketInsert.Append("BEGIN ");
        //    sbSQLDimMarketInsert.Append("INSERT INTO UCS_KPI_DIM_Market (market) VALUES ({$market_val}); ");
        //    sbSQLDimMarketInsert.Append("SET @iMarket_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimMarketInsert.Append("END ");

        //    StringBuilder sbSQLDimRegionInsert = new StringBuilder();
        //    sbSQLDimRegionInsert.Append("SET @iRegion_Id   = (SELECT Region_id FROM UCS_KPI_DIM_Region WHERE  Region = {$region_val}); ");
        //    sbSQLDimRegionInsert.Append("if @iRegion_Id IS NULL ");
        //    sbSQLDimRegionInsert.Append("BEGIN ");
        //    sbSQLDimRegionInsert.Append("INSERT INTO UCS_KPI_DIM_Region (region) VALUES ({$region_val}); ");
        //    sbSQLDimRegionInsert.Append("SET @iRegion_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimRegionInsert.Append("END ");


        //    StringBuilder sbSQLDimStateInsert = new StringBuilder();
        //    sbSQLDimStateInsert.Append("SET @iState_Id   = (SELECT State_id FROM UCS_KPI_DIM_State WHERE  State = {$state_val}); ");
        //    sbSQLDimStateInsert.Append("if @iState_Id IS NULL ");
        //    sbSQLDimStateInsert.Append("BEGIN ");
        //    sbSQLDimStateInsert.Append("INSERT INTO UCS_KPI_DIM_State (State) VALUES ({$state_val}); ");
        //    sbSQLDimStateInsert.Append("SET @iState_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimStateInsert.Append("END ");


        //    StringBuilder sbSQLDimFincArngInsert = new StringBuilder();
        //    sbSQLDimFincArngInsert.Append("SET @iFinc_Arng_Id   = (SELECT finc_arng_id FROM UCS_KPI_DIM_FINC_ARNG WHERE finc_arng = {$finc_arng_val}); ");
        //    sbSQLDimFincArngInsert.Append("if @iFinc_Arng_Id IS NULL ");
        //    sbSQLDimFincArngInsert.Append("BEGIN ");
        //    sbSQLDimFincArngInsert.Append("INSERT INTO UCS_KPI_DIM_FINC_ARNG (finc_arng) VALUES ({$finc_arng_val}); ");
        //    sbSQLDimFincArngInsert.Append("SET @iFinc_Arng_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimFincArngInsert.Append("END ");


        //    StringBuilder sbSQLDimERLevelInsert = new StringBuilder();
        //    sbSQLDimERLevelInsert.Append("SET @iER_Level_Id   = (SELECT er_level_id FROM UCS_KPI_DIM_ER_LEVEL WHERE er_level = {$er_level_val}); ");
        //    sbSQLDimERLevelInsert.Append("if @iER_Level_Id IS NULL ");
        //    sbSQLDimERLevelInsert.Append("BEGIN ");
        //    sbSQLDimERLevelInsert.Append("INSERT INTO UCS_KPI_DIM_ER_LEVEL (er_level) VALUES ({$er_level_val}); ");
        //    sbSQLDimERLevelInsert.Append("SET @iER_Level_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimERLevelInsert.Append("END ");


        //    StringBuilder sbSQLDimCashProjectInsert = new StringBuilder();
        //    sbSQLDimCashProjectInsert.Append("SET @iCash_Project_Id = (SELECT cash_project_id FROM UCS_KPI_DIM_CASH_PROJECT WHERE cash_project = {$cash_project_val}); ");
        //    sbSQLDimCashProjectInsert.Append("if @iCash_Project_Id IS NULL ");
        //    sbSQLDimCashProjectInsert.Append("BEGIN ");
        //    sbSQLDimCashProjectInsert.Append("INSERT INTO UCS_KPI_DIM_CASH_PROJECT (cash_project) VALUES ({$cash_project_val}); ");
        //    sbSQLDimCashProjectInsert.Append("SET @iCash_Project_Id = SCOPE_IDENTITY(); ");
        //    sbSQLDimCashProjectInsert.Append("END ");


        //    StringBuilder sbSQLDimBusinessSegmentInsert = new StringBuilder();
        //    sbSQLDimBusinessSegmentInsert.Append("SET @iBS_Id  = (SELECT bs_id FROM UCS_KPI_DIM_BUSINESS_SEGMENT WHERE business_segment = {$business_segment_val}); ");
        //    sbSQLDimBusinessSegmentInsert.Append("if @iBS_Id  IS NULL ");
        //    sbSQLDimBusinessSegmentInsert.Append("BEGIN ");
        //    sbSQLDimBusinessSegmentInsert.Append("INSERT INTO UCS_KPI_DIM_BUSINESS_SEGMENT (business_segment ) VALUES ({$business_segment_val}); ");
        //    sbSQLDimBusinessSegmentInsert.Append("SET @iBS_Id  = SCOPE_IDENTITY(); ");
        //    sbSQLDimBusinessSegmentInsert.Append("END ");


        //    StringBuilder sbSQLDimProgramInsert = new StringBuilder();
        //    sbSQLDimProgramInsert.Append("SET @iProgram_Id = (SELECT kpi_id FROM UCS_KPI_DIM_PROGRAM WHERE kpi_program = {$kpi_program_val}); ");
        //    sbSQLDimProgramInsert.Append("if @iProgram_Id  IS NULL ");
        //    sbSQLDimProgramInsert.Append("BEGIN ");
        //    sbSQLDimProgramInsert.Append("INSERT INTO UCS_KPI_DIM_PROGRAM (kpi_program) VALUES ({$kpi_program_val}); ");
        //    sbSQLDimProgramInsert.Append("SET @iProgram_Id  = SCOPE_IDENTITY(); ");
        //    sbSQLDimProgramInsert.Append("END ");

        //    Console.WriteLine("Reading Directories...");
        //    foreach (var d in Directory.GetDirectories(strDataFolderPath))
        //    {
        //        strCurrentProgram = new DirectoryInfo(d).Name;

        //        dtCurrentDataMap = OpenXML.ReadAsDataTable(wbDataMappingFile, strCurrentProgram);

        //        Console.WriteLine("Current Program: " + strCurrentProgram);
        //        foreach (string fileName in Directory.GetFiles(d,"*.xls*"))
        //        {
        //            Console.WriteLine("Reading " + fileName + "...");
        //            drCurrentDataMap = new List<DataRow>();
        //            wbCurrentExcelFile = SpreadsheetDocument.Open(fileName, false);



        //            foreach (DataRow row in dtCurrentDataMap.Rows )
        //            {
        //                if(new FileInfo(fileName).Name.Contains(row["Consistent File Name"].ToString()))
        //                {
        //                    drCurrentDataMap.Add(row);
        //                }
        //            }

        //            foreach (DataRow rows in drCurrentDataMap)
        //            {


        //                strSheetName = rows["Data Sheet Name"].ToString().Trim();
        //                strFactTable = rows["Fact Table"].ToString().Trim();
        //                strBusinessSegment = rows["Business Segment (LOB)"].ToString().Trim();
        //                intColumnRow = int.Parse(rows["Column Row"].ToString());
        //                intStartingDataRow = int.Parse(rows["Starting Data Row"].ToString());
        //                strExcelColumnsArr = rows["Excle Columns (Exclude Some????)"].ToString().Split(',');
        //                strDBColumnsArr = rows["Mapped DB Column Names (Map to col F)"].ToString().Split(',');


        //                //FIRST OF MANY SPOT CHECKS!!!!!
        //                if (strExcelColumnsArr.Count() != strDBColumnsArr.Count())
        //                    continue;

        //                //MAIN DATA
        //                Console.WriteLine("Converting sheet " + strSheetName + " into DataTable...");
        //                dtCurrentDataTable = OpenXML.ReadAsDataTable(wbCurrentExcelFile, strSheetName, intColumnRow, intStartingDataRow);
        //                intTotalCnt = dtCurrentDataTable.Rows.Count;
        //                foreach (DataRow dr in dtCurrentDataTable.Rows)
        //                {
        //                    if (intExecuteCnt == 1)
        //                    {
        //                        sbFinalSQL.Append(sbSQLDeclare.ToString());
        //                        sbSQLDimInserts.Append(sbSQLDimBusinessSegmentInsert.ToString().Replace("{$business_segment_val}", "'" + strBusinessSegment.Replace("'", "''") + "'"));
        //                        sbSQLDimInserts.Append(sbSQLDimProgramInsert.ToString().Replace("{$kpi_program_val}", "'" + strCurrentProgram.Replace("'", "''") + "'"));
        //                    }

        //                    //MAGIC HERE!!!!!
        //                    iCol = 0;
        //                    strFinalValue = "@iBS_Id";
        //                    sbFactRowInsertColumns.Append("bs_id,");
        //                    sbFactRowInsertValues.Append(strFinalValue + ",");

        //                    strFinalValue = "@iProgram_Id";
        //                    sbFactRowInsertColumns.Append("kpi_id,");
        //                    sbFactRowInsertValues.Append(strFinalValue + ",");
        //                    foreach (string strExcelCol in strExcelColumnsArr)
        //                    {
        //                        strCurrentDBColumn = strDBColumnsArr[iCol].Trim();
        //                        switch (strCurrentDBColumn)
        //                        {
        //                            case "mpin":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if(strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else if(Int64.TryParse(strCurrentValue, out long number))
        //                                {
        //                                    if(strCurrentValue == "0")
        //                                        strFinalValue = "NULL";
        //                                    else
        //                                        strFinalValue = strCurrentValue;
        //                                }
        //                                else
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                sbFactRowInsertColumns.Append(strCurrentDBColumn + ",");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "last_name":
        //                                //SEARCH FROM "first_name" FOR COMPOSITE CHECK
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else
        //                                {

        //                                    string name_set;
        //                                    string name_cols;
        //                                    string name_vals;

        //                                    var index = Array.FindIndex(strDBColumnsArr, x => x.Contains("first_name"));
        //                                    if (index != -1)
        //                                    {
        //                                        strSecondValue = (dr[index] + "").Trim();
        //                                        if (!strNullableValues.Contains(strSecondValue))
        //                                        {
        //                                            name_set = " WHERE first_name = '"+ strSecondValue.Replace("'","''") + "' AND last_name = '" + strCurrentValue.Replace("'", "''") + "'";
        //                                            name_cols = " first_name, last_name " ;
        //                                            name_vals = "'" + strSecondValue.Replace("'", "''") + "', '" + strCurrentValue.Replace("'", "''") + "'";
        //                                        }
        //                                        else
        //                                        {
        //                                            name_set = " WHERE last_name = '" + strCurrentValue.Replace("'", "''") + "'";
        //                                            name_cols = " last_name ";
        //                                            name_vals =  "'" + strCurrentValue.Replace("'", "''") + "'";
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        name_set = " WHERE last_name = '" + strCurrentValue.Replace("'", "''") + "'";
        //                                        name_cols = " last_name ";
        //                                        name_vals = "'" + strCurrentValue.Replace("'", "''") + "'";
        //                                    }

        //                                    sbSQLDimInserts.Append(sbSQLDimNameInsert.ToString().Replace("{$name_set}", name_set).Replace("{$name_cols}", name_cols).Replace("{$name_vals}", name_vals));
        //                                    strFinalValue = "@iName_Id";
        //                                }
        //                                sbFactRowInsertColumns.Append("name_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "market":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else
        //                                {
        //                                    sbSQLDimInserts.Append(sbSQLDimMarketInsert.ToString().Replace("{$market_val}", "'" + strCurrentValue.Replace("'", "''") + "'"));
        //                                    strFinalValue = "@iMarket_Id";
        //                                }
        //                                sbFactRowInsertColumns.Append("market_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "region":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else
        //                                {
        //                                    sbSQLDimInserts.Append(sbSQLDimRegionInsert.ToString().Replace("{$region_val}", "'" + strCurrentValue.Replace("'", "''") + "'"));
        //                                    strFinalValue = "@iRegion_Id";
        //                                }
        //                                sbFactRowInsertColumns.Append("region_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "state":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue) || strCurrentValue.Length != 2)
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else 
        //                                {
        //                                    sbSQLDimInserts.Append(sbSQLDimStateInsert.ToString().Replace("{$state_val}", "'" + strCurrentValue.Replace("'", "''") + "'"));
        //                                    strFinalValue = "@iState_Id";
        //                                }
        //                                sbFactRowInsertColumns.Append("state_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "finc_arng":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else
        //                                {
        //                                    sbSQLDimInserts.Append(sbSQLDimFincArngInsert.ToString().Replace("{$finc_arng_val}", "'" + strCurrentValue.Replace("'", "''") + "'"));
        //                                    strFinalValue = "@iFinc_Arng_Id  ";
        //                                }
        //                                sbFactRowInsertColumns.Append("finc_arng_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "er_level":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else
        //                                {
        //                                    sbSQLDimInserts.Append(sbSQLDimERLevelInsert.ToString().Replace("{$er_level_val}", "'" + strCurrentValue.Replace("'", "''") + "'"));
        //                                    strFinalValue = "@iER_Level_Id";
        //                                }
        //                                sbFactRowInsertColumns.Append("er_level_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "cash_project":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else
        //                                {
        //                                    sbSQLDimInserts.Append(sbSQLDimCashProjectInsert.ToString().Replace("{$cash_project_val}", "'" + strCurrentValue.Replace("'", "''") + "'"));
        //                                    strFinalValue = "@iCash_Project_Id";
        //                                }
        //                                sbFactRowInsertColumns.Append("cash_project_id,");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "temp_count":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else if (int.TryParse(strCurrentValue, out int number))
        //                                {
        //                                    strFinalValue = strCurrentValue;
        //                                }
        //                                else
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                sbFactRowInsertColumns.Append(strCurrentDBColumn + ",");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "paid_year":
        //                            case "paid_year_num":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else if (int.TryParse(strCurrentValue, out int number))
        //                                {
        //                                    if (strCurrentValue.Length != 4)
        //                                        strFinalValue = "NULL";
        //                                    else
        //                                        strFinalValue = strCurrentValue;
        //                                }
        //                                else
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                sbFactRowInsertColumns.Append(strCurrentDBColumn + ",");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "paid_month":
        //                            case "paid_month_num":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else if (Int16.TryParse(strCurrentValue, out Int16 number))
        //                                {
        //                                    if (strCurrentValue.Length > 2)
        //                                        strFinalValue = "NULL";
        //                                    else
        //                                        strFinalValue = strCurrentValue;
        //                                }
        //                                else
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                sbFactRowInsertColumns.Append(strCurrentDBColumn + ",");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "rptg_eff_dt":
        //                            case "rptg_eff_dt_place_hold":
        //                            case "rptg_eff_rmv_dt":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else 
        //                                { 
        //                                    if(int.TryParse(strCurrentValue, out int intVal))
        //                                    {
        //                                        strFinalValue = "'" + CleanerFunctions.FromExcelSerialDate(intVal).ToShortDateString() + "'";
        //                                    }
        //                                    else
        //                                    {
        //                                        strFinalValue = "NULL";
        //                                    }
        //                                }

        //                                sbFactRowInsertColumns.Append(strCurrentDBColumn + ",");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            case "obsclm":
        //                            case "down_adjust":
        //                            case "inscope":
        //                            case "free_standing_ind":
        //                            case "obs_ind_for_savings":
        //                            case "covid_clm_ind":
        //                                strCurrentValue = (dr[strExcelCol.Trim()] + "").Trim();
        //                                if (strNullableValues.Contains(strCurrentValue))
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                else if (strCurrentValue == "Y" || strCurrentValue == "Yes" || strCurrentValue == "T" || strCurrentValue == "True" || strCurrentValue == "1")
        //                                {
        //                                    strFinalValue = "1";
        //                                }
        //                                else if (strCurrentValue == "N" || strCurrentValue == "No" || strCurrentValue == "F" || strCurrentValue == "False" || strCurrentValue == "0")
        //                                {
        //                                    strFinalValue = "0";
        //                                }
        //                                else
        //                                {
        //                                    strFinalValue = "NULL";
        //                                }
        //                                sbFactRowInsertColumns.Append(strCurrentDBColumn + ",");
        //                                sbFactRowInsertValues.Append(strFinalValue + ",");
        //                                break;
        //                            default:
        //                                //ERROR EMAIL ???????
        //                                break;
        //                        }

        //                        iCol++;


        //                    }

        //                    sbFinalSQL.Append(sbSQLDimInserts.ToString());
        //                    sbFinalSQL.Append(strInsert.Replace("{$table_name}", strFactTable).Replace("{$columns}", sbFactRowInsertColumns.ToString().TrimEnd(',')).Replace("{$values}", sbFactRowInsertValues.ToString().TrimEnd(',')));
        //                    sbSQLDimInserts.Remove(0, sbSQLDimInserts.Length);
        //                    sbFactRowInsertColumns.Remove(0, sbFactRowInsertColumns.Length);
        //                    sbFactRowInsertValues.Remove(0, sbFactRowInsertValues.Length);

        //                    intExecuteCnt++;

        //                    if (intExecuteCnt > intExecuteLimit || intMainCnt == intTotalCnt )
        //                    {
        //                        Console.WriteLine("Inserting "+ intMainCnt + " out of "+ intTotalCnt + " rows...");
        //                        DBConnection32.ExecuteMSSQL(strConnectionStringFinalDatabase, sbFinalSQL.ToString());
        //                        sbFinalSQL.Remove(0, sbFinalSQL.Length);
        //                        intExecuteCnt = 1;
        //                    }

        //                    intMainCnt++;

        //                }

        //            }
        //        }
        //    }

        //}



        //        using System;
        //using System.Data;
        //using System.Linq;
        //using DocumentFormat.OpenXml.Packaging;
        //using DocumentFormat.OpenXml.Spreadsheet;

        //public static DataTable ReadAsDataTable(string fileName)
        //    {
        //        DataTable dataTable = new DataTable();
        //        using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
        //        {
        //            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
        //            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
        //            string relationshipId = sheets.First().Id.Value;
        //            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
        //            Worksheet workSheet = worksheetPart.Worksheet;
        //            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
        //            IEnumerable<Row> rows = sheetData.Descendants<Row>();

        //            foreach (Cell cell in rows.ElementAt(0))
        //            {
        //                dataTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
        //            }

        //            foreach (Row row in rows)
        //            {
        //                DataRow dataRow = dataTable.NewRow();
        //                for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
        //                {
        //                    dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
        //                }

        //                dataTable.Rows.Add(dataRow);
        //            }

        //        }
        //        dataTable.Rows.RemoveAt(0);

        //        return dataTable;
        //    }

        //    private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        //    {
        //        SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
        //        string value = cell.CellValue.InnerXml;

        //        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        //        {
        //            return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
        //        }
        //        else
        //        {
        //            return value;
        //        }
        //    }


    }
}
