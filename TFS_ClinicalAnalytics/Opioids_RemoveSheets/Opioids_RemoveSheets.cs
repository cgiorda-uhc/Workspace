using System;
using System.Collections;
using System.Configuration;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Opioids_RemoveSheets
{
    class Opioids_RemoveSheets
    {
        static void Main(string[] args)
        {
            string strStartingFilePath = ConfigurationManager.AppSettings["StartingFilePath"];
            string strNewFilePath = ConfigurationManager.AppSettings["NewFilePath"];
            Excel.Application excelApp = new Excel.Application();
            //excelApp.Visible = true;
            Excel.Workbook workBook;
            Excel.Worksheet worksheet;

           DirectoryInfo fileDirectory = new DirectoryInfo(strStartingFilePath);
            FileInfo[] foundFiles = fileDirectory.GetFiles("*.xlsm", SearchOption.AllDirectories);

            string strValue = null;





            int iFileCnt = 1;
            foreach (FileInfo f in foundFiles)
            {

                workBook = excelApp.Workbooks.Open(f.FullName);
                worksheet = (Excel.Worksheet)workBook.Worksheets["Data_Source"];

                strValue = worksheet.Cells[2, 1].Value;



                excelApp.ScreenUpdating = false;
                excelApp.DisplayAlerts = false;



                //A - Commerical Only
                //B - Medicare Only
                //C - Medicaid Only
                //   AB - Commerical & Medicare
                //   AC - Commerical & Medicaid
                //   BC - Medicaid & Medicare
                //  ABC - Commerical, Medicare, &Medicaid 
                Console.WriteLine(iFileCnt + " Started file " + f.Name);

                if (!strValue.Contains("A"))
                {
                    worksheet = (Excel.Worksheet)workBook.Worksheets["Commercial_Summary"];
                    worksheet.Delete();
                }


                if (!strValue.Contains("B"))
                {
                    worksheet = (Excel.Worksheet)workBook.Worksheets["Medicare_Summary"];
                    worksheet.Delete();
                }

                if (!strValue.Contains("C"))
                {
                    worksheet = (Excel.Worksheet)workBook.Worksheets["Medicaid_Summary"];
                    worksheet.Delete();
                }

                //workBook.Worksheets["Commercial_Summary"].Delete();
                //if (!strValue.Contains("B"))
                //    workBook.Worksheets["Medicare_Summary"].Delete();
                //if (!strValue.Contains("C"))
                //    workBook.Worksheets["Medicaid_Summary"].Delete();

               // excelApp.DisplayAlerts = false;
                // workBook.SaveAs(strNewFilePath + f.Name, FileFormat:Excel.XlFileFormat.xlOpenXMLTemplate);
                workBook.SaveAs(strNewFilePath + f.Name, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled);
                //excelApp.DisplayAlerts = true;

                Console.WriteLine(iFileCnt + " Finished file " + f.Name);
                iFileCnt++;

                workBook.Close(false);
                workBook = null;


                excelApp.ScreenUpdating = true;
                excelApp.DisplayAlerts = true;



            }

            excelApp.Quit();
        }
    }
}
