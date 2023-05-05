using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelFile_Cleanup_NJ
{
    class ExcelFile_Cleanup_NJ
    {

        //static string strFilePath = @"\\nasgw056pn\bi_out\PCR\OCP\ACO\202007";
        static string strFilePath = @"C:\Users\cgiorda\Desktop\EXCELFILEASIZE";

        static void Main(string[] args)
        {
            //string strNewFolder = "CSG CLEANUP";
            string strNewFolder = "CSG_FILESIZE_TEST";
            string strNewFile = null;
            //string[] strArr = new string[] {"COMMERCIAL", "MEDICAID", "MEDICARE" };
            string[] strArr = new string[] { "COMMERCIAL" };

            Application excel = new Application();
            Workbook wb = null;
            Worksheet xlWorkSheet = null;

            int i = 1;

            foreach (string s in strArr)
            {
                foreach(string file in Directory.GetFiles(strFilePath + "\\" + s, "*.xlsx", SearchOption.TopDirectoryOnly))
                {
                    Console.WriteLine("Processing file " + i + ":" + file);


                    strNewFile = strFilePath + "\\" + strNewFolder + "\\" + s;
                    if (!Directory.Exists(strNewFile))
                        Directory.CreateDirectory(strNewFile);

                    if (File.Exists(strNewFile + "\\" + Path.GetFileName(file)))
                        continue;
                    //File.Delete(strNewFile + "\\" + Path.GetFileName(file));


                    wb = excel.Workbooks.Open(file);
                    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.Item["ACO High Level Summary"];
                    xlWorkSheet.Columns["A:A"].ColumnWidth = 73;
                    xlWorkSheet.Activate();
                    xlWorkSheet.Select();
                    excel.ActiveWindow.Zoom = 100;


                    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.Item["ACO Member Summary"];
                    xlWorkSheet.Columns["R:R"].ColumnWidth = 9;



                    //xlWorkSheet.Activate.Select
                    //ActiveWindow.Zoom = 85;


                  

                    wb.SaveAs(strNewFile + "\\" + Path.GetFileName(file), XlFileFormat.xlOpenXMLWorkbook);


                    if (xlWorkSheet != null)
                    {
                        Marshal.ReleaseComObject(xlWorkSheet);
                        xlWorkSheet = null;
                    }


                    if (wb != null)
                    {
                        wb.Close(false); ;
                        Marshal.ReleaseComObject(wb);
                    }
                    wb = null;
                    i++;
                }

            }

            if (excel != null)
            {
                excel.Quit();
                //Marshal.FinalReleaseComObject(xlsApp);
                Marshal.ReleaseComObject(excel);
                excel = null;
            }

        }
    }
}
