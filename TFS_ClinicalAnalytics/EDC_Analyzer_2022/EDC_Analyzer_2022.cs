using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDC_Analyzer_2022
{
    class EDC_Analyzer_2022
    {

        //\\msp09fil02\D904\TAM_PMO\AffordabilityOptimization\EDC\PI_Savings
        static void Main(string[] args)
        {

            Console.WriteLine("EDC PI Savings Parser");
            string strDataFolderPath = ConfigurationManager.AppSettings["DataFolderPath"];
            string strArchiveFolderPath = ConfigurationManager.AppSettings["ArchiveFolderPath"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            string[] files;
            int intFileCnt = 1;
            int intRowCnt = 1;



            files = Directory.GetFiles(strDataFolderPath, "*.xlsx", SearchOption.AllDirectories);
            intFileCnt = 1;
            intRowCnt = 1;


            foreach (string strFile in files)
            {
                Console.Write("\rVerifying " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " compressed files");
                intFileCnt++;
                //string strExtension = Path.GetExtension(strFile);
                string strFileName = Path.GetFileName(strFile);
                string strLOBFolder = new DirectoryInfo(strFile.Replace(strFileName, "")).Name;

                if (File.Exists(strArchiveFolderPath + strLOBFolder + "\\" + strFileName))
                {
                    continue;
                }
                else
                {
                    //PROCESS!!!!!
                }
     

            }

        }
    }
}
