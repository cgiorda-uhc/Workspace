
using System;
using System.Data;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace PhysicianFeedbackTracker
{
    class MSExcel
    {

        //EXCEL TEMPLATE

        private static string strExcelTemplate;
        //START EXCEL OBJECTS
        public static Excel.Application xlsApp = null;
        public static Excel.Workbook xlsWB = null;

        public static Excel.Worksheet xlsSheet = null;
        private static Int16 intExcelRow;
        private static bool blVisibleExcel;
        private static bool blSaveExcel;

        public static string strReportsPath;

        private static bool blDownloadedTemplates = false;

        public static void populateExcelParameters(bool isVisible, bool isSave, string sReportsPath, string sTemplatePath)
        {
            blVisibleExcel = isVisible;
            blSaveExcel = isSave;
            strReportsPath = sReportsPath;

            strExcelTemplate = sTemplatePath;

        }

        public static void openExcelApp()
        {
            //OPEN NEW EXCEL TEMPLATE
            if ((xlsApp == null))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = blVisibleExcel;
            }
        }


        public static void openExcelWorkBook()
        {
            xlsWB = xlsApp.Workbooks.Open(strExcelTemplate, UpdateLinks: 0);
            xlsWB.CheckCompatibility = false;
        }



        public static void addValueToCell(string strSheetName, string strCell, string strValue)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();
            xlsSheet.Range[strCell].Value = strValue;
        }

        public static void addFocusToCell(string strSheetName, string strCell)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();
            xlsSheet.Range[strCell].Select();
        }

        public static void deleteWorksheet(string strSheetName)
        {
            xlsWB.Application.DisplayAlerts = false;
            xlsWB.Sheets[strSheetName].Delete();
            xlsWB.Application.DisplayAlerts = true;
        }



        public static void boldSection(string strSheetName, string strStartRow, string strEndRow, string strCell)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            string strRange = GetExcelColumnName(strCell) + strStartRow + ":" + GetExcelColumnName(strCell) + strEndRow;

            xlsSheet.Range[strRange].Select();
            xlsApp.Selection.Font.Bold = true;


        }

        public static void populateTable(DataTable dt, string strSheetName, int intStartExcelRow, char chrStartCell)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();


            Int16 intCnt = 1;

            if ((dt.Rows.Count > 0))
            {
                char chrCurrentCell = chrStartCell;

                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        xlsSheet.Range[chrCurrentCell + intStartExcelRow].Value = (!object.ReferenceEquals(dr[column.ColumnName.ToString()], DBNull.Value) ? dr[column.ColumnName.ToString()] : "");

                        chrCurrentCell = Convert.ToChar(Convert.ToInt32(chrCurrentCell) + 1);

                    }

                    chrCurrentCell = chrStartCell;
                    intStartExcelRow += 1;
                    intCnt += 1;
                }

            }


        }



        public static void AutoFitRange(string strSheetRange, string strSheetName)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            xlsSheet.Range[strSheetRange].Columns.AutoFit();

        }


        public static void deleteRows(string strSheetRange, string strSheetName)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            xlsSheet.Range[strSheetRange].Select();

            xlsApp.Selection.Delete(Shift: Excel.XlDirection.xlUp);
        }


        public static void addBorders(string strSheetRange, string strSheetName)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            xlsSheet.Range[strSheetRange].Select();

            xlsApp.Selection.Borders(Excel.XlBordersIndex.xlDiagonalDown).LineStyle = Excel.Constants.xlNone;
            xlsApp.Selection.Borders(Excel.XlBordersIndex.xlDiagonalUp).LineStyle = Excel.Constants.xlNone;
            var _with2 = xlsApp.Selection.Borders(Excel.XlBordersIndex.xlEdgeLeft);
            _with2.LineStyle = Excel.XlLineStyle.xlContinuous;
            _with2.Weight = Excel.XlBorderWeight.xlThin;
            _with2.ColorIndex = Excel.Constants.xlAutomatic;
            var _with3 = xlsApp.Selection.Borders(Excel.XlBordersIndex.xlEdgeTop);
            _with3.LineStyle = Excel.XlLineStyle.xlContinuous;
            _with3.Weight = Excel.XlBorderWeight.xlThin;
            _with3.ColorIndex = Excel.Constants.xlAutomatic;
            var _with4 = xlsApp.Selection.Borders(Excel.XlBordersIndex.xlEdgeBottom);
            _with4.LineStyle = Excel.XlLineStyle.xlContinuous;
            _with4.Weight = Excel.XlBorderWeight.xlThin;
            _with4.ColorIndex = Excel.Constants.xlAutomatic;
            var _with5 = xlsApp.Selection.Borders(Excel.XlBordersIndex.xlEdgeRight);
            _with5.LineStyle = Excel.XlLineStyle.xlContinuous;
            _with5.Weight = Excel.XlBorderWeight.xlThin;
            _with5.ColorIndex = Excel.Constants.xlAutomatic;

            try
            {
                var _with6 = xlsApp.Selection.Borders(Excel.XlBordersIndex.xlInsideVertical);
                _with6.LineStyle = Excel.XlLineStyle.xlContinuous;
                _with6.Weight = Excel.XlBorderWeight.xlThin;
                _with6.ColorIndex = Excel.Constants.xlAutomatic;

            }
            catch (Exception ex)
            {
            }

            try
            {
                var _with7 = xlsApp.Selection.Borders(Excel.XlBordersIndex.xlInsideHorizontal);
                _with7.LineStyle = Excel.XlLineStyle.xlContinuous;
                _with7.Weight = Excel.XlBorderWeight.xlThin;
                _with7.ColorIndex = Excel.Constants.xlAutomatic;

            }
            catch (Exception ex)
            {
            }




        }

        public static string GetExcelColumnName(string strColumnNumber)
        {
            int dividend = int.Parse(strColumnNumber);
            string columnName = String.Empty;
            int modulo = 0;
            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = Convert.ToInt32((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static void closeExcelWorkbook(string strFinalReportFileName, string profileType)
        {
            //IF WE WANT TO SAVE THE EXCEL FILES
            if ((blSaveExcel))
            {
                //FINAL REPORT NAME
                dynamic strReportPathTmp = strReportsPath.Replace("{$profileType}", profileType);


                string strFinalPath = strReportPathTmp + "excel\\" + strFinalReportFileName + ".xlsx";


                if ((!System.IO.Directory.Exists(strReportPathTmp + "excel\\")))
                {
                    System.IO.Directory.CreateDirectory(strReportPathTmp + "excel\\");
                }

                if ((File.Exists(strFinalPath)))
                {
                    File.Delete(strFinalPath);
                }
                //xlsWB.SaveAs(strFinalReportDirName & strFinalReportFileName, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal)
                xlsWB.SaveAs(strFinalPath, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook);
            }

            try
            {
                xlsSheet = null;
                if ((xlsWB != null))
                {
                    xlsWB.Close(false);
                    xlsWB = null;
                }


                releaseObject(xlsSheet);
                releaseObject(xlsWB);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }


        }

        public static void CloneAsPDF(string strPath, object[] strSheetsArr, object[] strRangeArr)
        {
            string strFinalPath = strReportsPath + "PDF" + "\\";

            if ((!System.IO.Directory.Exists(strFinalPath)))
            {
                System.IO.Directory.CreateDirectory(strFinalPath);
            }

            strFinalPath += strPath + ".pdf";


            //Excel.Worksheet sh = default(Excel.Worksheet);
            xlsApp.PrintCommunication = false;
            // Cycle through each sheet
            Int16 intCnt = 0;
            foreach (Excel.Worksheet sh in xlsWB.Sheets[strSheetsArr])
            {
                // Set print area to used range of sheet
                //sh.PageSetup.PrintArea = sh.UsedRange.Address
                sh.PageSetup.PrintArea = "A1:" + strRangeArr[intCnt];
                // Remove zoom, scale sheet to fit 1 page
                var _with9 = sh.PageSetup;
                _with9.CenterHorizontally = true;
                //.CenterVertically = True
                _with9.Zoom = false;
                _with9.FitToPagesWide = sh.PageSetup.FitToPagesWide;
                //.FitToPagesTall = sh.PageSetup.FitToPagesTall
                _with9.FitToPagesTall = 30;
                _with9.PrintTitleRows = sh.PageSetup.PrintTitleRows;
                _with9.PrintTitleColumns = sh.PageSetup.PrintTitleColumns;
                intCnt += 1;
            }

            // Enable PrintCommunication to apply settings
            xlsApp.PrintCommunication = true;


            xlsWB.Sheets[strSheetsArr].Select();

            xlsWB.ActiveSheet.ExportAsFixedFormat(Type: Excel.XlFixedFormatType.xlTypePDF, Filename: strFinalPath, Quality: Excel.XlFixedFormatQuality.xlQualityStandard, IncludeDocProperties: true, IgnorePrintAreas: false, OpenAfterPublish: false);

        }



        public static void closeExcelApp()
        {

            try
            {
                if ((xlsApp != null))
                {
                    xlsApp.Quit();
                    xlsApp = null;
                }

                releaseObject(xlsApp);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }


        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
            }
        }

    }
}



