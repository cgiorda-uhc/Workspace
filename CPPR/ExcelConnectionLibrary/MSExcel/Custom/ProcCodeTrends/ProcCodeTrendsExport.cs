using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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


            string strFilePath = @"\\WP000003507\csg_share\VCPortal\Files\Proc_Code_Trend_Template.xlsx";

            int intNameCntTmp = 0;
            XLWorkbook wb = new XLWorkbook(strFilePath);
            IXLWorksheet wsSource = null;
            IXLRange range;
            int rowCnt = 0;
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append(getterStatus());
            var sheet = "OP";

            sbStatus.Append("--Creating sheet for " + sheet + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            wsSource = wb.Worksheet(sheet);



            //CLM OP Unique Individual START
            //CLM OP Unique Individual START
            //CLM OP Unique Individual START
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
