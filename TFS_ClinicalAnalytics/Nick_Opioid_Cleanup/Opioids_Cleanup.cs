using System;
using System.Collections;
using System.Configuration;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Opioids_Cleanup
{
    class Opioids_Cleanup
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
            FileInfo[] foundFiles = fileDirectory.GetFiles("*.xlsx", SearchOption.AllDirectories);

    

            int iFileCnt = 1;
            foreach (FileInfo f in foundFiles)
            {

                workBook = excelApp.Workbooks.Open(f.FullName);
                worksheet = (Excel.Worksheet)workBook.Worksheets["Dates"];
                worksheet.Cells[3, 1].Value = "01/01/2019 - 12/31/2019";

                worksheet = (Excel.Worksheet)workBook.Worksheets["Specialty Referral Designation"];
                //worksheet.Cells[7, 7].Formula = "=\"Premium Designation **                                                        Reporting Period: \"&Dates!A3  &                       Reflects physician's entire population\"";
                worksheet.Cells[7, 7].Formula = worksheet.Cells[7, 7].Formula.ToString().Replace("A2", "A3");


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

                //workBook.Worksheets["Commercial_Summary"].Delete();
                //if (!strValue.Contains("B"))
                //    workBook.Worksheets["Medicare_Summary"].Delete();
                //if (!strValue.Contains("C"))
                //    workBook.Worksheets["Medicaid_Summary"].Delete();

                // excelApp.DisplayAlerts = false;
                // workBook.SaveAs(strNewFilePath + f.Name, FileFormat:Excel.XlFileFormat.xlOpenXMLTemplate);
                workBook.SaveAs(strNewFilePath + f.Name);
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
