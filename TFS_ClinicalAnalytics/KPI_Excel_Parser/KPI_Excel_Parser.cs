using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI_Excel_Parser
{
    class KPI_Excel_Parser
    {
        static void Main(string[] args)
        {
            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];

            using (var wb = new XLWorkbook(@"C:\Users\cgiorda\Desktop\KPI_Report_May_Final.xlsx", XLEventTracking.Disabled))
            {
                var ws = wb.Worksheet("Post-Deployment");
                var rows = ws.RangeUsed().RowsUsed().Skip(4); // Skip header row


                //string strSQLDeclare = "DECLARE @kpi_cleanup_id AS BIGINT; ";

                //string strSQLMain = "INSERT INTO [dbo].[KPI_Cleanup] ([LOB] ,[Blue_Chip_Cash_id] ,[Blue_Chip_SubCat_id] ,[Program_Rollup_Cash_id] ,[Initiative_Cash_id] ,[Affordability_Workgroup_Owner_Id] ,[Cash_Accountability_Partner_id] ,[PM_Initiative_Owner_id] ,[Strategic_Program_Lead_id] ,[UCS_Lead_id] ,[ECS_Lead_id] ,[OH_Lead_id] ,[OH_Owner_id] ,[HCE_Owner_byLOB_id] ,[Cash_Phase] ,[Expected_Deploy_Date] ,[Planned_Deploy_Date] ,[Actual_Deploy_Date] ,[Delegation] ,[KPI_Auditor_id] ,[KPI_Owner_id] ,[Financial_Confidence] ,[Year_Target] ,[Primary_Drivers] ,[Key_Performance_Indicator_id] ,[Current_Results] ,[Prior_Period_Results] ,[Key_Performance_Indicator_id2] ,[Frequency] ,[Leading_Lagging_Indicator] ,[process_month] ,[process_year] ,[process_date]) VALUES ({$insert});";

                string strCleanupForTesting = "truncate table KPI_Cleanup_Results; truncate table KPI_Cleanup_Cash; truncate table KPI_Cleanup; truncate table KPI_Initiative_List; truncate table KPI_Rollup_List; truncate table KPI_Users_List;";
                strCleanupForTesting = "";

                string strSQLMain = "DECLARE @kpi_cleanup_id AS BIGINT; INSERT INTO [dbo].[KPI_Cleanup] ([LOB] ,[Blue_Chip_Cash_id] ,[Blue_Chip_SubCat_id] ,[Program_Rollup_Cash_id] ,[Initiative_Cash_id] ,[Affordability_Workgroup_Owner_Id] ,[Cash_Accountability_Partner_id] ,[PM_Initiative_Owner_id] ,[Strategic_Program_Lead_id] ,[UCS_Lead_id] ,[ECS_Lead_id] ,[OH_Lead_id] ,[OH_Owner_id] ,[HCE_Owner_byLOB_id] ,[Cash_Phase],[Expected_Deploy_Date] ,[Planned_Deploy_Date] ,[Actual_Deploy_Date],[Delegation] ,[KPI_Auditor_id] ,[KPI_Owner_id] ,[Financial_Confidence] ,[Year_Target],[Primary_Drivers], [Frequency], [Leading_Lag_Indicator], [process_month], [process_year]) VALUES ({$insert}); SET @kpi_cleanup_id = SCOPE_IDENTITY(); ";


                string strSQLInitiative = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE BEGIN TRANSACTION DECLARE {$SQL_var} AS SMALLINT SELECT {$SQL_var} = KPI_Initiative_id FROM KPI_Initiative_List WHERE KPI_Initiative_desc={$value} IF {$SQL_var} IS NULL BEGIN INSERT INTO KPI_Initiative_List(KPI_Initiative_desc) VALUES ({$value}) SELECT {$SQL_var} = SCOPE_IDENTITY() END SELECT {$SQL_var} COMMIT TRANSACTION;";

                string strSQLRollup = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE BEGIN TRANSACTION DECLARE {$SQL_var} AS SMALLINT SELECT {$SQL_var} = KPI_Rollup_id FROM KPI_Rollup_List WHERE KPI_Rollup_desc={$value} IF {$SQL_var} IS NULL BEGIN INSERT INTO KPI_Rollup_List(KPI_Rollup_desc) VALUES ({$value}) SELECT {$SQL_var} = SCOPE_IDENTITY() END SELECT {$SQL_var} COMMIT TRANSACTION;";

                string strSQLUsers = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE BEGIN TRANSACTION DECLARE {$SQL_var} AS SMALLINT SELECT {$SQL_var} = KPI_User_id FROM KPI_Users_List WHERE KPI_User_Name={$value} IF {$SQL_var} IS NULL BEGIN INSERT INTO KPI_Users_List(KPI_User_Name) VALUES ({$value}) SELECT {$SQL_var} = SCOPE_IDENTITY() END SELECT {$SQL_var} COMMIT TRANSACTION;";

                StringBuilder sbMainInsert = new StringBuilder();
                StringBuilder sbIdentityInserts = new StringBuilder();


                string strSQLCash = "INSERT INTO [dbo].[KPI_Cleanup_Cash] ([KPI_Cash_Id] ,[kpi_cleanup_id]) VALUES ({$value}, @kpi_cleanup_id);";
                StringBuilder sbCashInserts = new StringBuilder();


                string strSQLResults = "INSERT INTO [dbo].[KPI_Cleanup_Results] ([Key_Perfomance_Indicator] ,[Current_Results] ,[Prior_Results] ,[kpi_cleanup_id]) VALUES ({$kpi},{$cr},{$pr},@kpi_cleanup_id);";
                StringBuilder sbResultInserts = new StringBuilder();


                bool blSkip = false;

                string strValue;
                string strFinalSQL;

                string[] strArrSplits;
                string[] strArrSplits2;
                string[] strArrSplits3;
                int intRowCnt = 0;
                foreach (var row in rows)
                {
                    intRowCnt++;

                    //DETERMINE IF RUN OR IGNORE!!!!
                    //Key Performance Indicators (KPIs), Current Results, Prior Period Results
                    if (row.Cell("AD").CachedValue + "" != "" && row.Cell("AE").CachedValue + "" != "" && row.Cell("AF").CachedValue + "" != "")
                    {
                        //Key Performance Indicators (KPIs)
                        strArrSplits = row.Cell("AD").CachedValue.ToString().Trim().Split('\n');
                        //Current Results
                        strArrSplits2 = row.Cell("AE").CachedValue.ToString().Trim().Split('\n');
                        //Prior Period Results
                        strArrSplits3 = row.Cell("AF").CachedValue.ToString().Trim().Split('\n');


                        if (strArrSplits.Length == strArrSplits2.Length && strArrSplits.Length == strArrSplits3.Length)
                        {
                            for (int i = 0; i < strArrSplits.Length; i++)
                            {
                                sbResultInserts.Append(strSQLResults.Replace("{$kpi}", "'" + strArrSplits[i].Replace("'", "''") + "'").Replace("{$cr}", "'" + strArrSplits2[i].Replace("'", "''") + "'").Replace("{$pr}", "'" + strArrSplits3[i].Replace("'", "''") + "'"));
                            }
                            blSkip = false;
                        }
                        else
                            blSkip = true;

                    }
                    else
                        blSkip = true;

                    //DETERMINE IF RUN OR IGNORE!!!!
                    if (blSkip)
                    {
                        Console.WriteLine("Skipping row # " + intRowCnt + " out of " + rows.Count() + " rows...");
                        continue;
                    } 
                    else
                        Console.WriteLine("Processing row # " + intRowCnt + " out of " + rows.Count() + " rows...");



                    //LOB
                    sbMainInsert.Append((row.Cell("A").CachedValue + "" != "" ? "'" + row.Cell("A").CachedValue.ToString().Replace("'", "''") + "'": "NULL") + ",");

                    //CASH ID
                    if(row.Cell("B").CachedValue + "" != "")
                    {
                        strArrSplits = row.Cell("B").CachedValue.ToString().Split('\n');
                        foreach(string s in strArrSplits)
                        {
                            sbCashInserts.Append(strSQLCash.Replace("{$value}", "'" + s.Replace("'","''") + "'"));
                        }
                    }

                    //Blue Chip CASH
                    if (row.Cell("C").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLRollup.Replace("{$value}", "'" + row.Cell("C").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@bc_cash_id"));
                        sbMainInsert.Append("@bc_cash_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");

                    //Blue Chip SubCategory
                    if (row.Cell("D").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLRollup.Replace("{$value}", "'" + row.Cell("D").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@bc_sc_id"));
                        sbMainInsert.Append("@bc_sc_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");

                    //Program Roll Up CASH
                    if (row.Cell("E").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLRollup.Replace("{$value}", "'" + row.Cell("E").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@pr_cash_id"));
                        sbMainInsert.Append("@pr_cash_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //Initiative Name CASH
                    if (row.Cell("F").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLInitiative.Replace("{$value}", "'" + row.Cell("F").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@in_cash_id"));
                        sbMainInsert.Append("@in_cash_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");

                    //Affordability Workgroup Owner
                    if (row.Cell("G").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("G").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@awo_id"));
                        sbMainInsert.Append("@awo_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //CASH Accountability Partner
                    if (row.Cell("H").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("H").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@cap_id"));
                        sbMainInsert.Append("@cap_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");

                    //Project Manager / Initiative Owner
                    if (row.Cell("I").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("I").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@pmio_id"));
                        sbMainInsert.Append("@pmio_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");

                    //Strategic / Program Lead
                    if (row.Cell("J").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("J").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@spl_id"));
                        sbMainInsert.Append("@spl_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //UCS Lead
                    if (row.Cell("K").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("K").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@ucsl_id"));
                        sbMainInsert.Append("@ucsl_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //ECS Lead 
                    if (row.Cell("L").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("L").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@ecsl_id"));
                        sbMainInsert.Append("@ecsl_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //OH Product Lead
                    if (row.Cell("M").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("M").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@ohpl_id"));
                        sbMainInsert.Append("@ohpl_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //LOB Owner
                    if (row.Cell("N").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("N").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@lobo_id"));
                        sbMainInsert.Append("@lobo_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //HCE Owner by LOB
                    if (row.Cell("O").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("O").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@hceo_id"));
                        sbMainInsert.Append("@hceo_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");



                    //CASH Phase (0-7)
                    sbMainInsert.Append((row.Cell("P").CachedValue + "" != "" ? row.Cell("P").CachedValue.ToString() : "NULL") + ",");


                    //Expected  Deploy Date
                    if (row.Cell("Q").CachedValue + "" != "")
                    {
                        DateTime temp;
                        if (DateTime.TryParse(row.Cell("Q").CachedValue.ToString(), out temp))
                            strValue = "'" + temp.ToShortDateString()  + "'";
                        else
                            strValue = "NULL";
                    }
                    else
                        strValue = "NULL";

                    sbMainInsert.Append(strValue + ", ");


                    //Planned   Deploy Date
                    if (row.Cell("R").CachedValue + "" != "")
                    {
                        DateTime temp;
                        if (DateTime.TryParse(row.Cell("R").CachedValue.ToString(), out temp))
                            strValue = "'" + temp.ToShortDateString() + "'";
                        else
                            strValue = "NULL";
                    }
                    else
                        strValue = "NULL";

                    sbMainInsert.Append(strValue + ", ");


                    //Actual Deploy Date
                    if (row.Cell("S").CachedValue + "" != "")
                    {
                        DateTime temp;
                        if (DateTime.TryParse(row.Cell("S").CachedValue.ToString(), out temp))
                            strValue = "'" + temp.ToShortDateString() + "'";
                        else
                            strValue = "NULL";
                    }
                    else
                        strValue = "NULL";

                    sbMainInsert.Append(strValue + ", ");


                    //Delegation? 
                    if (row.Cell("T").CachedValue + "" != "")
                    {
                        if (row.Cell("T").CachedValue.ToString().ToLower().Equals("n") || row.Cell("T").CachedValue.ToString().ToLower().Equals("no"))
                        {
                            strValue = "0";
                        }
                        else if (row.Cell("T").CachedValue.ToString().ToLower().Equals("y") || row.Cell("T").CachedValue.ToString().ToLower().Equals("yes"))
                        {
                            strValue = "1";
                        }
                        else
                            strValue = "NULL";
                    }
                    else
                        strValue = "NULL";

                    sbMainInsert.Append(strValue + ", ");



                    //KPI Auditor
                    if (row.Cell("U").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("U").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@kpia_id"));
                        sbMainInsert.Append("@kpia_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");

                    //KPI Owner
                    if (row.Cell("V").CachedValue + "" != "")
                    {
                        sbIdentityInserts.Append(strSQLUsers.Replace("{$value}", "'" + row.Cell("V").CachedValue.ToString().Replace("'", "''") + "'").Replace("{$SQL_var}", "@kpio_id"));
                        sbMainInsert.Append("@kpio_id,");
                    }
                    else
                        sbMainInsert.Append("NULL,");


                    //Financial Confidence
                    sbMainInsert.Append((row.Cell("W").CachedValue + "" != "" ? "'" + row.Cell("W").CachedValue.ToString().Replace("'", "''") + "'" : "NULL") + ",");

                    //2021 Target 
                    sbMainInsert.Append((row.Cell("X").CachedValue + "" != "" ?  row.Cell("X").CachedValue.ToString()  : "NULL") + ",");


                    //Primary Driver(s)
                    sbMainInsert.Append((row.Cell("AC").CachedValue + "" != "" ? "'" + row.Cell("AC").CachedValue.ToString().Replace("'", "''") + "'" : "NULL") + ",");


                    //Frequency
                    sbMainInsert.Append((row.Cell("AG").CachedValue + "" != "" ? "'" + row.Cell("AG").CachedValue.ToString().Replace("'", "''") + "'" : "NULL") + ",");


                    //Leading Lagging Indicator 
                    sbMainInsert.Append((row.Cell("AH").CachedValue + "" != "" ? "'" + row.Cell("AH").CachedValue.ToString().Replace("'", "''") + "'" : "NULL") + ",");

                    //process_month
                    sbMainInsert.Append("'" + DateTime.Now.ToString("MMMM") + "',");

                    //process_year
                    sbMainInsert.Append("'" + DateTime.Now.Year + "'");



                    strFinalSQL = strCleanupForTesting + "                " + sbIdentityInserts.ToString() + "               " + strSQLMain.Replace("{$insert}", sbMainInsert.ToString()) + "               " + sbCashInserts.ToString() + "               " + sbResultInserts.ToString();



                    DBConnection32.ExecuteMSSQL(strConnectionString, strFinalSQL);


                    sbIdentityInserts.Remove(0, sbIdentityInserts.Length);
                    sbCashInserts.Remove(0, sbCashInserts.Length);
                    sbMainInsert.Remove(0, sbMainInsert.Length);
                    sbResultInserts.Remove(0, sbResultInserts.Length);
                }


                /* Process data table as you wish */
            }


        }
    }
}
