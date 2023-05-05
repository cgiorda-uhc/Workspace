using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEI2BulkLoadCache
{
    class PEI2BulkLoadCache
    {

        static StreamWriter swBulkCacheScript = null;
        static void Main(string[] args)
        {

            string strScriptPath = ConfigurationManager.AppSettings["ScriptPath"];
            string strFilePath = ConfigurationManager.AppSettings["FileShare"];

            swBulkCacheScript = new StreamWriter(strScriptPath +  "1.0.1_bulk_cache_script.sql", false);
            DirSearch(strFilePath);



            swBulkCacheScript.Flush();
            swBulkCacheScript.Close();


        }


        static void DirSearch(string sDir)
        {
            try
            {
                //foreach (string d in Directory.GetDirectories(sDir))
                //{
                   // if (d.Contains("\\~"))
                       // continue;

                    foreach (string f in Directory.GetFiles(sDir))
                    {
                        Console.WriteLine(f);


                        string[] strFileArr = f.Split('\\');
                        string strFileName = "", strKeyTopic = "", strFolder = "";

                        if(strFileArr.Length == 6)
                        {
                            strKeyTopic = strFileArr[4];
                            strFileName = strFileArr[5];
                            strFolder = "";
                        }
                        else if (strFileArr.Length == 7)
                        {
                            strKeyTopic = strFileArr[4];
                            strFolder = strFileArr[5];
                            strFileName = strFileArr[6];
                        }



                        if (strFolder != "" && strFolder != "TIN" && strFolder != "HICN")
                            continue;

                        swBulkCacheScript.WriteLine("INSERT INTO pei2_bulk_load_cache (folder_name,key_topic_description, file_name) VALUES ('"+ strFolder.Replace("'","''").Trim() + "', '"+ strKeyTopic.Replace("'", "''").Trim() + "', '" + strFileName.Replace("'", "''").Trim() + "');");


                    }
                    //DirSearch(d);
                    swBulkCacheScript.Flush();
                //}
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }



    }
}
