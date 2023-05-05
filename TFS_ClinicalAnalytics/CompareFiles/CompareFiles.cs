using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareFiles
{
    class CompareFiles
    {
        static void Main(string[] args)
        {


            string stFolder1 = ConfigurationManager.AppSettings["Folder1"];
            string stFolder2 = ConfigurationManager.AppSettings["Folder2"];
            string strCurrentFileName = null;

            foreach(string strFileName in Directory.GetFiles(stFolder1, "*.*", SearchOption.AllDirectories))
            {
                strCurrentFileName = Path.GetFileName(strFileName).Replace(".pdf", ".doc");

                if (!File.Exists(stFolder2  + strCurrentFileName))
                {
                    Console.WriteLine(strCurrentFileName + " is missing");
                }
            }


        }
    }
}
