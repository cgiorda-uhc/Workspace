using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Outlook = Microsoft.Office.Interop.Outlook;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    static class OutlookHelper
    {

        public static bool sendEmail(string strEmailAddress, string strEmailAddressCC, string strSubject, string strMessage)
        {
            Outlook.Application oapp;
            Outlook.MailItem omessage;
            bool blEmailSent = false;

            bool blNewInstance = false;
            try
            {
                if (Process.GetProcessesByName("OUTLOOK").Count() > 0)
                {
                    oapp = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
                }
                else
                {
                    oapp = new Outlook.Application();
                    Outlook.NameSpace nameSpace = oapp.GetNamespace("MAPI");
                    nameSpace.Logon("", "", Missing.Value, Missing.Value);
                    nameSpace = null;
                    blNewInstance = true;
                }


               //omessage = new Outlook.MailItem();

                omessage = oapp.CreateItem(Outlook.OlItemType.olMailItem);

               // omessage.To = "amie.r.cook@uhc.com";

                omessage.To = "chris_giordano@uhc.com";
                omessage.To = strEmailAddress;
                omessage.CC = strEmailAddressCC;


                omessage.Subject = "Engagement Added to PEI";

                omessage.Body = strMessage;

                omessage.Send();

                blEmailSent = true;

            }
            catch(Exception ex)
            {
                blEmailSent = false;
            }
            finally
            {
                if (blNewInstance)
                {
                    oapp = null;
                }
                omessage = null;
            }

            return blEmailSent;
        }

    }
}
