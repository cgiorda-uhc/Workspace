using HelperFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileManager
{
    class ProfileManager
    {




        static void Main(string[] args)
        {

            string strILUCA_Database = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strSourcePath = ConfigurationManager.AppSettings["SourcePathRoot"];
            string strDestinationPath = ConfigurationManager.AppSettings["DestinationPathRoot"];
            string strProfileType = ConfigurationManager.AppSettings["ProfileType"];
            string strPhaseID = ConfigurationManager.AppSettings["PhaseID"];
            string strMailingID = ConfigurationManager.AppSettings["MailingID"];
            string strExtension = ConfigurationManager.AppSettings["Extension"];

            bool blRecursiveSearch = Boolean.Parse(ConfigurationManager.AppSettings["RecursiveSearch"]);
            bool blGroupResultsToTIN = Boolean.Parse(ConfigurationManager.AppSettings["GroupResultsToTIN"]);


            string strTIN = null;
            string strMPIN = null;
            string strSpecialHandling = null;

            string strTmpCurrentDestinationPath = null;

            int intCnt = 0;
            int intTotal = 0;

            SearchOption soOption = SearchOption.TopDirectoryOnly;
            if(blRecursiveSearch == true)
                soOption = SearchOption.AllDirectories;


            if (!Directory.Exists(strSourcePath))
            {
                Console.WriteLine("Invalid Source Directory");
                Console.Beep();
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists(strDestinationPath))
            {
                Directory.CreateDirectory(strDestinationPath);
            }

            Console.WriteLine("Getting data from IL_UCA...");
            DataTable dtMain = getDataSet(strILUCA_Database, strProfileType, strPhaseID, strMailingID);
            intTotal = dtMain.Rows.Count;
            foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
            {
                Console.WriteLine("Processing row " + intCnt + " out of " + intTotal);


                strMPIN = (string.IsNullOrEmpty(dr["MPIN"].ToString().Trim()) ? null : (!dr["MPIN"].ToString().IsNumeric() ? null : dr["MPIN"].ToString().Trim()));
                strTIN = (string.IsNullOrEmpty(dr["TIN"].ToString().Trim()) ? null : (!dr["TIN"].ToString().IsNumeric() ? null : dr["TIN"].ToString().Trim()));
                strSpecialHandling = (string.IsNullOrEmpty(dr["SpecialHandling"].ToString().Trim()) ? null : dr["SpecialHandling"].ToString());

                strTmpCurrentDestinationPath = strDestinationPath + "\\" + strSpecialHandling + "\\";

                if (strProfileType == "TIN" && strTIN == null)
                {
                    Console.WriteLine("No TIN supplied for a TIN run!!!");
                    Console.Beep();
                    Console.ReadLine();
                }

                if (strProfileType == "MPIN" && strMPIN == null)
                {
                    Console.WriteLine("No MPIN supplied for an MPIN run!!!");
                    Console.Beep();
                    Console.ReadLine();
                }

                if (strProfileType == "MPIN")//MIGHT NOT NEED PROFILETYPES BUT FOR NOW WELL KEEP IT FLEXIBLE
                {

                    if (strTIN != null)
                    {
                        if (Directory.Exists(strSourcePath + "\\" + strTIN))
                        {
                            foreach (string f in Directory.GetFiles(strSourcePath + "\\" + strTIN, strMPIN + "_*", soOption).Where(s => s.EndsWith(strExtension)))
                            {
                                //moving and shaking and deleting folders???!!!
                                Console.WriteLine(Path.GetFileName(f));
                            }
                        }

                        if (blGroupResultsToTIN == true)//TIN HAS A VALUE AND CONFIG SPECIFIED TIN FOLDERS
                            strTmpCurrentDestinationPath = strTmpCurrentDestinationPath + "\\" + strTIN;


                    }


                    foreach (string f in Directory.GetFiles(strSourcePath, strMPIN + "_*", soOption).Where(s => s.EndsWith(strExtension)))
                    {
                        //moving and shaking and deleting folders???!!!
                        Console.WriteLine("Moving MPIN file :" + f + " ...");

                        if (!Directory.Exists(strTmpCurrentDestinationPath))
                        {
                            Directory.CreateDirectory(strTmpCurrentDestinationPath);
                        }


                        File.Move(f, strTmpCurrentDestinationPath + "\\" + Path.GetFileName(f));

                    }


                }
                else if (strProfileType == "TIN")
                {
                    foreach (string f in Directory.GetFiles(strSourcePath, strTIN + "_*", soOption).Where(s => s.EndsWith(strExtension)))
                    {
                        //moving and shaking and deleting folders???!!!
                        Console.WriteLine("Moving TIN file :" + f + " ...");

                        if (!Directory.Exists(strTmpCurrentDestinationPath))
                        {
                            Directory.CreateDirectory(strTmpCurrentDestinationPath);
                        }


                        File.Move(f, strTmpCurrentDestinationPath + "\\" + Path.GetFileName(f));

                    }
                }


               intCnt++;

            }

            //NOW THAT WERE DONE DELETE ALL EMPTY DIRECTORIES
            deleteEmptyDirectories(strSourcePath);




        }




        private static void deleteEmptyDirectories(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                deleteEmptyDirectories(directory);
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }




        private static DataTable getDataSet(string strConnectionString, string strProfileType, string strPhaseID, string strMailingID)
        {
            DataTable dt = null;

            dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("MPIN");
            dt.Columns.Add("TIN");
            dt.Columns.Add("SpecialHandling");

            DataRow dr = dt.NewRow();
            dr["MPIN"] = "12345";
            dr["TIN"] = "999999";
            dr["SpecialHandling"] = "folder 1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["MPIN"] = "23456";
            dr["TIN"] = "999999";
            dr["SpecialHandling"] = "folder 1";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["MPIN"] = "34567";
            dr["TIN"] = "999999";
            dr["SpecialHandling"] = "folder 2";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["MPIN"] = "45678";
            dr["TIN"] = "888888";
            dr["SpecialHandling"] = "folder 3";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["MPIN"] = "56789";
            dr["TIN"] = "888888";
            dr["SpecialHandling"] = "folder 3";
            dt.Rows.Add(dr);


            return dt;

        }


    }
}
