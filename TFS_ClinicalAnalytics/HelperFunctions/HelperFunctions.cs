using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace HelperFunctions
{
    public static class HelperFunctions
    {
        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static string CreateTableSQLFromDataTable(string tableName, DataTable table)
        {
            string sqlsc;
            sqlsc = "CREATE TABLE " + tableName + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        }



        public static void Email(string strTo, string strFrom, string strSubect, string strBody, string strCC = null, string strAttachmentPaths = null, MailPriority mp = MailPriority.Normal)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(strFrom);

                //LOOP ; separated??
                string[] strRecipientArr = strTo.Split(';');
                foreach(string s in strRecipientArr)
                    message.To.Add(new MailAddress(s.Trim()));

                if(strCC != null)
                {
                    string[] strCCArr = strCC.Split(';');
                    foreach (string s in strCCArr)
                        message.CC.Add(new MailAddress(s.Trim()));
                }
 

                if (strAttachmentPaths != null)
                {
                    string[] strAttachmentPathArr = strAttachmentPaths.Split(';');
                    foreach (string s in strAttachmentPathArr)
                        message.Attachments.Add(new Attachment(s.Trim()));
                }

                message.Priority = mp;

                message.Subject = strSubect;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = strBody;
                smtp.Port = 25;
                smtp.Host = "mailo2.uhc.com"; //for gmail host  
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = true;
                //smtp.Credentials = new NetworkCredential("ms/peisaid", "Iluv2playtennis&bsktball");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                    }
        }


    }



}
