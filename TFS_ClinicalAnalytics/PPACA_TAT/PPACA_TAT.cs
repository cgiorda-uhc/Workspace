using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPACA_TAT
{
    class PPACA_TAT
    {
        static void Main(string[] args)
        {
            string strSQL = "SELECT [file_month] ,[file_year] ,[num_tat] ,[den_tat] ,[tat_val] ,[rtype] FROM [IL_UCA].[dbo].[VW_PPACA_TAT]";
            string strConnectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
            StringBuilder sbEmail = new StringBuilder();

            string recipients = "LAlfonso@uhc.com;allyson_k_clark@uhc.com;sanford_p_cohen@uhc.com;laura_fischer@uhc.com;mayrene_hernandez@uhc.com;steve_lumpinski@optum.com;renee_l_struck@uhc.com;jessica_l_tarnowski@uhc.com;heather_vanis@uhc.com;mark_j_newman@uhc.com;Judy.Fujimoto@optum.com;carol_s_winter@uhc.com;inez.bulatao@uhc.com;nancy.morden@uhc.com;christopher_pauwels@uhc.com;roma_adipat@uhc.com;dana.savoie@optum.com;laurie.gianturco@uhc.com;rosamond_e_eschert@uhc.com;loaiello@uhc.com;candace_smith@uhc.com;stacy_v_washington@uhc.com; Carella-lisa.carellaashla@uhc.com;jon_maguire@uhc.com;inna_rudi@uhc.com";

            recipients = "mary_ann_dimartino@uhc.com;hong_gao@uhc.com;chris_giordano@uhc.com";
            recipients = "mary_ann_dimartino@uhc.com";
            string from = "chris_giordano@uhc.com";
            string cc = "chris_giordano@uhc.com;inna_rudi@uhc.com";
            //cc = "chris_giordano@uhc.com";
            //recipients = "chris_giordano@uhc.com";

            string emailFilePath = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Automation\EmailTemplates\PPACA_TAT.txt";

            
            DataRow dr;
            DataTable dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);


            strSQL = "select top 1 file_month, [file_year],file_date FROM [IL_UCA].[stg].[EviCore_TAT] where file_date = (select max(file_date) from[IL_UCA].[stg].[EviCore_TAT])";
            DataTable dtDate = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL);
            string fileSearch = "United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_"+ dtDate.Rows[0]["file_year"] + "_" + dtDate.Rows[0]["file_month"] + ".xlsx";
            DateTime fileDate = (DateTime)dtDate.Rows[0]["file_date"];
            string filePath = @"\\NASGWFTP03\Care_Core_FTP_Files\Radiology";
            FileInfo fi = new FileInfo(filePath + "\\" + fileSearch);
            DateTime dtCreateDate = fi.CreationTime;


            //string visibility = "hidden";
            //string creatdate = dtCreateDate.ToShortDateString();
            //string filedate = fileDate.AddMonths(1).ToShortDateString();
            //string lgt = "<";
            //string todo = "Hide";

            //if (dtCreateDate > fileDate.AddMonths(1))
            //{
            //    visibility = "visible";
            //    lgt = ">";
            //     todo = "Show";
            //}




            int file_month = int.Parse(dt.Rows[0]["file_month"] +"");
            int file_year = int.Parse(dt.Rows[0]["file_year"] + "");
            string strMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(file_month);

            string subject = "United Enterprise Wide PPACA TAT Report - " + strMonthName + " " + file_year; //October 2022


            string body = File.ReadAllText(emailFilePath);
            //body = body.Replace("{$filename}", fileSearch);
            //body = body.Replace("{$filedate}", filedate);
            //body = body.Replace("{$createdate}", creatdate);
            //body = body.Replace("{$lgt}", lgt);
            //body = body.Replace("{$todo}", todo);
            //body = body.Replace("{$visibility}", visibility);
            body = body.Replace("{$month}", strMonthName);
            body = body.Replace("{$year}", file_year.ToString());
            body = body.Replace("{$current_month}", dtCreateDate.ToString("MMMM"));
            body = body.Replace("{$current_year}", dtCreateDate.Year.ToString());
            dr = dt.Select("rtype = 'CS'").FirstOrDefault();
            body = body.Replace("{$tat_val_cs}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_cs}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_cs}", String.Format("{0:n0}", dr["den_tat"]));
            dr = dt.Select("rtype = 'MR'").FirstOrDefault();
            body = body.Replace("{$tat_val_mr}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_mr}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_mr}", String.Format("{0:n0}", dr["den_tat"]));
            dr = dt.Select("rtype = 'COMM'").FirstOrDefault();
            body = body.Replace("{$tat_val_comm}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_comm}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_comm}", String.Format("{0:n0}", dr["den_tat"]));
            dr = dt.Select("rtype = 'OX'").FirstOrDefault();
            body = body.Replace("{$tat_val_ox}", ((double)dr["tat_val"]).ToString("#0.##%"));
            body = body.Replace("{$num_tat_ox}", String.Format("{0:n0}", dr["num_tat"]));
            body = body.Replace("{$den_tat_ox}", String.Format("{0:n0}", dr["den_tat"]));

            //return;

            //            sbEmail.Append("<div style='color:#2F5496;'>");
            //            sbEmail.Append("<p>Hi all- </p>");

            //            sbEmail.Append("<p>Attached please find the <b>United Enterprise - Wide PPACA TAT Report</b> received from eviCore Healthcare for <b>" + strMonthName + " " + file_year + "</b></p>");


            //            //IF THIS VS THAT THEN
            //            sbEmail.Append("<p  style='color:red;'><b>**Please be aware that the " + strMonthName + " " + file_year + " data provided by eviCore was delayed as it was sent to UnitedHealthcare in late "+ DateTime.Now.ToString("MMMM") + ", "+DateTime.Now.Year+". * * </b></p>");

            //            sbEmail.Append("<p>As a reminder, we are providing this report to your team on a monthly basis to support monitoring urgent case TAT for our Notification/ Prior Authorization Programs, in compliance with original requirements within the Patient Protection and Affordable Care Act(PPACA).The attached report contains a summary view as well as a split between line of business and program. </p>");

            //            sbEmail.Append("<p>Here is a summary of the data by lines of business:  </p> <p></p>");

            //            dr = dt.Select("rtype = 'CS'").FirstOrDefault();
            //            sbEmail.Append("<UL>");
            //            sbEmail.Append("<LI>For Community & State, <b>" + dr["tat_val"] + "% (" + String.Format("{0:n0}", dr["num_tat"]) + "/" + String.Format("{0:n0}", dr["den_tat"]) + ")</b> of urgent radiology and cardiology authorizations were within the contractual compliance expedited turn - around - times.</LI> ");
            //            dr = dt.Select("rtype = 'MR'").FirstOrDefault();
            //            sbEmail.Append("<LI>For Medicare & Retirement, <b>" + dr["tat_val"] + "% (" + String.Format("{0:n0}", dr["num_tat"]) + "/" + String.Format("{0:n0}", dr["den_tat"]) + ")</b> of urgent radiology and cardiology authorizations were within the contractual compliance expedited turn - around - times.</LI> ");
            //            sbEmail.Append("<LI>Within the commercial lines of business: ");
            //            dr = dt.Select("rtype = 'COMM'").FirstOrDefault();
            //            sbEmail.Append("<UL>");
            //            sbEmail.Append("<LI>UHC UNET Prior Auth segment completed <b>" + dr["tat_val"] + "% (" + String.Format("{0:n0}", dr["num_tat"]) + "/" + String.Format("{0:n0}", dr["den_tat"]) + ")</b> of urgent radiology and cardiology notifications / authorizations within the contractual compliance expedited turn - around - times; and </LI>");
            //            dr = dt.Select("rtype = 'OX'").FirstOrDefault();
            //            sbEmail.Append("<LI>Oxford completed <b>" + dr["tat_val"] + "% (" + String.Format("{0:n0}", dr["num_tat"]) + "/" + String.Format("{0:n0}", dr["den_tat"]) + ")</b> authorizations within the contractual compliance turn-around - times.</LI> ");
            //            sbEmail.Append("</UL></LI>");
            //            sbEmail.Append("<LI>The commercial individual exchanges are now also reported separately from United PA and are linked at the bottom of the[Document Map] tab.</LI> ");
            //            sbEmail.Append("</UL>");


            //            sbEmail.Append("<p>Please let me know if you have any questions.</p>");
            //sbEmail.Append("<p>Thank you.</p>");

            //            sbEmail.Append("<b>Mary Ann DiMartino, RN, BSN</b><br/>");
            //            sbEmail.Append("Principal Data Analyst, Clinical Analytics & Development<br/>");
            //            sbEmail.Append("UCS – Value Creation<br/>");

            //            sbEmail.Append("<p>O(952)202-7990</p>");

            //            sbEmail.Append("</div>");

            var manual = @"C:\Users\cgiorda\Desktop\Projects\PPACA_TAT\Archive\United_Enterprise_Wide_Urgent_TAT_UHC_Enterprise_2023_05.xlsx";


            HelperFunctions.HelperFunctions.Email(recipients, from, subject, body, cc, manual);

        }
    }
}
