using ClosedXML.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PhysicianFeedbackTracker
{
    public static class ClosedXMLExcelFunctions
    {

        //private static XLWorkbook wb = null;
        //public static void loadExcel(string strFilePath)
        //{
        //    wb = new XLWorkbook(strFilePath);
        //    // IXLWorksheet

        //   // DataTable dt = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectTrackerFullRequestSQL, getParameterList());

        //   // wb.Worksheets.Add(dt, "WorksheetName");
        //   // wb.SaveAs(strFileName, true);

        //}

        public static void addValidationFeedbackSheet(string strFilePath, DataTable dtOverview, DataTable dtMain, string strSheetName)
        {
            XLWorkbook workbook = new XLWorkbook(strFilePath);
            string strFinalSheetName = null;


            //CHECK FOR OLDER SHEETS BEFORE CREATING NEW
            int iCnt = 1;
            while ((workbook.Worksheets.FirstOrDefault(worksheets => worksheets.Name == strSheetName + "_" + iCnt)) != null)
                iCnt++;

   
            strFinalSheetName = strSheetName + "_" + iCnt;


            IXLWorksheet worksheet = workbook.Worksheets.Add(strFinalSheetName);


            //START OFF ASSUMING THE BEST
            worksheet.SetTabColor(XLColor.Green);


            //worksheet.Rows().Style.Fill.BackgroundColor = XLColor.LightCyan;
            //ADD OVERVIEW TO SHEET
            //ADD OVERVIEW TO SHEET
            //ADD OVERVIEW TO SHEET
            int iRowCnt = 1;
            int iColCnt = 1;
            string strMessage = null;
            int[] intStartIndexesArr;
            int[] intEndIndexesArr;
            foreach (DataColumn col in dtOverview.Columns)
            {
                foreach (DataRow row in dtOverview.Rows)
                {
                    strMessage =   "Message " + iRowCnt + ": " + row[col.ColumnName].ToString();
                    intStartIndexesArr = strMessage.FindAllIndexof('{');
                    intEndIndexesArr = strMessage.FindAllIndexof('}');

                    worksheet.Cell(iRowCnt, iColCnt).Value = strMessage.Replace("{", "").Replace("}", "");
                    worksheet.Range("A" + iRowCnt + ":T" + iRowCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    worksheet.Range("A" + iRowCnt + ":T" + iRowCnt).Style.Border.OutsideBorderColor = XLColor.Red;

                    for(int i = 0; i < intStartIndexesArr.Length; i++)
                    {
                        worksheet.Cell(iRowCnt, iColCnt).RichText.Substring(intStartIndexesArr[i] - (i * 2), (intEndIndexesArr[i] - intStartIndexesArr[i])).SetBold();
                    }
                    //worksheet.Cell(iRowCnt, iColCnt).Value = worksheet.Cell(iRowCnt, iColCnt).Value.ToString().Replace("{", "").Replace("}", "");

                    worksheet.Range("A" + iRowCnt + ":T" + iRowCnt).Merge();

                    iRowCnt++;
                }
                iColCnt++;
                iRowCnt = 1;
            }
            if (dtOverview.Rows.Count > 0)
                worksheet.SetTabColor(XLColor.Red);



            //ADD MAIN DATA TO SHEET
            //ADD MAIN DATA TO SHEET
            //ADD MAIN DATA TO SHEET
            if(dtMain != null)
            {
                iRowCnt = dtOverview.Rows.Count + 1;
                iColCnt = 1;
                foreach (DataColumn col in dtMain.Columns)
                {
                    foreach (DataRow row in dtMain.Rows)
                    {

                        if (iRowCnt == dtOverview.Rows.Count + 1 && !col.ColumnName.EndsWith("_Feedback!!!"))//ADD COLUMN HEADER TO EXCEL
                        {
                            worksheet.Cell(iRowCnt, iColCnt).Value = col.ColumnName.ToString();
                            worksheet.Cell(iRowCnt, iColCnt).Style.Font.FontColor = XLColor.White;
                            worksheet.Cell(iRowCnt, iColCnt).Style.Fill.BackgroundColor = XLColor.Teal;
                            worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;
                            iRowCnt++;
                        }


                        //if (!col.ColumnName.EndsWith("_Feedback!!!"))
                        //{
                        //    worksheet.Cell(iRowCnt, iColCnt).Value = row[col.ColumnName].ToString();
                        //    worksheet.Cell(iRowCnt, iColCnt).Style.Alignment.WrapText = true;
                        //    worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //    worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;
                        //}

                        if (col.ColumnName.EndsWith("_Feedback!!!"))
                        {

                            if((iRowCnt - (dtOverview.Rows.Count + 1)) == 0)
                            {
                                worksheet.Cell(1, iColCnt - 1).Style.Fill.BackgroundColor = XLColor.Red;
                                worksheet.SetTabColor(XLColor.Red);
                                iRowCnt++;
                            }

                            if (row[col.ColumnName].ToString() != "")
                            {

                                worksheet.Cell(iRowCnt, iColCnt - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                worksheet.Cell(iRowCnt, iColCnt - 1).Style.Border.OutsideBorderColor = XLColor.Red;
                                worksheet.Cell(iRowCnt, iColCnt - 1).Style.Font.Bold = true;


                                worksheet.Cell(iRowCnt, iColCnt - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                worksheet.Cell(iRowCnt, iColCnt - 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);


                                worksheet.Cell(iRowCnt, iColCnt - 1).Comment.AddText(row[col.ColumnName].ToString()).AddNewLine();
                                //worksheet.Cell(iRowCnt, iColCnt - 1).Comment.Style.Alignment.SetVertical(XLDrawingVerticalAlignment.Top);
                                //worksheet.Cell(iRowCnt, iColCnt - 1).Comment.Style.Alignment.SetHorizontal(XLDrawingHorizontalAlignment.Left);
                                //worksheet.Cell(iRowCnt, iColCnt - 1).Comment.Style.Alignment.SetAutomaticSize();
                                worksheet.Cell(iRowCnt, iColCnt - 1).Comment.Style.Size.SetHeight(100).Size.SetWidth(70);

                            }

                        }
                        else
                        {
                            worksheet.Cell(iRowCnt, iColCnt).Value = row[col.ColumnName].ToString();
                            worksheet.Cell(iRowCnt, iColCnt).Style.Alignment.WrapText = true;
                            worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;
                        }

                        iRowCnt++;
                    }

                    //if (col.ColumnName.EndsWith("_Feedback!!!"))
                    //{

                    //}
                    if (!col.ColumnName.EndsWith("_Feedback!!!"))
                        iColCnt++;

                    iRowCnt = dtOverview.Rows.Count + 1;
                }
            }
            

            //FORMAT ENTIRE SHEET
            worksheet.Rows().Height = 30;
            worksheet.Columns().AdjustToContents();

            //worksheet.Rows().Height = 60;
            //worksheet.Rows().AdjustToContents();
            //worksheet.Columns().AdjustToContents();
            


            workbook.Save();



        }


        static bool _blCancelRequest;

        public static string addQAResultsLoader(DataTable dtMain, string strSampleSQL, string strPhase, ref TextBox txtStatus, CancellationToken cancellationToken)
        {
            //XLWorkbook workbook = new XLWorkbook(strFilePath);

            string strFinalSheetName = null;



            XLWorkbook workbook = new XLWorkbook();


            string strFinalDetailsSQL, strFinalViewSQL;
            string strDetailsSQL, strViewSQL, strSampleQuery, strMeasureName, strMeasureType, strMeasureId;
            string strMPINSample = "";

            DateTime startTime;
            DateTime endTime;
            TimeSpan span;

            DataTable dtDetailsResults;
            DataTable dtViewResults;

            _blCancelRequest = false;

            string strRange = null;


            string strPath = Environment.ExpandEnvironmentVariables(GlobalObjects.strQACompanion_Reports_Path);
            string strNewFile = "QA_Companion_Results_" + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + ".xlsx";

            var strFinalPath = strPath + "\\" + strNewFile;
            //var strFinalPath = AppDomain.CurrentDomain.BaseDirectory + "\\tmp.xlsx";
            if (File.Exists(strFinalPath))
                File.Delete(strFinalPath);

            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);




            int intDebugCnt = 0;
            foreach (DataRow dr in dtMain.Rows)
            {
                Application.DoEvents();


                dtDetailsResults = null;
                dtViewResults = null;


                strDetailsSQL = dr["measure_detail_query"].ToString();
                strViewSQL = dr["measure_view_query"].ToString();
                strMeasureId = dr["iluca_id"].ToString();
                strMeasureName = dr["measure_description"].ToString();
                strMeasureType = dr["measure_type"].ToString();



                strFinalSheetName = strMeasureName.Replace(" ", "_").Replace("-", "_").Replace("/", "_").Replace("\\", "_");
                if (strFinalSheetName.Length > 31)
                    strFinalSheetName = strFinalSheetName.Substring(0, 31);


                if (strFinalSheetName == "")
                    continue;

                IXLWorksheet worksheet = workbook.Worksheets.Add(strFinalSheetName);
                //worksheet.Cell("A1").Value = "Hello World!";




                /////////////////////////////MPINS//////////////////////////////////
                strSampleQuery = strSampleSQL.Replace("{$measure_id}", strMeasureId).Replace("{$suffix}", strPhase);
                startTime = DateTime.Now;
                StartTheThread("----------------------(" + strMeasureName + ") MPIN Samples -----------------------------" + Environment.NewLine, txtStatus, cancellationToken);
                StartTheThread("(" + strMeasureName + ") Finding sample MPINs - Start Time = " + startTime.ToString("h:mm:ss tt") + Environment.NewLine, txtStatus, cancellationToken);
                strMPINSample = (string)DBConnection.getMSSQLExecuteScalar(GlobalObjects.strILUCAConnectionString, strSampleQuery, cancellationToken);
                endTime = DateTime.Now;
                StartTheThread("(" + strMeasureName + ") Found sample MPINs (" + strMPINSample + ") - End Time = " + endTime.ToString("h:mm:ss tt") + Environment.NewLine, txtStatus, cancellationToken);
                span = endTime.Subtract(startTime);
                StartTheThread("(" + strMeasureName + ") Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms ") + Environment.NewLine, txtStatus, cancellationToken);


                //worksheet.Cell(1,1).Comment.AddText(strFinalSQL).AddNewLine();
                //worksheet.Cell(1, 1).Comment.Style.Size.SetHeight(100).Size.SetWidth(70);

                /////////////////////////////DETAILS//////////////////////////////////
                strFinalDetailsSQL = strDetailsSQL.Replace("{$mpin}", strMPINSample).Replace("{$suffix}", strPhase);
                //strFinalDetailsSQL = "SELECT * FROM Bogus";
                startTime = DateTime.Now;
                StartTheThread("----------------------(" + strMeasureName + ") Run Details -----------------------------" + Environment.NewLine, txtStatus, cancellationToken);
                StartTheThread("(" + strMeasureName + ") Running Details SQL - Start Time = " + startTime.ToString("h:mm:ss tt") + Environment.NewLine, txtStatus, cancellationToken);
                try
                {
                    dtDetailsResults = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strFinalDetailsSQL, cancellationToken);
                }
                catch(Exception)
                {
                    StartTheThread(Environment.NewLine + Environment.NewLine + Environment.NewLine + "!!!! (" + strMeasureName + ") details table does not exist, skipping measure" + Environment.NewLine + Environment.NewLine + Environment.NewLine, txtStatus, cancellationToken);
                    workbook.Worksheets.Delete(strFinalSheetName);
                    continue;
                }
                endTime = DateTime.Now;
                StartTheThread("(" + strMeasureName + ") Finished Running Details SQL - End Time = " + endTime.ToString("h:mm:ss tt") + Environment.NewLine, txtStatus, cancellationToken);
                span = endTime.Subtract(startTime);
                StartTheThread("(" + strMeasureName + ") Total time for  Details SQL = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms ") + Environment.NewLine, txtStatus, cancellationToken);





                ////////////////////////////////VIEW/////////////////////////////////////////////////////////////////////////////////
                strFinalViewSQL = strViewSQL.Replace("{$mpin}", strMPINSample).Replace("{$suffix}", strPhase);
                startTime = DateTime.Now;
                StartTheThread("----------------------(" + strMeasureName + ") Run View -----------------------------" + Environment.NewLine, txtStatus, cancellationToken);
                StartTheThread("(" + strMeasureName + ") Running View - Start Time = " + startTime.ToString("h:mm:ss tt") + Environment.NewLine, txtStatus, cancellationToken);
                try
                {
                    dtViewResults = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strFinalViewSQL, cancellationToken);
                }
                catch (Exception)
                {
                    StartTheThread(Environment.NewLine + Environment.NewLine + Environment.NewLine + "!!!! (" + strMeasureName + ") view table does not exist, skipping measure" + Environment.NewLine + Environment.NewLine + Environment.NewLine, txtStatus, cancellationToken);
                    workbook.Worksheets.Delete(strFinalSheetName);
                    continue;
                }
                endTime = DateTime.Now;
                StartTheThread("(" + strMeasureName + ") Finished Running View - End Time = " + endTime.ToString("h:mm:ss tt") + Environment.NewLine, txtStatus, cancellationToken);
                span = endTime.Subtract(startTime);
                StartTheThread("(" + strMeasureName + ") Total time for View = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms ") + Environment.NewLine, txtStatus, cancellationToken);



                //COMPARE TABLES
                dtDetailsResults = SharedDataTableFunctions.compareDataTables(dtDetailsResults, dtViewResults);
                if (dtDetailsResults == null)
                    return null;



                //DETAILS TABLE POPULATE
                strRange = MSExcel.GetExcelColumnName("1") + "1" + ":" + MSExcel.GetExcelColumnName(dtDetailsResults.Columns.Count.ToString()) + "1";
                worksheet.Range(strRange).Merge();
                worksheet.Range(strRange).Value = "Detail Results for " + strMeasureName;
                worksheet.Range(strRange).Style.Fill.BackgroundColor = XLColor.BallBlue;
                worksheet.Range(strRange).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(strRange).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //REPLACE THIS 2018
               // worksheet.Cell(2, 1).InsertTable(dtDetailsResults);
                //WITH THIS
                var iRowCnt = 3;
                var iColCnt = 1;

                foreach (DataRow row in dtDetailsResults.Rows)
                {
                    foreach (DataColumn col in dtDetailsResults.Columns)
                    {

                        if (iRowCnt == 3)//ADD COLUMN HEADER TO EXCEL
                        {
                            worksheet.Cell((iRowCnt - 1), iColCnt).Value = col.ColumnName.ToString();
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Font.FontColor = XLColor.White;
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Fill.BackgroundColor = XLColor.Teal;
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;
                        }

                        worksheet.Cell(iRowCnt, iColCnt).Value = row[col.ColumnName].ToString();
                        worksheet.Cell(iRowCnt, iColCnt).Style.Alignment.WrapText = true;

                        worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;

                        if (row.Table.Columns.Contains("Error_Flags"))
                        {
                            if (row["Error_Flags"] != DBNull.Value)
                            {
                                //worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                //worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorderColor = XLColor.Red;

                                worksheet.Cell(iRowCnt, iColCnt).Style.Fill.BackgroundColor = XLColor.LightSalmonPink;

                            }
                        }

                        iColCnt++;
                    }
                    iColCnt = 1;

                    iRowCnt++;
                }
                worksheet.Cell(1, 1).Comment.AddText(strFinalDetailsSQL).AddNewLine();
                worksheet.Cell(1, 1).Comment.Style.Size.SetHeight(150).Size.SetWidth(70);







                //VIEW TABLE POPULATE
                strRange = MSExcel.GetExcelColumnName((dtDetailsResults.Columns.Count + 2).ToString()) + "1" + ":" + MSExcel.GetExcelColumnName((dtDetailsResults.Columns.Count + dtViewResults.Columns.Count + 1).ToString()) + "1";
                worksheet.Range(strRange).Merge();
                worksheet.Range(strRange).Value = "View Results for " + strMeasureName;
                worksheet.Range(strRange).Style.Fill.BackgroundColor = XLColor.BallBlue;
                worksheet.Range(strRange).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(strRange).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);


                //REPLACE THIS 2018
                //worksheet.Cell(2, dtDetailsResults.Columns.Count + 2).InsertTable(dtViewResults);

                //WITH THIS
                iRowCnt = 3;
                iColCnt = dtDetailsResults.Columns.Count + 2;

                foreach (DataRow row in dtViewResults.Rows)
                {
                    foreach (DataColumn col in dtViewResults.Columns)
                    {

                        if (iRowCnt == 3)//ADD COLUMN HEADER TO EXCEL
                        {
                            worksheet.Cell((iRowCnt - 1), iColCnt).Value = col.ColumnName.ToString();
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Font.FontColor = XLColor.White;
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Fill.BackgroundColor = XLColor.Teal;
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell((iRowCnt - 1), iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;
                        }

                        worksheet.Cell(iRowCnt, iColCnt).Value = row[col.ColumnName].ToString();
                        worksheet.Cell(iRowCnt, iColCnt).Style.Alignment.WrapText = true;
                        worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(iRowCnt, iColCnt).Style.Border.OutsideBorderColor = XLColor.Black;
                     

                        iColCnt++;
                    }

                    iColCnt = dtDetailsResults.Columns.Count + 2;
                    iRowCnt++;
                }



                worksheet.Cell(1, dtDetailsResults.Columns.Count + 2).Comment.AddText(strFinalViewSQL).AddNewLine();
                worksheet.Cell(1, dtDetailsResults.Columns.Count + 2).Comment.Style.Size.SetHeight(150).Size.SetWidth(70);


                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Comment.AddText(strSampleQuery).AddNewLine();
                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Comment.Style.Size.SetHeight(150).Size.SetWidth(70);


                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Value = "MPIN SQL";
                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Style.Font.FontColor = XLColor.White;
                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Style.Fill.BackgroundColor = XLColor.YellowGreen;
                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(1, dtDetailsResults.Columns.Count + 1).Style.Border.OutsideBorderColor = XLColor.Black;




                ////////////////////////////////SPACING/////////////////////////////////////////////////////////////////////////////////

                if (dtDetailsResults.Columns.Contains("Error_Flags"))
                {
                    worksheet.SetTabColor(XLColor.Red);
                }




                worksheet.Columns().AdjustToContents();
                StartTheThread(Environment.NewLine + Environment.NewLine  , txtStatus, cancellationToken);


                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (System.OperationCanceledException)
                {
                    _blCancelRequest = true;
                    StartTheThread("Process Cancelled" + Environment.NewLine, txtStatus, cancellationToken);
                    return null;
                    //throw;
                }

                intDebugCnt++;

                if (intDebugCnt == 5000)
                {
                    break;
                }


                workbook.SaveAs(strFinalPath);
                //workbook.Save();
                workbook.Dispose();
                workbook = null;
                GC.Collect();
                workbook = new XLWorkbook(strFinalPath);




            }


            //if (iRowCnt % 400 == 0)
            //{
            //    // workbook.SaveAs(strFinalPath);
            //    workbook.Save();
            //    workbook.Dispose();
            //    workbook = null;
            //    GC.Collect();
            //    workbook = new XLWorkbook(strFinalPath);
            //}






            if (workbook.Worksheets.Count > 0)
                workbook.SaveAs(strFinalPath);
            else
                strFinalPath = null;

            return strFinalPath;

        }



        public static void clearSpreadsheet(string strPath)
        {
            XLWorkbook workbook = new XLWorkbook(strPath);

            foreach (IXLWorksheet worksheet in workbook.Worksheets)
            {
                //Console.WriteLine(worksheet.Name); // outputs the current worksheet name.
                                                   // do the thing you want to do on each individual worksheet.
            }


        }




        public static  Thread StartTheThread(string strText, TextBox txtStatus, CancellationToken cancellationToken)
        {
            var t = new Thread(() => SetText(strText, txtStatus, cancellationToken));
            t.Start();
            t.Join();
            return t;
        }
        // This method is executed on the worker thread and makes 
        // a thread-safe call on the TextBox control. 
        private static void SetText( string strText, TextBox txtStatus, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested || _blCancelRequest)
                ThreadHelper.ThreadHelperClass.SetText(txtStatus.Parent.FindForm(), txtStatus, strText);
        }


       

    }

}
