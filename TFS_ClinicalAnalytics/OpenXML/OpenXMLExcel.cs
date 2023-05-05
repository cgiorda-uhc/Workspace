using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace OpenXMLExcel
{
    public class OpenXMLExcel
    {
        //MERGED SO IGNORE THESE IF NOT NULL
        //SEEMS LIKE HACK, FIND BETTER WAY!!!
        public static string[] strCellsToIgnoreArr = null;

        public static SheetData sheetData = null;
        public static WorkbookPart workbookPart = null;

        public static DataTable ReadAsDataTable(SpreadsheetDocument spreadSheetDocument, string strSheetName, int intStartingColumnRow = 1, int intStartingDataRow = 2, int intStartingDataCell = 0, bool blSheetsWild = false, bool blNullColumns = false)
        {
            DataTable dataTable = new DataTable();

            workbookPart = spreadSheetDocument.WorkbookPart;
            // IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();

            //WorkbookPart bkPart = spreadSheetDocument.WorkbookPart;
            DocumentFormat.OpenXml.Spreadsheet.Workbook workbook = workbookPart.Workbook;

            Sheet wsSheet;

            if(!blSheetsWild) //DEFAULT!!!
            {
                wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name == strSheetName).FirstOrDefault();
                if(wsSheet == null)
                    wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name.ToString().ToLower().Equals(strSheetName.ToLower())).FirstOrDefault();
            } 
            else
                wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name.ToString().ToLower().StartsWith(strSheetName.ToLower())).FirstOrDefault();



            //string relationshipId = sheets.First().Id.Value;
            string relationshipId = wsSheet.Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();



            //HANDLE MERGED CELLS DYNAMICALLY!!!
            //if (worksheetPart.Worksheet.Elements<MergeCells>().Count() > 0)
            //{
            //    MergeCells mergeCells = worksheetPart.Worksheet.Elements<MergeCells>().First();
            //    foreach (MergeCell mc in mergeCells)
            //    {
            //        if (mc.Reference.InnerText.Contains(intStartingDataRow.ToString()))
            //        {
            //            var theCell = worksheetPart.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>().
            //               Where(c => c.CellReference == "C3").FirstOrDefault();
            //            var val = GetCellValue(spreadSheetDocument, theCell);
            //        }
            //    }
            //}




            int intNullCnt = 1;
            //foreach (Cell cell in rows.ElementAt(0))
            foreach (Cell cell in rows.ElementAt(intStartingColumnRow - 1))
            {
                var value = GetCellValue(spreadSheetDocument, cell);
                //if (String.IsNullOrEmpty(value))
                if (value == null && blNullColumns)
                {
                    value = "Column" + intNullCnt;
                    intNullCnt++;
                }
      
                if (value == null)
                continue;

                if(dataTable.Columns.Contains(value.Trim()))
                    dataTable.Columns.Add(value.Trim() + "1");
                else
                    dataTable.Columns.Add(value.Trim());
            }

            // skip the part that retrieves the worksheet sheetData
            int intcnt = 0;
            int cellCnt = 0;
            bool blAllBlank = true;
            Int16 intBlankCnt = 0;
            Int16 intBlankLimit = 10;
            foreach (Row row in rows)
            {

                if (intcnt < intStartingDataRow - 1)
                {
                    intcnt++;
                    continue;
                }

                DataRow dataRow = dataTable.NewRow();
                IEnumerable<Cell> cells = GetRowCells(row, intStartingDataCell);
                cellCnt = 0;
                blAllBlank = true;
                //foreach (Cell cell in cells)
                foreach (Cell cell in cells)
                {
                    if (cellCnt >= dataTable.Columns.Count)
                        continue;

                    // skip part that reads the text according to the cell-type
                    var val = GetCellValue(spreadSheetDocument, cell);

                    if (!string.IsNullOrEmpty(val))
                        blAllBlank = false;

                    dataRow[cellCnt] = val;
                    cellCnt++;
                }

                if (!blAllBlank)
                {
                    dataTable.Rows.Add(dataRow);
                    intBlankCnt = 0;
                }
                else
                    intBlankCnt++;


                if (intBlankCnt == intBlankLimit)
                {
                    break;
                }

            }


            //dataTable.Rows.RemoveAt(0);

            return dataTable;
        }


        public static Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
            }
            return theSheets;
        }

        private static IEnumerable<Cell> GetRowCells(Row row, int intStartingDataCell = 0)
        {
            int currentCount = intStartingDataCell;

            foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in
                row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>())
            {

                string columnName = GetColumnName(cell.CellReference);

                //MERGED CHECKER
                if (strCellsToIgnoreArr != null)
                {
                    //MERGED SO IGNORE THESE
                    if (strCellsToIgnoreArr.Contains(columnName))
                    {
                        currentCount++;
                        continue;
                    }

                }

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    yield return new DocumentFormat.OpenXml.Spreadsheet.Cell();
                }

                yield return cell;
                currentCount++;
            }
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        private static string GetColumnName(string cellReference)
        {
            // Match the column name portion of the cell name.
            var regex = new System.Text.RegularExpressions.Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }

        /// <summary>
        /// Given just the column name (no row index),
        /// it will return the zero based column index.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful</returns>
        /// <exception cref="ArgumentException">thrown if the given string
        /// contains characters other than uppercase letters</exception>
        private static int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            int convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = (cell.CellValue != null ? cell.CellValue.InnerXml : null);

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }




        public static Cell GetCell(SheetData sheetData, string cellAddress)
        {
            uint rowIndex = uint.Parse(Regex.Match(cellAddress, @"[0-9]+").Value);
            return sheetData.Descendants<Row>().FirstOrDefault(p => p.RowIndex == rowIndex).Descendants<Cell>().FirstOrDefault(p => p.CellReference == cellAddress);
        }

        public static string GetCellValue(Cell cell, WorkbookPart wbPart)
        {
            string value = cell.InnerText;
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                        var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        if (stringTable != null)
                        {
                            value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                        }
                        break;

                    case CellValues.Boolean:
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                        break;
                }
            }
            return value;
        }



        public static void ConvertFromXLSBToXLSX(string filepath, string filepathFinal)
        {

                try
                {
                    Microsoft.Office.Interop.Excel.Application excelApplication = new Microsoft.Office.Interop.Excel.Application();
                    excelApplication.DisplayAlerts = false;
                    Microsoft.Office.Interop.Excel.Workbooks workbooks = excelApplication.Workbooks;
                    // open book in any format
                    Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Open(filepath, Microsoft.Office.Interop.Excel.XlUpdateLinks.xlUpdateLinksNever, true, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    // save in XlFileFormat.xlExcel12 format which is XLSB
                    workbook.SaveAs(filepathFinal, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    // close workbook
                    workbook.Close(false, Type.Missing, Type.Missing);

                    excelApplication.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                    //{
                    //    proc.Kill();
                    //}
                }
  
        }



        private static void fixCorrupedXLSX(string filepath)
        {

            try
            {
                Microsoft.Office.Interop.Excel.Application excelApplication = new Microsoft.Office.Interop.Excel.Application();
                //Missing missing = Missing.Value;
                excelApplication.DisplayAlerts = false;
                //Microsoft.Office.Interop.Excel.Workbooks workbooks = excelApplication.Workbooks;
                // open book in any format
                //Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Open(filepath, Microsoft.Office.Interop.Excel.XlUpdateLinks.xlUpdateLinksNever, true, Type.Missing, Type.Missing, Type.Missing,
                   // Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // save in XlFileFormat.xlExcel12 format which is XLSB
                //workbook.SaveAs(filepath.Replace(".xls", "CSG.xls"), Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                //// close workbook
                //workbook.Close(false, Type.Missing, Type.Missing);




                Microsoft.Office.Interop.Excel.Workbook workbook = excelApplication.Workbooks.Open(filepath,Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlCorruptLoad.xlRepairFile);
                workbook.SaveAs(filepath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workbook.Close(true, Type.Missing, Type.Missing);



                excelApplication.DisplayAlerts = true;
                excelApplication.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                //{
                //    proc.Kill();
                //}
            }

        }




        //public static DataTable ConvertExcelToDataTable(string FileName, string strSheetname, string strStart = "", string strWhere = "")
        //{
        //    DataTable dtResult = null;
        //    System.Data.OleDb.OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");

        //    try
        //    {
        //        objConn.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message == "External table is not in the expected format.")
        //        {
        //            fixCorrupedXLSX(FileName);

        //            objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName.Replace(".xls", "CSG.xls") + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
        //            objConn.Open();
        //        }
        //    }

        //    OleDbCommand cmd = new OleDbCommand();
        //    OleDbDataAdapter oleda = new OleDbDataAdapter();
        //    DataSet ds = new DataSet();
        //    DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //    cmd.Connection = objConn;
        //    cmd.CommandType = CommandType.Text;
        //    //cmd.CommandText = "SELECT * FROM [" + strSheetname + "$]";
        //    cmd.CommandText = "SELECT * FROM [" + strSheetname + "$" + strStart + "]" + strWhere;
        //    oleda = new OleDbDataAdapter(cmd);
        //    oleda.Fill(ds, "excelData");
        //    dtResult = ds.Tables["excelData"];
        //    objConn.Close();
        //    return dtResult; //Returning Dattable  
        //}
        public static OleDbDataReader ConvertExcelToDataReader(string FileName, string strSheetname, string strStart = "", string strWhere = "", string strColumns = "*", string limit = null)
        {
 
            //int totalSheet = 0; //No of sheets on excel file  

            //NEED ACE HEADER ROW FOR WHERE CLAUSE
            string strHDR = "NO";
            if (strWhere != "" || strColumns != "*")
                strHDR = "YES";

            string strTop = "";
            if(limit != null)
                strTop = " TOP " + limit;

            System.Data.OleDb.OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
          

                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                OleDbDataReader oleDr = null;
                //string sheetName = string.Empty;
                //if (dt != null)
                //{
                //    var tempDataTable = (from dataRow in dt.AsEnumerable()
                //                         where !dataRow["TABLE_NAME"].ToString().Contains("FilterDatabase")
                //                         select dataRow).CopyToDataTable();
                //    dt = tempDataTable;
                //    totalSheet = dt.Rows.Count;
                //    sheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                //}
                cmd.Connection = objConn;
                cmd.CommandType = CommandType.Text;
                //cmd.CommandText = "SELECT * FROM [" + strSheetname + "$]";
                cmd.CommandText = "SELECT " + strTop  + " " + strColumns + " FROM [" + strSheetname + "$" + strStart + "]" + strWhere;
                cmd.CommandTimeout = 99999999;
                oleda = new OleDbDataAdapter(cmd);
                try
                {
                    cmd.CommandTimeout = 9999999;
                    oleDr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return oleDr;
           
        }

        public static DataTable ConvertExcelToDataTable(string FileName, string strSheetname, string strStart = "", string strWhere = "", string strColumns = "*")
        {
            DataTable dtResult = null;
            //int totalSheet = 0; //No of sheets on excel file  

            //NEED ACE HEADER ROW FOR WHERE CLAUSE
            string strHDR = "NO";
            if (strWhere != "" || strColumns != "*")
                strHDR = "YES";

            using (System.Data.OleDb.OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR="+ strHDR + ";IMEX=1;';"))
            {

                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //string sheetName = string.Empty;
                //if (dt != null)
                //{
                //    var tempDataTable = (from dataRow in dt.AsEnumerable()
                //                         where !dataRow["TABLE_NAME"].ToString().Contains("FilterDatabase")
                //                         select dataRow).CopyToDataTable();
                //    dt = tempDataTable;
                //    totalSheet = dt.Rows.Count;
                //    sheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                //}
                cmd.Connection = objConn;
                cmd.CommandType = CommandType.Text;
                //cmd.CommandText = "SELECT * FROM [" + strSheetname + "$]";
                cmd.CommandText = "SELECT "+ strColumns + " FROM [" + strSheetname + "$" + strStart + "]" + strWhere;
                cmd.CommandTimeout = 99999999;
                oleda = new OleDbDataAdapter(cmd);
                
                oleda.Fill(ds, "excelData");
                dtResult = ds.Tables["excelData"];
                objConn.Close();

                //MUST CREATE OUR OWN HEADERS!!!
                if (strWhere == "" && strColumns == "*")
                {
                    //THIS MAKES ALL TYPES TO STRING SO NO NULLS FOR MISMATCHES!!!
                    //SINCE WE ADDED  HDR=NO TO HEADER WE MUST MANUALLY NAME EACH COLUMN WITH ROW 1
                    foreach (DataColumn column in dtResult.Columns)
                    {
                        string cName = dtResult.Rows[0][column.ColumnName].ToString();
                        if (!dtResult.Columns.Contains(cName) && cName != "")
                        {
                            column.ColumnName = cName;
                        }
                    }
                    dtResult.Rows[0].Delete();
                    dtResult.AcceptChanges();
                }

                //Microsoft.ACE.OLEDB WONT IGNORE EMPTY ROWS
                //BUT THIS WILL!!!
                dtResult = dtResult.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();


                return dtResult; //Returning Dattable  
            }
        }


    }
}
