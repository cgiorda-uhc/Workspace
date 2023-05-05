
using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
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

        public static Excel.Workbooks xlsWBs = null;

        public static Excel.Range xlsRange = null;

        public static Excel.Worksheet xlsSheet = null;

        public static Excel.Sheets xlsSheets = null;


        private static bool blVisibleExcel;
        private static bool blSaveExcel;

        public static string strReportsPath;

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
               // xlsApp.DisplayAlerts = false;
            }
        }


        public static void openExcelWorkBook()
        {
            //xlsWB = xlsApp.Workbooks.Open(strExcelTemplate,ReadOnly: true, UpdateLinks: 0);
            xlsWBs = xlsApp.Workbooks;
            xlsWB = xlsWBs.Open(strExcelTemplate, UpdateLinks: 0);
            xlsWB.CheckCompatibility = false;


            //if (!xlsWB.ReadOnly)
            //    xlsWB.ChangeFileAccess(Excel.XlFileAccess.xlReadOnly, "Sigmund2010!!");


        }



        public static void addValueToCell(string strSheetName, string strCell, string strValue)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();
            xlsRange = xlsSheet.Range[strCell];
            xlsRange.Value = strValue;
        }

        public static void addFocusToCell(string strSheetName, string strCell)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();
            xlsRange = xlsSheet.Range[strCell];
            xlsRange.Select();
        }

        public static void deleteWorksheet(string strSheetName)
        {
            //xlsWB.Application.DisplayAlerts = false;
            //xlsWB.Sheets[strSheetName].Delete();
            //xlsWB.Application.DisplayAlerts = true;

            xlsApp.DisplayAlerts = false;
            xlsSheet = xlsWB.Sheets[strSheetName];
            xlsSheet.Delete();
            xlsApp.DisplayAlerts = true;

           
        }



        public static void boldSection(string strSheetName, string strStartRow, string strEndRow, string strCell)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            string strRange = GetExcelColumnName(strCell) + strStartRow + ":" + GetExcelColumnName(strCell) + strEndRow;

            xlsRange = xlsSheet.Range[strRange];

            xlsRange.Select();
            xlsApp.Selection.Font.Bold = true;


        }

        public static void populateTable(DataTable dt, string strSheetName, int intStartExcelRow, char chrStartCell, bool insertNewRow = false, char[] chrArrCols = null)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            Int16 intColCnt = 0;
            Int16 intCnt = 1;

            bool blTotal = false;
            object objValue = null;
            if ((dt.Rows.Count > 0))
            {
                char chrCurrentCell = chrStartCell;

                foreach (DataRow dr in dt.Rows)
                {
                    blTotal = false;


                    if (insertNewRow && intCnt > 1)
                    {
                        xlsSheet.Rows[intStartExcelRow].EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);


                        xlsSheet.Rows[intStartExcelRow].EntireRow.Offset(-1, 0).EntireRow.Copy();
                        xlsSheet.Rows[intStartExcelRow].EntireRow.Offset(0, 0).PasteSpecial(Excel.XlPasteType.xlPasteFormats);
                        xlsApp.CutCopyMode = Excel.XlCutCopyMode.xlCopy;

                    }
                        

                    foreach (DataColumn column in dt.Columns)
                    {

                        if(chrArrCols != null)
                        {
                            chrCurrentCell = chrArrCols[intColCnt];
                        }



                        xlsRange = xlsSheet.Range[chrCurrentCell.ToString() + intStartExcelRow.ToString()];

                        objValue = (!object.ReferenceEquals(dr[column.ColumnName.ToString()], DBNull.Value) ? dr[column.ColumnName.ToString()] : "");
                        xlsRange.Value = objValue;

                        if(objValue.GetType() == typeof(String))
                        {
                            if (objValue.ToString().ToUpper().Equals("TOTAL"))
                                blTotal = true;

                        }


                        xlsRange.Font.Bold = blTotal;


                        chrCurrentCell = Convert.ToChar(Convert.ToInt32(chrCurrentCell) + 1);
                        intColCnt++;
                    }

                    chrCurrentCell = chrStartCell;
                    intStartExcelRow += 1;
                    intCnt += 1;
                    intColCnt = 0;
                }

            }


        }



        public static void AutoFitRange(string strSheetRange, string strSheetName)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();
            xlsRange = xlsSheet.Range[strSheetRange];
            xlsRange.Columns.EntireColumn.AutoFit();
            //xlsRange.Rows.EntireRow.AutoFit();
        }


        public static void deleteRows(string strSheetRange, string strSheetName)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            xlsRange = xlsSheet.Range[strSheetRange];
            xlsRange.Select();

            xlsApp.Selection.Delete(Shift: Excel.XlDirection.xlUp);
        }


        public static void addBorders(string strSheetRange, string strSheetName)
        {
            xlsSheet = xlsWB.Worksheets[strSheetName];
            xlsSheet.Activate();

            //xlsSheet.Range[strSheetRange].Select();
            xlsRange = xlsSheet.Range[strSheetRange];
            xlsRange.Select();

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

        public static void closeExcelWorkbook()
        {
            xlsWB.Close(false);
        }



            public static void closeExcelWorkbook(string strFinalReportFileName, string profileType)
        {
            //IF WE WANT TO SAVE THE EXCEL FILES
            if ((blSaveExcel))
            {
                //FINAL REPORT NAME
                dynamic strReportPathTmp = strReportsPath.Replace("{$profileType}", profileType);


                string strFinalPath = strReportPathTmp + "\\excel\\" + strFinalReportFileName + ".xlsx";


                if ((!System.IO.Directory.Exists(strReportPathTmp + "\\excel\\")))
                {
                    System.IO.Directory.CreateDirectory(strReportPathTmp + "\\excel\\");
                }

                if ((File.Exists(strFinalPath)))
                {
                    File.Delete(strFinalPath);
                }

                //xlsWB.ChangeFileAccess(Excel.XlFileAccess.xlReadWrite, "Sigmund2010!!");
                //xlsWB.SaveAs(strFinalReportDirName & strFinalReportFileName, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal)
                xlsWB.SaveAs(strFinalPath, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook);

                 //xlsWB.Unprotect();
                //xlsWB.s= false;

                //xlsWB.SaveAs(strFinalPath, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook, Password: Type.Missing, WriteResPassword: Type.Missing, ReadOnlyRecommended: false, CreateBackup: false, AccessMode: Excel.XlSaveAsAccessMode.xlShared);


                //xlsWB.SaveAs(strFinalPath, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook,Password:Type.Missing, WriteResPassword: "Sigmund2010!!",ReadOnlyRecommended:false,CreateBackup: false,AccessMode:Excel.XlSaveAsAccessMode.xlShared);
            }

            //try
            //{


                GC.Collect();
                GC.WaitForPendingFinalizers();



                if (xlsRange != null)
                    Marshal.ReleaseComObject(xlsRange);
                xlsRange = null;

                if (xlsSheet != null)
                    Marshal.ReleaseComObject(xlsSheet);
                xlsSheet = null;


                if (xlsSheets != null)
                    Marshal.ReleaseComObject(xlsSheets);
                xlsSheets = null;

                if (xlsWBs != null)
                {
                    //xlsWBs.Close(); ;
                    Marshal.ReleaseComObject(xlsWBs);
                }
                xlsWBs = null;

                if (xlsWB != null)
                {
                    xlsWB.Close(false); ;
                    Marshal.ReleaseComObject(xlsWB);
                }
                xlsWB = null;


                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();


            //}
            //catch (Exception ex)
            //{
            //}
            //finally
            //{
            //}


        }

        public static string CloneAsPDF(string strPath, string strSheetName, string strRange )
        {
            string strFinalPath = strReportsPath + "\\";
            //string strFinalPath = strReportsPath + "\\PDF" + "\\";

            if ((!System.IO.Directory.Exists(strFinalPath)))
            {
                System.IO.Directory.CreateDirectory(strFinalPath);
            }

            strFinalPath += strPath + ".pdf";


            //Excel.Worksheet sh = default(Excel.Worksheet);
            xlsApp.PrintCommunication = false;
            // Cycle through each sheet
            Int16 intCnt = 0;
            Excel.Worksheet sh = xlsWB.Sheets[strSheetName];
           
            // Set print area to used range of sheet
            //sh.PageSetup.PrintArea = sh.UsedRange.Address
            sh.PageSetup.PrintArea = "A1:" + strRange;
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
    

            // Enable PrintCommunication to apply settings
            xlsApp.PrintCommunication = true;

            sh.Select();

            xlsWB.ActiveSheet.ExportAsFixedFormat(Type: Excel.XlFixedFormatType.xlTypePDF, Filename: strFinalPath, Quality: Excel.XlFixedFormatQuality.xlQualityStandard, IncludeDocProperties: true, IgnorePrintAreas: false, OpenAfterPublish: true);



            //xlsWB.Sheets(strSheetsArr).Select

            //xlsWB.ActiveSheet.ExportAsFixedFormat(Type:= Excel.XlFixedFormatType.xlTypePDF, Filename:= strFinalPath, Quality:= Excel.XlFixedFormatQuality.xlQualityStandard, IncludeDocProperties:= True, IgnorePrintAreas:= False, OpenAfterPublish:= False)

            return strFinalPath;


        }


        public static string CloneAsPDF(string strPath, object[] strSheetsArr, object[] strRangeArr)
        {
            string strFinalPath = (strReportsPath + ("PDF" + "\\"));
            if (!System.IO.Directory.Exists(strFinalPath))
            {
                System.IO.Directory.CreateDirectory(strFinalPath);
            }

            strFinalPath = (strFinalPath
                        + (strPath + ".pdf"));
            xlsApp.PrintCommunication = false;
            Int16 intCnt = 0;
            foreach (Excel.Worksheet  sh in xlsWB.Sheets[strSheetsArr])
            {
                //  Set print area to used range of sheet
                // sh.PageSetup.PrintArea = sh.UsedRange.Address
                sh.PageSetup.PrintArea = ("A1:" + strRangeArr[intCnt]);
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

            //  Enable PrintCommunication to apply settings
            xlsApp.PrintCommunication = true;
            xlsWB.Sheets[strSheetsArr].Select();
            xlsWB.ActiveSheet.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, strFinalPath, Excel.XlFixedFormatQuality.xlQualityStandard, true, false, false);

            return strFinalPath;


        }

        public static void closeExcelApp()
        {

            //try
            //{

            xlsWB.Saved = true;
                GC.Collect();
                GC.WaitForPendingFinalizers();


                if (xlsRange != null)
                {
                    Marshal.ReleaseComObject(xlsRange);
                    xlsRange = null;
                }

                if (xlsSheet != null)
                {
                    Marshal.ReleaseComObject(xlsSheet);
                    xlsSheet = null;
                }

                if (xlsSheets != null)
                {
                    Marshal.ReleaseComObject(xlsSheets);
                    xlsSheets = null;
                }

                if (xlsWBs != null)
                {
                    xlsWBs.Close(); 
                    Marshal.ReleaseComObject(xlsWBs);
                    xlsWBs = null;
                }
                

                if (xlsWB != null)
                {
                    //xlsWB.Close(false); 
                    Marshal.ReleaseComObject(xlsWB);
                    xlsWB = null;
                }
                

                if ((xlsApp != null))
                {
                    xlsApp.Quit();
                    //Marshal.FinalReleaseComObject(xlsApp);
                    Marshal.ReleaseComObject(xlsApp);
                    xlsApp = null;
                }



                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();




            //}
            //catch (Exception ex)
            //{
            //}
            //finally
            //{
            //}


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



