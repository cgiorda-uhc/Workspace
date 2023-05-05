using System;
using System.Linq;
using System.Reflection;

using Outlook = Microsoft.Office.Interop.Outlook;
using System.Diagnostics;
using System.Runtime.InteropServices;


    public class OutlookHelper
    {
        public static bool sendEmail(string strEmailAddress, string strSubject, string strMessage, string strEmailAddressCC=null)
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


                omessage.Subject = strSubject;

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



        public static void generateEmail(string strEmailAddress, string strSubject, string strMessage, string strEmailAddressCC = null, string strAttachmentPath = null)
        {
            Outlook.Application oapp;
            Outlook.MailItem omessage;

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

                omessage.To = (String.IsNullOrEmpty(strEmailAddress)? "" : strEmailAddress);
                omessage.CC = (String.IsNullOrEmpty(strEmailAddressCC) ? "" : strEmailAddressCC);


                omessage.Subject = (String.IsNullOrEmpty(strSubject) ? "" : strSubject);

                omessage.Body = (String.IsNullOrEmpty(strMessage) ? "" : strMessage);

                if(!String.IsNullOrEmpty(strAttachmentPath))
                {
                    omessage.Attachments.Add(strAttachmentPath);
                }

                omessage.Display();


                //omessage.Send();


            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (blNewInstance)
                {
                    oapp = null;
                }
               // omessage = null;
            }

        }

    }

