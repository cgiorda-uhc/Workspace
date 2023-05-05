using Microsoft.Office.Interop.Excel;
using System;
using System.Runtime.InteropServices;

namespace ASimpleExcelTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string strPath = AppDomain.CurrentDomain.BaseDirectory + "template\\CS_Reporting_Template.xlsx";

            Application xlApp = new Application();
            Workbook workbook = xlApp.Workbooks.Open(strPath);

            workbook.Close();
            xlApp.Quit();
            Marshal.FinalReleaseComObject(workbook);
            Marshal.FinalReleaseComObject(xlApp);


        }
    }
}
