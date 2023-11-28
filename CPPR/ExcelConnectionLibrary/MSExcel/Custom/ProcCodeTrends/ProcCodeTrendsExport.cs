using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Models.ProcCodeTrends;

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

            //MAIN HEADER ROW
            rowCnt = 0;

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
            rowCnt = 1;

            colCnt = 1;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = "Proc Code";
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
            wsSource.Range("D1:"+ columnLetter + "1").Merge();

            //LOOP TREND COLUMN HEADERS
            foreach (var yq in clm_op_results.year_quarter)
            {
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);

                cell.Value = yq.year + "Q" + yq.quarter;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                colCnt++;
            }

            //MERGE YQ "Trend"
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt - 1);
            wsSource.Range("D1:" + columnLetter + "1").Merge();


            //POPULATE DATA
            Int16 cnt = 1;
            foreach (var c in clm_op_results.unique_individual)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                sbStatus.Append("--Populating data for CLM OP Unique Individual" + Environment.NewLine);
                setterStatus(sbStatus.ToString());


                wsSource.Cell("B" + cnt).Value = c.px;
                wsSource.Cell("D" + cnt).Value = c.px_desc;
                wsSource.Cell("F" + cnt).Value = c.Y1Q1_indv;
                cnt++;
            }
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
