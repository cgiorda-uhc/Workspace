using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace UCS_Project_Manager
{
    public class OutlookHelper
    {

        private Outlook.Application outlookAppGLOBAL;
        private Outlook.NameSpace outlookNameSpaceGLOBAL;

        public void FindContactEmailByName(string firstName, string lastName)
        {

           // if(outlookAppGLOBAL == null)
          //  {
                //TESTING DELETE ME!!!!
                //TESTING DELETE ME!!!!
                foreach (Process Proc in Process.GetProcesses())
                    if (Proc.ProcessName.Equals("OUTLOOK") || Proc.ProcessName.Equals("OUTLOOK"))  //Process Excel?
                        Proc.Kill();

                outlookAppGLOBAL = new Outlook.Application();
 

                outlookNameSpaceGLOBAL = outlookAppGLOBAL.GetNamespace("MAPI");
               // outlookNameSpaceGLOBAL.Logon("", "", Missing.Value, Missing.Value);


            ////LEVERAGE EXISTING OUTLOOK
            //if (Process.GetProcessesByName("OUTLOOK").Count() > 0)
            //    {
            //        outlookAppGLOBAL = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
            //    }
            //    else //CREATE NEW OUTLOOK
            //    {
            //        outlookAppGLOBAL = new Outlook.Application();
            //    }

            //    outlookNameSpaceGLOBAL = outlookAppGLOBAL.GetNamespace("MAPI");
            //    outlookNameSpaceGLOBAL.Logon("", "", Missing.Value, Missing.Value);
            //}
           

            //Outlook.NameSpace outlookNameSpace = outlookApp.GetNamespace("MAPI");
            Outlook.MAPIFolder contactsFolder = outlookNameSpaceGLOBAL.GetDefaultFolder(
                Microsoft.Office.Interop.Outlook.
                OlDefaultFolders.olFolderInbox);

            Outlook.Items contactItems = contactsFolder.Items;





            string strFilter = "@SQL=\"urn:schemas:httpmail:subject\" like '%chr%'";
            strFilter = "@SQL=\"urn:schemas:httpmail:toemail\" like 'chris_giordano@uhc.com'";

            Outlook.Items filteredItems = contactItems.Restrict(strFilter);

            foreach(object f in  filteredItems)
            {

               

                string ff = f.ToString();
                ff = "";
            }


            //Outlook.ContactItem contact2 = contactItems.Find("[Subject] > 's' And [Subject] <'u'");
            Outlook.ContactItem contact1 = contactItems.Find("[From]='Inna Rudi'");
          //  Outlook.ContactItem contact1 = contactItems.Find("[FirstName]='Inna'");

            GC.Collect();

            try
            {
                Outlook.ContactItem contact =
                    (Outlook.ContactItem)contactItems.
                    Find(String.Format("[FirstName]='{0}' and "
                    + "[LastName]='{1}'", firstName, lastName));
                if (contact != null)
                {
                    contact.Display(true);
                }
                else
                {
                    string strTest = "The contact information was not found.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
