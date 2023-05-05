using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicianFeedbackTracker
{
    static class SharedFunctions
    {


        public static string getPEIEngagementLink(string strMPIN, string strKeyTopic)
        {
            string strLink = null;
            DataTable dt = DBConnection.getMSSQLDataTable(GlobalObjects.strPEIConnectionString, GlobalObjects.getPEIEngagementSQL(strKeyTopic, strMPIN));
            if (dt.Rows.Count <= 0)
                return null;

            if (dt.Rows[0]["is_open"].ToString().Equals("Closed"))
                strLink = GlobalObjects.strPEIClosedEngagementURL.Replace("{$eid}", dt.Rows[0]["engagement_id"].ToString());
            else
                strLink = GlobalObjects.strPEIOpenEngagementURL.Replace("{$eid}", dt.Rows[0]["engagement_id"].ToString());

            return strLink;
        }


        public static List<string> getPEIFileLinks(string strMPIN, string strTin, string strKeyTopic, string strParentMPIN)
        {
            List<string> strLinks = new List<string>();

            string strFilePath = GlobalObjects.strPEIDocumentsPath + "\\" + strKeyTopic + "\\TIN\\";
            if (Directory.Exists(strFilePath))
                strLinks.AddRange(Directory.GetFiles(strFilePath, strTin + "_*").ToList());


            strFilePath = GlobalObjects.strPEIDocumentsPath + "\\" + strKeyTopic + "\\";
            if (Directory.Exists(strFilePath))
            {
                if(strMPIN != strParentMPIN)
                    strLinks.AddRange(Directory.GetFiles(strFilePath, strParentMPIN + "_*").ToList());

                strLinks.AddRange(Directory.GetFiles(strFilePath, strMPIN + "_*").ToList());
            }
                

            return strLinks;
        }



        public static string getEmailAddress(string strUserId)
        {
            DataTable dtTmp = null;
            List<string> lstADInfo = null;
            Hashtable htTmp = GlobalObjects.htGetUserEmailByUserIdSQL(strUserId);
            object objPEIReturn = DBConnection.getMSSQLExecuteScalarSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strGetUserEmailByUserIdSQL, htTmp);



            if (objPEIReturn + "" == "")
            {

                if (ActiveDirectoryFunctions.strADUserName == null || ActiveDirectoryFunctions.strADPassword == null)
                {
                    dtTmp = DBConnection.getMSSQLDataTable(GlobalObjects.strPEIConnectionString, GlobalObjects.strGetAdUserNamePassword);
                    if (dtTmp != null)
                    {
                        ActiveDirectoryFunctions.strADUserName = dtTmp.Rows[0][0] + "";
                        ActiveDirectoryFunctions.strADPassword = dtTmp.Rows[0][1] + "";
                    }
                }


                try
                {
                    lstADInfo = ActiveDirectoryFunctions.GetADInfo(strUserId);
                    htTmp = GlobalObjects.htInsertUpdateUserSQL(strUserId, lstADInfo[0], lstADInfo[1], lstADInfo[2]);
                    DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strInsertUpdateUserSQL, htTmp);
                }
                catch(Exception ex)
                {
                    return null;
                }



                return lstADInfo[2];
            }
            else
            {
                return objPEIReturn.ToString();
            }
        }

        public static bool hasAccess(string strUserId, string strGroupCSV)
        {
            //QA TOOLS SECTION
            if (GlobalObjects.getNameValueDataTable("users").Select("filter in ('"+ strGroupCSV.Replace(" ","").Replace(",","','") + "') AND  value ='" + strUserId + "'").Count() > 0)
                return true;
            else
                return false;
        }


        public static void killProcess(string strProcessName)
        {
            foreach (Process Proc in Process.GetProcesses())
                if (Proc.ProcessName.Equals(strProcessName))  //Process Excel?
                    Proc.Kill();
        }



        public static DataTable prepDataTableForEditing(string strConnectionString, string strSQL)
        {
            DataTable dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
            var column = new DataColumn("IsUpdated", typeof(bool));
            column.DefaultValue = false;
            dt.Columns.Add(column);
            return dt;
        }


        public static DataTable prepDataTableForEditing(DataTable dt)
        {
            if(!dt.Columns.Contains("IsUpdated"))
            {
                var column = new DataColumn("IsUpdated", typeof(bool));
                column.DefaultValue = false;
                dt.Columns.Add(column);
            }

            return dt;
        }




    }

}
