using ActiveDirectoryLibrary;
using HelperFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            ActiveDirectory ad = new ActiveDirectory();

            ad.LDAPPath =  ConfigurationManager.AppSettings["LDAPPath"];
            ad.LDAPDomain =  ConfigurationManager.AppSettings["LDAPDomain"];



            List<string> main = new List<string>();
            List<ADUserModel> _users;
            List<ADUserModel> _usersFinal = new List<ADUserModel>();
            main.Add("jturne63");
            main.Add("llanger2");
            main.Add("lgiantur");

            foreach(string s in main)
            {

                ad.UsersByManagerList = new List<ADUserModel>();
                ad.GetUsersByManager(s);
                _users = ad.UsersByManagerList;
                foreach(var u in _users)
                {
                    if (string.IsNullOrEmpty(u.EmailAddress))//NOT THE MAIN ACCOUNT
                        continue;
                    _usersFinal.Add(u);

                    //var matches = _usersFinal.Where(p => p.EmailAddress == u.EmailAddress);
                    //if(matches.Count() == 0)
                    //{
                    //    _usersFinal.Add(u);
                    //}
                }
            }







            StringBuilder sb = new StringBuilder();

            sb.Append("<h3>Looks Okay?</h3>");
            sb.Append("<h4>Just added each user's manager to the output</h4>");
            sb.Append("<h5>I will add this list to the DB so I can compare ONLY users added or removed for your future emails?</h5>");
            foreach (var u in _usersFinal)
            {
                ADUserModel m = u.Manager;
                sb.Append("<p><b>" + m.FirstName + " " + m.LastName +"</b>, " + u.LoginName + ", " +  u.FirstName + " " + u.LastName + ", " + u.EmailAddress + "</p>");
            }
            

            // ad.GetAllUsersManger("jturne63");


            //HelperFunctions.HelperFunctions.Email("chris_giordano@uhc.com", "chris_giordano@uhc.com", "AD Direct Reports Prototype", sb.ToString(), "chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.Normal);


            //HelperFunctions.HelperFunctions.Email("lindsey_ross@uhc.com", "chris_giordano@uhc.com", "AD Direct Reports Prototype", sb.ToString(), "chris_giordano@uhc.com;jon_maguire@uhc.com", null, System.Net.Mail.MailPriority.Normal);

            return;











            DataRow currentRow;

            DataTable dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("userid", typeof(String));
            dtFinalDataTable.Columns.Add("email", typeof(String));
            dtFinalDataTable.Columns.Add("global_group", typeof(String));
            dtFinalDataTable.Columns.Add("department", typeof(String));
            dtFinalDataTable.TableName = "stg.pbi_membership";


            var groups = ad.GetGroupByName("AZU_ORBIT_POWERBI_UHC_VC_CLIN_PROG_PERF_*");

            foreach(var g in groups)
            {
                var grp = g.Replace("CN=", "");
                var users = ad.GetUserFromGroup(grp);
                foreach(var u in users)
                {
                    currentRow = dtFinalDataTable.NewRow();
                    currentRow["userid"] = u.LoginName;
                    currentRow["email"] = u.EmailAddress;
                    currentRow["global_group"] = grp;
                    currentRow["department"] = u.Department ;
                    dtFinalDataTable.Rows.Add(currentRow);
                }
            }



            if (dtFinalDataTable.Rows.Count > 0)
            {
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

            }

            ////var results = ad.GetUsersByName(name);

            //// Print the value of the variable (userName), which will display the input value
            //StringBuilder sb = new StringBuilder();
            //foreach(var r in results)
            //{
            //    sb.Append(r.FirstName + " " + r.LastName + ", " + r.EmailAddress + ", " + r.State + ", " + r.ManagerName + Environment.NewLine);

            //}
            //Console.WriteLine("Users found: " + Environment.NewLine);
            //Console.WriteLine(sb.ToString());


            //Console.ReadLine();


        }

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }
    }
}
