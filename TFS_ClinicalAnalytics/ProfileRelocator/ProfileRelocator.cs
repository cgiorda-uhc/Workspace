using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileRelocator
{
    class ProfileRelocator
    {

       
        static void Main(string[] args)
        {
            DataTable dt = null;

            

            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strFilePath = ConfigurationManager.AppSettings["FilePath"];
            string strExclusionPath = ConfigurationManager.AppSettings["ExclusionPath"];
            string strIdentifier = null;

            string strNewPath = null;
            DirectoryInfo fileDirectory = new DirectoryInfo(strFilePath);
            FileInfo[] foundFiles = null;

            string strSQL = null;

            strSQL = "SELECT 402 as MPIN UNION SELECT 1098 as MPIN UNION SELECT 2574 as MPIN UNION SELECT 5172486 as MPIN UNION SELECT 3044692 as MPIN UNION SELECT 374572 as MPIN";

            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

            foreach (DataRow dr in dt.Rows)//MAIN LOOP START
            {
                strIdentifier = (dr["MPIN"] != DBNull.Value ? dr["MPIN"].ToString().Trim() : "MPIN MISSING");

                foundFiles = fileDirectory.GetFiles(strIdentifier + "_*.pdf", SearchOption.AllDirectories);

                foreach(FileInfo f in foundFiles)
                {

                    strNewPath = strExclusionPath + f.FullName.ToString().Replace(strFilePath, "").Replace(f.Name, "");

                    if (!Directory.Exists(strNewPath))
                    {
                        Directory.CreateDirectory(strNewPath);
                    }


                    File.Move(f.FullName, strNewPath + f.Name); // Try to move

                  
                    DeleteFolderIfEmpty(f.DirectoryName);



                }


            }

          


        }


        public static void DeleteFolderIfEmpty(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            if (dir.EnumerateFiles().Any() || dir.EnumerateDirectories().Any())
                return;
            DirectoryInfo parent = dir.Parent;
            dir.Delete();

            // Climb up to the parent
            DeleteFolderIfEmpty(parent.FullName);
        }

    }
}
