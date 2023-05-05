using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using System.Text;
using System.Threading.Tasks;

namespace UCS_Project_Manager
{
    public class ExcelHelper
    {
        static string strSheetNameGLOBAL = "ETG_Summary";

        public static void ExportGenericListClosedXML<T>(List<T> lst, string destination)
        {

            //GENERIC TO STRONG
            Type elementType = lst.GetType().GetGenericArguments()[0];
            IList<PropertyInfo> props = new List<PropertyInfo>(elementType.GetProperties());
            string strCurrentRange;
            int intColumnCnt;
            int intRowCnt;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(strSheetNameGLOBAL);

                //HEADERS
                intRowCnt = 1;
                intColumnCnt = 1;
                foreach (var prop in props)
                {

                    strCurrentRange = worksheet.Range(worksheet.Cell(intRowCnt, intColumnCnt), worksheet.Cell(intRowCnt, intColumnCnt)).RangeAddress.ToString();
                    worksheet.Range(strCurrentRange).Value = prop.Name;

                    //worksheet.Range(strCurrentRange).Style.Alignment.WrapText = true;

                    worksheet.Range(strCurrentRange).Style.Font.Bold = true;
                    worksheet.Range(strCurrentRange).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Range(strCurrentRange).Style.Fill.BackgroundColor = XLColor.Yellow;
                    worksheet.Range(strCurrentRange).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(strCurrentRange).Style.Border.BottomBorderColor = XLColor.Black;
                    worksheet.Range(strCurrentRange).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(strCurrentRange).Style.Border.LeftBorderColor = XLColor.Black;
                    worksheet.Range(strCurrentRange).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(strCurrentRange).Style.Border.TopBorderColor = XLColor.Black;
                    worksheet.Range(strCurrentRange).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(strCurrentRange).Style.Border.RightBorderColor = XLColor.Black;

                    intColumnCnt++;
                }
                //ROWS
                intRowCnt++;
                intColumnCnt = 1;
                foreach (var row in lst)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (var prop in props)
                    {
                        var value = row.GetType().GetProperty(prop.Name).GetValue(row, null) + "";
                        strCurrentRange = worksheet.Range(worksheet.Cell(intRowCnt, intColumnCnt), worksheet.Cell(intRowCnt, intColumnCnt)).RangeAddress.ToString();
                        worksheet.Range(strCurrentRange).Value = value;
                        intColumnCnt++;
                    }
                    intRowCnt++;
                    intColumnCnt = 1;
                }


                //worksheet.Columns().AdjustToContents();

                //workbook.Save(true);

                workbook.SaveAs(destination, true);
            }
        }

        public static void ExportGenericListOpenXML<T>(List<T> lst, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                //COMMENTED FOR AUTOSIZE BELOW
                //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                //Stylesheet stylesheet = styles.Stylesheet;
                //CellFormats cellformats = stylesheet.CellFormats;
                //Fonts fonts = stylesheet.Fonts;


                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                uint sheetId = 1;
                if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                {
                    sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = strSheetNameGLOBAL };
                sheets.Append(sheet);

                DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


                //ADD COLORS
                WorkbookStylesPart styles = workbookPart.AddNewPart<WorkbookStylesPart>();
                styles.Stylesheet = CreateStylesheet();
                styles.Stylesheet.Save();




                //GENERIC TO STRONG
                Type elementType = lst.GetType().GetGenericArguments()[0];
                IList<PropertyInfo> props = new List<PropertyInfo>(elementType.GetProperties());

                List<String> col = new List<string>();
                int intColCnt = 1;
                foreach (var prop in props)
                {
                    col.Add(prop.Name);

                    Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                    intColCnt++;

                    //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                    headerRow.AppendChild(cell);
                }


                sheetData.AppendChild(headerRow);

                foreach (var row in lst)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (String c in col)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                //ADDED TO AUTOSIZE
                Columns columns = AutoSize(sheetData);
                sheetPart.Worksheet = new Worksheet();
                sheetPart.Worksheet.Append(columns);
                sheetPart.Worksheet.Append(sheetData);



            }
        }


        public static void ExportMultipleGenericListOpenXML(List<ETG_Fact_Symmetry_Export_Model> lst1, List<ETG_Fact_Symmetry_PateintCentric> lst2, List<ETG_Fact_Symmetry_Config_Model> lst3, List<ETG_Fact_Symmetry_Export_Model2> lst4, List<ETG_Fact_Symmetry_RxNrxConfig_Model> lst5, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();

                // Begin: Code block for Excel sheet 1
                var sheetPart1 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData1 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                //COMMENTED FOR AUTOSIZE BELOW
                //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                //Stylesheet stylesheet = styles.Stylesheet;
                //CellFormats cellformats = stylesheet.CellFormats;
                //Fonts fonts = stylesheet.Fonts;



                string relationshipId1 = workbook.WorkbookPart.GetIdOfPart(sheetPart1);

                uint sheetId = 1;
                if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                {
                    sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet1 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId1, SheetId = sheetId, Name = strSheetNameGLOBAL };
                sheets.Append(sheet1);

                DocumentFormat.OpenXml.Spreadsheet.Row headerRow1 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                //ADD COLORS
                WorkbookStylesPart styles = workbookPart.AddNewPart<WorkbookStylesPart>();
                styles.Stylesheet = CreateStylesheet();
                styles.Stylesheet.Save();




                //GENERIC TO STRONG
                Type elementType = lst1.GetType().GetGenericArguments()[0];
                IList<PropertyInfo> props = new List<PropertyInfo>(elementType.GetProperties());

                List<String> col = new List<string>();
                int intColCnt = 1;
                foreach (var prop in props)
                {
                    col.Add(prop.Name);

                    Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                    intColCnt++;

                    //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                    headerRow1.AppendChild(cell);
                }


                sheetData1.AppendChild(headerRow1);

                foreach (var row in lst1)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (String c in col)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                        newRow.AppendChild(cell);
                    }

                    sheetData1.AppendChild(newRow);
                }


                //ADDED TO AUTOSIZE
                Columns columns = AutoSize(sheetData1);
                sheetPart1.Worksheet = new Worksheet();
                sheetPart1.Worksheet.Append(columns);
                sheetPart1.Worksheet.Append(sheetData1);
                // End: Code block for Excel sheet 1



                //SHEET 4 ATTEMPT
                if (lst4 != null)
                {
                    // Begin: Code block for Excel sheet 1
                    var sheetPart4 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData4 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    //COMMENTED FOR AUTOSIZE BELOW
                    //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                    //Stylesheet stylesheet = styles.Stylesheet;
                    //CellFormats cellformats = stylesheet.CellFormats;
                    //Fonts fonts = stylesheet.Fonts;



                    string relationshipId4 = workbook.WorkbookPart.GetIdOfPart(sheetPart4);

                    sheetId = 4;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet4 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId4, SheetId = sheetId, Name = "EPISODE COST" };
                    sheets.Append(sheet4);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow4 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                    //ADD COLORS
                    //WorkbookStylesPart styles3 = workbookPart.AddNewPart<WorkbookStylesPart>();
                    //styles3.Stylesheet = CreateStylesheet();
                    //styles3.Stylesheet.Save();




                    //GENERIC TO STRONG
                    elementType = lst4.GetType().GetGenericArguments()[0];
                    props = new List<PropertyInfo>(elementType.GetProperties());

                    col = new List<string>();
                    intColCnt = 1;
                    foreach (var prop in props)
                    {
                        col.Add(prop.Name);

                        Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                        intColCnt++;

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                        headerRow4.AppendChild(cell);
                    }


                    sheetData4.AppendChild(headerRow4);

                    foreach (var row in lst4)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String c in col)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                            newRow.AppendChild(cell);
                        }

                        sheetData4.AppendChild(newRow);
                    }


                    //ADDED TO AUTOSIZE
                    columns = AutoSize(sheetData4);
                    sheetPart4.Worksheet = new Worksheet();
                    sheetPart4.Worksheet.Append(columns);
                    sheetPart4.Worksheet.Append(sheetData4);
                }


                //SHEET 5 ATTEMPT
                if (lst5 != null && 1 == 2)
                {
                    // Begin: Code block for Excel sheet 1
                    var sheetPart5 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData5 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    //COMMENTED FOR AUTOSIZE BELOW
                    //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                    //Stylesheet stylesheet = styles.Stylesheet;
                    //CellFormats cellformats = stylesheet.CellFormats;
                    //Fonts fonts = stylesheet.Fonts;



                    string relationshipId5 = workbook.WorkbookPart.GetIdOfPart(sheetPart5);

                    sheetId = 5;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet5 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId5, SheetId = sheetId, Name = "RxNRx Coding" };
                    sheets.Append(sheet5);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow5 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                    //ADD COLORS
                    //WorkbookStylesPart styles3 = workbookPart.AddNewPart<WorkbookStylesPart>();
                    //styles3.Stylesheet = CreateStylesheet();
                    //styles3.Stylesheet.Save();




                    //GENERIC TO STRONG
                    elementType = lst5.GetType().GetGenericArguments()[0];
                    props = new List<PropertyInfo>(elementType.GetProperties());

                    col = new List<string>();
                    intColCnt = 1;
                    foreach (var prop in props)
                    {
                        col.Add(prop.Name);

                        Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                        intColCnt++;

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                        headerRow5.AppendChild(cell);
                    }


                    sheetData5.AppendChild(headerRow5);

                    foreach (var row in lst5)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String c in col)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                            newRow.AppendChild(cell);
                        }

                        sheetData5.AppendChild(newRow);
                    }


                    //ADDED TO AUTOSIZE
                    columns = AutoSize(sheetData5);
                    sheetPart5.Worksheet = new Worksheet();
                    sheetPart5.Worksheet.Append(columns);
                    sheetPart5.Worksheet.Append(sheetData5);
                }

                //SHEET 2 ATTEMPT
                if (lst2 != null)
                {
                    // Begin: Code block for Excel sheet 1
                    var sheetPart2 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData2 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    //COMMENTED FOR AUTOSIZE BELOW
                    //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                    //Stylesheet stylesheet = styles.Stylesheet;
                    //CellFormats cellformats = stylesheet.CellFormats;
                    //Fonts fonts = stylesheet.Fonts;



                    string relationshipId2 = workbook.WorkbookPart.GetIdOfPart(sheetPart2);

                    sheetId = 2;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet2 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId2, SheetId = sheetId, Name = "PATIENT_CENTRIC" };
                    sheets.Append(sheet2);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow2 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                    //ADD COLORS
                    //WorkbookStylesPart styles2 = workbookPart.AddNewPart<WorkbookStylesPart>();
                    //styles2.Stylesheet = CreateStylesheet();
                    //styles2.Stylesheet.Save();




                    //GENERIC TO STRONG
                    elementType = lst2.GetType().GetGenericArguments()[0];
                    props = new List<PropertyInfo>(elementType.GetProperties());

                    col = new List<string>();
                    intColCnt = 1;
                    foreach (var prop in props)
                    {
                        col.Add(prop.Name);

                        Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                        intColCnt++;

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                        headerRow2.AppendChild(cell);
                    }


                    sheetData2.AppendChild(headerRow2);

                    foreach (var row in lst2)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String c in col)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                            newRow.AppendChild(cell);
                        }

                        sheetData2.AppendChild(newRow);
                    }


                    //ADDED TO AUTOSIZE
                    columns = AutoSize(sheetData2);
                    sheetPart2.Worksheet = new Worksheet();
                    sheetPart2.Worksheet.Append(columns);
                    sheetPart2.Worksheet.Append(sheetData2);
                }


                //SHEET 3 ATTEMPT
                if (lst3 != null)
                {
                    // Begin: Code block for Excel sheet 1
                    var sheetPart3 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData3 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    //COMMENTED FOR AUTOSIZE BELOW
                    //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                    //Stylesheet stylesheet = styles.Stylesheet;
                    //CellFormats cellformats = stylesheet.CellFormats;
                    //Fonts fonts = stylesheet.Fonts;



                    string relationshipId3 = workbook.WorkbookPart.GetIdOfPart(sheetPart3);

                    sheetId = 3;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet3 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId3, SheetId = sheetId, Name = "POP_EPISODE" };
                    sheets.Append(sheet3);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow3 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                    //ADD COLORS
                    //WorkbookStylesPart styles3 = workbookPart.AddNewPart<WorkbookStylesPart>();
                    //styles3.Stylesheet = CreateStylesheet();
                    //styles3.Stylesheet.Save();




                    //GENERIC TO STRONG
                    elementType = lst3.GetType().GetGenericArguments()[0];
                    props = new List<PropertyInfo>(elementType.GetProperties());

                    col = new List<string>();
                    intColCnt = 1;
                    foreach (var prop in props)
                    {
                        col.Add(prop.Name);

                        Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                        intColCnt++;

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                        headerRow3.AppendChild(cell);
                    }


                    sheetData3.AppendChild(headerRow3);

                    foreach (var row in lst3)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String c in col)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                            newRow.AppendChild(cell);
                        }

                        sheetData3.AppendChild(newRow);
                    }


                    //ADDED TO AUTOSIZE
                    columns = AutoSize(sheetData3);
                    sheetPart3.Worksheet = new Worksheet();
                    sheetPart3.Worksheet.Append(columns);
                    sheetPart3.Worksheet.Append(sheetData3);
                }





            }
        }


        public static void ExportMultipleGenericListOpenXML(List<ETG_Fact_Symmetry_Export_Model> lst1, List<ETG_Fact_Symmetry_Export_Model2> lst2, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();

                // Begin: Code block for Excel sheet 1
                var sheetPart1 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData1 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                //COMMENTED FOR AUTOSIZE BELOW
                //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                //Stylesheet stylesheet = styles.Stylesheet;
                //CellFormats cellformats = stylesheet.CellFormats;
                //Fonts fonts = stylesheet.Fonts;



                string relationshipId1 = workbook.WorkbookPart.GetIdOfPart(sheetPart1);

                uint sheetId = 1;
                if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                {
                    sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet1 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId1, SheetId = sheetId, Name = strSheetNameGLOBAL };
                sheets.Append(sheet1);

                DocumentFormat.OpenXml.Spreadsheet.Row headerRow1 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                //ADD COLORS
                WorkbookStylesPart styles = workbookPart.AddNewPart<WorkbookStylesPart>();
                styles.Stylesheet = CreateStylesheet();
                styles.Stylesheet.Save();




                //GENERIC TO STRONG
                Type elementType = lst1.GetType().GetGenericArguments()[0];
                IList<PropertyInfo> props = new List<PropertyInfo>(elementType.GetProperties());

                List<String> col = new List<string>();
                int intColCnt = 1;
                foreach (var prop in props)
                {
                    col.Add(prop.Name);

                    Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                    intColCnt++;

                    //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                    headerRow1.AppendChild(cell);
                }


                sheetData1.AppendChild(headerRow1);

                foreach (var row in lst1)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (String c in col)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                        newRow.AppendChild(cell);
                    }

                    sheetData1.AppendChild(newRow);
                }


                //ADDED TO AUTOSIZE
                Columns columns = AutoSize(sheetData1);
                sheetPart1.Worksheet = new Worksheet();
                sheetPart1.Worksheet.Append(columns);
                sheetPart1.Worksheet.Append(sheetData1);
                // End: Code block for Excel sheet 1



                //SHEET 4 ATTEMPT
                if (lst2 != null)
                {
                    // Begin: Code block for Excel sheet 1
                    var sheetPart2 = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData2 = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    //COMMENTED FOR AUTOSIZE BELOW
                    //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    //WorkbookStylesPart styles = workbook.WorkbookPart.WorkbookStylesPart;
                    //Stylesheet stylesheet = styles.Stylesheet;
                    //CellFormats cellformats = stylesheet.CellFormats;
                    //Fonts fonts = stylesheet.Fonts;



                    string relationshipId2 = workbook.WorkbookPart.GetIdOfPart(sheetPart2);

                    sheetId = 2;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet2 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId2, SheetId = sheetId, Name = "EPISODE COST" };
                    sheets.Append(sheet2);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow2 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                    //ADD COLORS
                    //WorkbookStylesPart styles3 = workbookPart.AddNewPart<WorkbookStylesPart>();
                    //styles3.Stylesheet = CreateStylesheet();
                    //styles3.Stylesheet.Save();




                    //GENERIC TO STRONG
                    elementType = lst2.GetType().GetGenericArguments()[0];
                    props = new List<PropertyInfo>(elementType.GetProperties());

                    col = new List<string>();
                    intColCnt = 1;
                    foreach (var prop in props)
                    {
                        col.Add(prop.Name);

                        Cell cell = new Cell() { CellReference = GetExcelColumnName(intColCnt) + "1", StyleIndex = (UInt32Value)3U };
                        intColCnt++;

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop.Name);
                        headerRow2.AppendChild(cell);
                    }


                    sheetData2.AppendChild(headerRow2);

                    foreach (var row in lst2)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String c in col)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            var value = row.GetType().GetProperty(c).GetValue(row, null) + "";
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(value); //
                            newRow.AppendChild(cell);
                        }

                        sheetData2.AppendChild(newRow);
                    }


                    //ADDED TO AUTOSIZE
                    columns = AutoSize(sheetData2);
                    sheetPart2.Worksheet = new Worksheet();
                    sheetPart2.Worksheet.Append(columns);
                    sheetPart2.Worksheet.Append(sheetData2);
                }


            }
        }


        private static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }


        private static Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            fonts1.Append(font1);

            Font font2 = new Font();
            font2.Append(new Bold());

            fonts1.Append(font2);

            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);

            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };
            fill2.Append(patternFill2);

            // FillId = 2,RED
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFF0000" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3,BLUE
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "FF0070C0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4,YELLO
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);

            fill5.Append(patternFill5);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);

            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            Border border0 = new Border();
            borders1.Append(border0);

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color colorb1 = new Color() { Indexed = (UInt32Value)64U, Auto = true, Rgb = new HexBinaryValue() { Value = "000000" } };
            leftBorder1.Append(colorb1);
            RightBorder rightBorder1 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color colorb2 = new Color() { Indexed = (UInt32Value)64U, Auto = true, Rgb = new HexBinaryValue() { Value = "000000" } };
            rightBorder1.Append(colorb2);
            TopBorder topBorder1 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color colorb3 = new Color() { Indexed = (UInt32Value)64U, Auto = true, Rgb = new HexBinaryValue() { Value = "000000" } };
            topBorder1.Append(colorb3);
            BottomBorder bottomBorder1 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color colorb4 = new Color() { Indexed = (UInt32Value)64U, Auto = true, Rgb = new HexBinaryValue() { Value = "000000" } };
            bottomBorder1.Append(colorb4);
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();


            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFill = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);
            return stylesheet1;
        }


        private WorkbookStylesPart AddStyleSheet(SpreadsheetDocument spreadsheet)
        {
            WorkbookStylesPart stylesheet = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();

            Stylesheet workbookstylesheet = new Stylesheet();

            Font font0 = new Font();         // Default font

            Font font1 = new Font();         // Bold font
            Bold bold = new Bold();
            font1.Append(bold);

            Fonts fonts = new Fonts();      // <APENDING Fonts>
            fonts.Append(font0);
            fonts.Append(font1);

            // <Fills>
            Fill fill0 = new Fill();        // Default fill

            Fills fills = new Fills();      // <APENDING Fills>
            fills.Append(fill0);

            // <Borders>
            Border border0 = new Border();     // Defualt border

            Borders borders = new Borders();    // <APENDING Borders>
            borders.Append(border0);

            // <CellFormats>
            CellFormat cellformat0 = new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 }; // Default style : Mandatory | Style ID =0

            CellFormat cellformat1 = new CellFormat() { FontId = 1 };  // Style with Bold text ; Style ID = 1


            // <APENDING CellFormats>
            CellFormats cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);


            // Append FONTS, FILLS , BORDERS & CellFormats to stylesheet <Preserve the ORDER>
            workbookstylesheet.Append(fonts);
            workbookstylesheet.Append(fills);
            workbookstylesheet.Append(borders);
            workbookstylesheet.Append(cellformats);

            // Finalize
            stylesheet.Stylesheet = workbookstylesheet;
            stylesheet.Stylesheet.Save();

            return stylesheet;
        }




        private static Columns AutoSize(SheetData sheetData)
        {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            Columns columns = new Columns();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                columns.Append(col);
            }

            return columns;
        }

        private static Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
        {
            //iterate over all cells getting a max char value for each column
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            UInt32[] numberStyles = new UInt32[] { 5, 6, 7, 8 }; //styles that will add extra chars
            UInt32[] boldStyles = new UInt32[] { 1, 2, 3, 4, 6, 7, 8 }; //styles that will bold
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();

                //using cell index as my column
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    var cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    var cellTextLength = cellValue.Length;

                    if (cell.StyleIndex != null && numberStyles.Contains(cell.StyleIndex))
                    {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);

                        //add 3 for '.00' 
                        cellTextLength += (3 + thousandCount);
                    }

                    if (cell.StyleIndex != null && boldStyles.Contains(cell.StyleIndex))
                    {
                        //add an extra char for bold - not 100% acurate but good enough for what i need.
                        cellTextLength += 1;
                    }

                    if (maxColWidth.ContainsKey(i))
                    {
                        var current = maxColWidth[i];
                        if (cellTextLength > current)
                        {
                            maxColWidth[i] = cellTextLength;
                        }
                    }
                    else
                    {
                        maxColWidth.Add(i, cellTextLength);
                    }
                }
            }

            return maxColWidth;
        }

    }
}
