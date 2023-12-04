using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Models.ProcCodeTrends;
using Task = System.Threading.Tasks.Task;

namespace FileParsingLibrary.MSExcel.Custom.ProcCodeTrends
{
    public class ProcCodeTrendsExport
    {

        public static async Task<byte[]> ExportProcDataToExcel(CLM_OP_Report_Model clm_op_results, Func<string> getterStatus, Action<string> setterStatus, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");
            byte[] final = new byte[0];



            XLWorkbook wb = new XLWorkbook();
            IXLWorksheet wsSource = null;
            IXLRange range;
            IXLCell cell;
    
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append(getterStatus());

            string header;
            string columnLetter;
            string columnLetterLast;
            string bgcolor = "#D9D9D9";
            string sheet = "OP";

            sbStatus.Append("--Creating sheet for " + sheet + Environment.NewLine);
            setterStatus(sbStatus.ToString());


            wsSource = wb.Worksheets.Add(sheet);


            Int16 colCnt = 1;
            Int16 rowCnt = 1;


            //CLM OP Unique Individual START
            //CLM OP Unique Individual START
            //CLM OP Unique Individual START
            header = "Unique Individual";

            //MAIN HEADER 'Unique Individual' ROW
            rowCnt = 1;

            colCnt = 1;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            wsSource.Cell(columnLetter + rowCnt).Value = header;

            colCnt = 3;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = header;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);

            //COLUMN HEADER ROW
            rowCnt = 2;

            colCnt = 1;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = "Proc" + Environment.NewLine + "Code";
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);

  
            colCnt = 2;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = "Proc Desc";
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);

            colCnt = 3;
            //LOOP YQ COLUMN HEADERS
            foreach (var yq in  clm_op_results.year_quarter)
            {
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);

                cell.Value = yq.year + "Q" + yq.quarter;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                colCnt++;
            }

            //MERGE YQ "Unique Individual"
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt - 1);
            columnLetterLast = SharedExcelFunctions.GetColumnName(colCnt);
            range = wsSource.Range("D1:" + columnLetter + "1");
            range.Merge();

            //ADD MAIN HEADER 'Trend' ROW
            rowCnt = 1;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = "Trend";
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217


            //COLUMN HEADER ROW
            rowCnt = 2;
            //LOOP TREND COLUMN HEADERS

            var t = clm_op_results.year_quarter[0].year.ToString().Substring(2,2) + "Q" + clm_op_results.year_quarter[0].quarter +"/" + clm_op_results.year_quarter[4].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[4].quarter;
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);


            t = clm_op_results.year_quarter[1].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[1].quarter + "/" + clm_op_results.year_quarter[5].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[5].quarter;
            colCnt++;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);



            t = clm_op_results.year_quarter[2].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[2].quarter + "/" + clm_op_results.year_quarter[6].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[6].quarter;
            colCnt++;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);



            t = clm_op_results.year_quarter[3].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[3].quarter + "/" + clm_op_results.year_quarter[7].year.ToString().Substring(2, 2) + "Q" + clm_op_results.year_quarter[7].quarter;
            colCnt++;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);


            //MERGE YQ "Trend"
            range = wsSource.Range(columnLetterLast + "1:" + columnLetter + "1");
            range.Merge();
            range.Style.Border.RightBorderColor = XLColor.Black;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //DATA ROW
            rowCnt = 3;

            //START FROM FIRST COLUMN
            colCnt = 1;
            //POPULATE DATA
            foreach (var c in clm_op_results.unique_individual)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                sbStatus.Append("--Populating data for CLM OP Unique Individual" + Environment.NewLine);
                setterStatus(sbStatus.ToString());




                foreach (PropertyInfo propertyInfo in c.GetType().GetProperties())
                {

                    object val = propertyInfo.GetValue(c, null);
                    columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                    cell = wsSource.Cell(columnLetter + rowCnt);
  

                    //decimal test = 0;
                    //if (decimal.TryParse(val + "", out test))
                    //{
                    //    cell.Value = decimal.Parse(val.ToString());
                    //    cell.Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Integer);
                    //}
                    //else
                    //{
                    //    cell.Value = val.ToString();
                    //}

                    if(propertyInfo.GetType() == typeof(int) || propertyInfo.GetType() == typeof(int?))   
                     {
                        if(val != null)
                        {
                            cell.Value = int.Parse(val.ToString()); 
                        }
                    }
                    else
                    {
                        cell.Value = val + "";
                    }


                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                    colCnt++;
                }


                rowCnt++;
                colCnt = 1;
            }
            wsSource.Columns().AdjustToContents();
            //CLM OP Unique Individual END
            //CLM OP Unique Individual END
            //CLM OP Unique Individual END







            if (token.IsCancellationRequested)
            {
                setterStatus("~~~Report Generation Cancelled~~~");
                token.ThrowIfCancellationRequested();
            }


            sbStatus.Append("--Preparing Excel file for saving" + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                final = ms.ToArray();
            }

            await Task.CompletedTask;
            return final;
        }

    }
}
