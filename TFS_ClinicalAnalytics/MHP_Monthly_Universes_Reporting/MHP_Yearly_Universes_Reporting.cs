using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MHP_Yearly_Universes_Reporting
{
    class MHP_Yearly_Universes_Reporting
    {
        private static string _strState;
        private static string _strStartDate;
        private static string _strEndDate;

        static void Main(string[] args)
        {

            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strRequestPath = ConfigurationManager.AppSettings["RequestPath"];



            List<string> files = Directory.EnumerateFiles(strRequestPath, "*.xlsx", SearchOption.TopDirectoryOnly).ToList();
            DataSet dsParamaters = new DataSet();
            DataSet dsResults = new DataSet();
            System.Data.DataTable dt;
            string strState, strEmail, strStartDate, strEndDate, strMktSegDesc, strFinArngCdLst,  strLegalEntityName, strLegalEntityIdList;
            foreach (string strFile in files)
            {

                if (strFile.StartsWith("~"))
                {
                    continue;
                }

                var workbook = new XLWorkbook(strFile);

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {

                    //dt = worksheet.RangeUsed().AsTable().AsNativeDataTable();
                    dt = ImportExceltoDatatable(worksheet);
                    dt.TableName = worksheet.Name;
                    dsParamaters.Tables.Add(dt);
                }

                if(dsParamaters.Tables.Contains("Details"))
                {
                    strEmail = dsParamaters.Tables["Details"].Rows[0][1].ToString();
                    strState = dsParamaters.Tables["Details"].Rows[1][1].ToString();
                    strStartDate = dsParamaters.Tables["Details"].Rows[2][1].ToString();
                    strEndDate = dsParamaters.Tables["Details"].Rows[3][1].ToString();
                    strMktSegDesc = dsParamaters.Tables["Details"].Rows[4][1].ToString();
                    strFinArngCdLst = dsParamaters.Tables["Details"].Rows[5][1].ToString();

                }
                else
                {
                    // notify
                    continue;
                }


                foreach (System.Data.DataTable t in dsParamaters.Tables)
                {

                    if (t.TableName == "Details")
                        continue;

                    strLegalEntityName = t.TableName;
                    strLegalEntityIdList = String.Join(",", t.AsEnumerable().Select(x => x.Field<string>("LegalEntityID").ToString()).ToArray()).TrimEnd(',');


                    dt = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, getDrivingSQL(strState, DateTime.Parse(strStartDate).ToShortDateString(), DateTime.Parse(strEndDate).ToShortDateString(), strMktSegDesc, strFinArngCdLst.Replace(" ", "").Replace(",", "','"), strLegalEntityName, strLegalEntityIdList));
                    dt.TableName = t.TableName;
                    dsResults.Tables.Add(dt);
                }


                GenerateReportXML(dsResults);




                Console.WriteLine(); // outputs the current worksheet name.
         


                //GenerateReportInterop();


               // System.Data.DataTable dtMain = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, getDrivingSQL());

                // NOTE: Don't call Excel objects in here... 
                // Debugger would keep alive until end, preventing GC cleanup
                // Call a separate function that talks to Excel
                //GenerateReportXML(dtMain);
                // Now let the GC clean up (repeat, until no more)
                //do
                //{
                //    GC.Collect();
                //    GC.WaitForPendingFinalizers();
                //}
                //while (Marshal.AreComObjectsAvailableForCleanup());

            }



        }


        static void GenerateReportXML(DataSet dsMain)
        {
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "template\\MHP_Reporting_Template.xlsx";

            XLWorkbook wb = new XLWorkbook(strPath);
            IXLWorksheet wsSource = null;

            foreach (System.Data.DataTable dt in dsMain.Tables)
            {

                wsSource = wb.Worksheet("template");
                // Copy the worksheet to a new sheet in this workbook
                //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
                wsSource.CopyTo("template COPY");
                wsSource = wb.Worksheet("template COPY");
                wsSource.Cell("A1").Value = _strState + " "+ dt.TableName +" : " + _strStartDate + "-" + _strEndDate;

                foreach (DataRow dr in dt.Rows)
                {

                    wsSource.Cell("B" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_in_ip"] + "") ? null : dr["cnt_in_ip"] + "");
                    wsSource.Cell("D" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_on_ip"] + "") ? null : dr["cnt_on_ip"] + "");
                    wsSource.Cell("F" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_in_op"] + "") ? null : dr["cnt_in_op"] + "");
                    wsSource.Cell("H" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_on_op"] + "") ? null : dr["cnt_on_op"] + "");

                    //int intPosition = 1;
                    ////REORDER NEW SHEETS
                    //foreach (IXLWorksheet worksheet in wb.Worksheets)
                    //{
                    //    foreach (DataRow dr in dtMain.Rows)
                    //    {
                    //        strYQ = dr["file_year"].ToString() + dr["file_quarter"].ToString();
                    //        if (worksheet.Name.ToString().Contains(strYQ))
                    //        {
                    //            worksheet.Position = intPosition;
                    //            intPosition++;
                    //            break;
                    //        }

                    //    }
                    //}

                }
            }

            wb.Worksheet("template").Delete();
            // Save the workbook with the 2 copies
            wb.SaveAs(@"C:\Users\cgiorda\Desktop\MHP Report Test\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx");
        }



        static void GenerateReportXML(System.Data.DataTable dtMain)
        {

            string strPath = AppDomain.CurrentDomain.BaseDirectory + "template\\MHP_Reporting_Template.xlsx";

            XLWorkbook wb = new XLWorkbook(strPath);
            IXLWorksheet wsSource = null;

            wsSource = wb.Worksheet("template");
            // Copy the worksheet to a new sheet in this workbook
            //wsSource.CopyTo("template COPY1").SetTabColor(XLColor.Orange);
            wsSource.CopyTo("template COPY");
            wsSource = wb.Worksheet("template COPY");
            wsSource.Cell("A1").Value = _strState + " UHIC : " + _strStartDate + "-" + _strEndDate;

            foreach (DataRow dr in dtMain.Rows)
            {

                wsSource.Cell("B" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_in_ip"] + "") ? null : dr["cnt_in_ip"] + "");
                wsSource.Cell("D" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_on_ip"] + "") ? null : dr["cnt_on_ip"] + "");
                wsSource.Cell("F" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_in_op"] + "") ? null : dr["cnt_in_op"] + "");
                wsSource.Cell("H" + dr["cell"]).Value = (string.IsNullOrEmpty(dr["cnt_on_op"] + "") ? null : dr["cnt_on_op"] + "");

                //int intPosition = 1;
                ////REORDER NEW SHEETS
                //foreach (IXLWorksheet worksheet in wb.Worksheets)
                //{
                //    foreach (DataRow dr in dtMain.Rows)
                //    {
                //        strYQ = dr["file_year"].ToString() + dr["file_quarter"].ToString();
                //        if (worksheet.Name.ToString().Contains(strYQ))
                //        {
                //            worksheet.Position = intPosition;
                //            intPosition++;
                //            break;
                //        }

                //    }
                //}

            }


            wb.Worksheet("template").Delete();
            // Save the workbook with the 2 copies
            wb.SaveAs(@"C:\Users\cgiorda\Desktop\MHP Report Test\MHP_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx");
        }


        


        static string getDrivingSQL(string strState, string strStartDate, string strEndDate, string strMktSegDesc, string strFinArngCdLst, string strLegalEntityName, string strLegalEntityIdList)
        {
           
            StringBuilder sbSQL = new StringBuilder();
            string strWhere = null;
            string strExcelRow = null;

            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        strWhere = "AND u.[Authorization_Type]  in ('S', 'U', 'PS') ";
                        strExcelRow = "4";
                        break;
                    case 1:
                        strWhere = "AND u.[Request_Decision] in ('FF', 'PF')  AND u.[Authorization_Type]  in ('S', 'U', 'PS') ";
                        strExcelRow = "5";
                        break;
                    case 2:
                        strWhere = "AND u.[Request_Decision] in ('AD')  AND u.[Authorization_Type]  in ('S', 'U', 'PS') ";
                        strExcelRow = "6";
                        break;
                    case 3:
                        strWhere = "AND u.[Authorization_Type]  in ('S', 'U', 'PS')  ";
                        strExcelRow = "56";
                        break;
                    case 4:
                        strWhere = "AND u.[Request_Decision] in ('FF', 'PF')  AND u.[Authorization_Type]  in ('S', 'U', 'PS') ";
                        strExcelRow = "57";
                        break;
                    case 5:
                        strWhere = "AND u.[Request_Decision] in ('AD')  AND u.[Authorization_Type]  in ('S', 'U', 'PS') ";
                        strExcelRow = "58";
                        break;
                    default:
                        break;
                }

                sbSQL.Append("SELECT ");
                sbSQL.Append(strExcelRow + " as cell, ");//4 AND
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'Site is PAR' AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'Site is PAR' AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                sbSQL.Append("MAX(CASE WHEN tmp.[Par_NonPar_Site] = 'NonPar Site' AND tmp.[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op ");
                sbSQL.Append("FROM( ");
                sbSQL.Append("SELECT count(Distinct u.[Authorization]) cnt,  u.[Par_NonPar_Site],  u.[Inpatient_Outpatient] ");
                sbSQL.Append("FROM [IL_UCA].[stg].[MHP_Yearly_Universes] u ");
                sbSQL.Append("INNER JOIN [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP_CACHE3] c ON c.[mhp_uni_id] = u.[mhp_uni_id]  ");
                sbSQL.Append("WHERE  ");
                //sbSQL.Append("--UNIVERSAL FILTERS");
                sbSQL.Append("u.[State_of_Issue] = '" + strState + "' AND  u.[Par_NonPar_Site] <> 'N/A' AND  u.[Request_Date] >= '" + strStartDate + "' AND   u.[Request_Date] <= '" + strEndDate + "' ");
                sbSQL.Append("AND c.[MKT_SEG_DESC] = '" + strMktSegDesc + "' AND  c.[FINC_ARNG_CD] in ('"+ strFinArngCdLst + "') ");
                //sbSQL.Append("--PER SHEET FILTERS");
                sbSQL.Append("AND c.[LEG_ENTY_NBR] in (" + strLegalEntityIdList + ") ");
                //sbSQL.Append("--SECTION FILTER(S) ROW " + strExcelRow);
                sbSQL.Append(strWhere);
                sbSQL.Append("GROUP BY u.[State_of_Issue], u.[Par_NonPar_Site], u.[Inpatient_Outpatient] ");
                sbSQL.Append(") tmp ");
                sbSQL.Append("UNION ALL ");



                //sbSQL.Append("SELECT ");
                //sbSQL.Append(strExcelRow + " as cell, ");//4 AND
                //sbSQL.Append("MAX(CASE WHEN[Par_NonPar_Site] = 'Site is PAR' AND[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_in_ip, ");
                //sbSQL.Append("MAX(CASE WHEN[Par_NonPar_Site] = 'NonPar Site' AND[Inpatient_Outpatient] = 'Inpatient' THEN cnt ELSE NULL END) cnt_on_ip, ");
                //sbSQL.Append("MAX(CASE WHEN[Par_NonPar_Site] = 'Site is PAR' AND[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END) cnt_in_op, ");
                //sbSQL.Append("MAX(CASE WHEN[Par_NonPar_Site] = 'NonPar Site' AND[Inpatient_Outpatient] = 'Outpatient' THEN cnt ELSE NULL END)  cnt_on_op ");
                //sbSQL.Append("FROM( ");
                //sbSQL.Append("SELECT count(Distinct[Authorization]) cnt, [Par_NonPar_Site], [Inpatient_Outpatient] ");
                //sbSQL.Append("FROM [IL_UCA].[stg].[MHP_Yearly_Universes] ");
                //sbSQL.Append("WHERE State_of_Issue = '"+ strState + "' AND [Par_NonPar_Site] <> 'N/A' AND [Request_Date] >= '"+strStartDate+"' AND  [Request_Date] <= '"+strEndDate+"' "); //
                //sbSQL.Append(strWhere);
                //sbSQL.Append("GROUP BY[State_of_Issue], [Par_NonPar_Site], [Inpatient_Outpatient] ");
                //sbSQL.Append(") tmp ");
                //sbSQL.Append("UNION ALL ");

            }

            return sbSQL.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' ');

        }


        public static System.Data.DataTable ImportExceltoDatatable(IXLWorksheet workSheet)
        {

            //Create a new DataTable.
            System.Data.DataTable dt = new System.Data.DataTable();

            //Consider the first row as container column names
            bool firstRow = true;
            foreach (IXLRow row in workSheet.Rows())
            {
                //Use the first row to add columns to DataTable.
                if (firstRow)
                {
                    foreach (IXLCell cell in row.Cells())
                    {
                        dt.Columns.Add(cell.Value.ToString());
                    }
                    firstRow = false;
                }
                else
                {
                    //Add rows to DataTable.
                    dt.Rows.Add();
                    int i = 0;
                    foreach (IXLCell cell in row.Cells())
                    {
                        string val = string.Empty;

                        try
                        {
                            val = cell.Value.ToString();
                        }
                        catch { }

                        dt.Rows[dt.Rows.Count - 1][i] = val;
                        i++;
                    }
                }
            }
     

            return dt;
        }
    }
}
