using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetailReportGenerator
{
    class DetailReportGenerator
    {





        static void Main(string[] args)
        {

            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strReportsPath = ConfigurationManager.AppSettings["ReportsMainPath"];

            DataTable dt = DBConnection64.getMSSQLDataTableSP(strConnectionString, "sp_cg_QATrackerTool_select_member_detail_requests", null);

            string strMPIN = null;
            string strEmail = null;
            string strNTID = null;
            string strFullName = null;
            string strProject = null;
            string strParentId = null;
            string strChildId = null;

            StringBuilder sbFinalFolder = new StringBuilder();

            foreach (DataRow dr in dt.Rows)
            {
                sbFinalFolder.Append(strReportsPath);


                if (!dr["user_nt_id"].ToString().Equals(strNTID))
                {
                    strNTID = dr["user_nt_id"].ToString();
                    strProject = null;
                    strMPIN = null;
                    //if(!Directory.Exists(strReportsPath + "\\" + strNTID))
                    // Directory.CreateDirectory(strReportsPath + "\\" + strNTID);
                }
                sbFinalFolder.Append("\\" + strNTID);


                if (!dr["phase_description"].ToString().Equals(strProject))
                {
                    strProject = dr["phase_description"].ToString();
                    strMPIN = null;
                    //if(!Directory.Exists(strReportsPath + "\\" + strNTID))
                    // Directory.CreateDirectory(strReportsPath + "\\" + strNTID);
                }
                sbFinalFolder.Append("\\" + strProject );


                if (dr["mpin"].ToString().Equals(strMPIN))
                    continue;



                //if (!Directory.Exists(sbFinalFolder.ToString()))
                //    Directory.CreateDirectory(sbFinalFolder.ToString());




                strMPIN = dr["mpin"].ToString();

                strEmail = dr["user_email"].ToString();

                strParentId = dr["qa_tracker_parent_id"].ToString();

                strChildId = dr["qa_tracker_child_id"].ToString();

                strFullName = dr["user_fullname"].ToString();


                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = ConfigurationManager.AppSettings[strProject];
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "\"" + strMPIN + "\"" +  " " + "\"" + sbFinalFolder.ToString() + "\"";

                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch
                {
                    // Log error.
                }







                //          < add key = "PCPCohort1_ExecutablePath" value = "" />

                //< add key = "PCPCohort2_ExecutablePath" value = "C:\Work\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\PCP_Phase_1.2_AdHoc_Details\bin\Debug\PCP_Phase_1.2_AdHoc_Details.exe" />

                //   < add key = "OBGYNCohort1_ExecutablePath" value = "C:\Work\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\PR_OB_AdHoc_Details\bin\Debug\PR_AdHoc_Details.exe" />

                //      < add key = "SpecialtiesCohort1_ExecutablePath" value = "C:\Work\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\PR_Spec_AdHoc_Details\bin\Debug\PR_AdHoc_Details.exe" />






                //strMPIN = dr["mpin"].ToString();

                //strEmail = dr["user_email"].ToString();

                //strProject = dr["phase_description"].ToString();

                //strParentId = dr["qa_tracker_parent_id"].ToString();

                //strChildId = dr["qa_tracker_child_id"].ToString();

                //strNTID = dr["user_nt_id"].ToString();

                //strFullName = dr["user_fullname"].ToString();


                sbFinalFolder.Remove(0, sbFinalFolder.Length);

            }

        }
    }
}
