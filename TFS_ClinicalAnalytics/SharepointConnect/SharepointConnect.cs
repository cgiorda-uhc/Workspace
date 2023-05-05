using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using ClientOM = Microsoft.SharePoint.Client;
using System.Security;

namespace SharepointConnect
{
    public static class SharepointConnect
    {
        //https://learn.microsoft.com/en-us/answers/questions/90187/sharepoint-app-only-add-ins-throwing-401-unauthori.html?page=1&pageSize=10&sort=oldest
        //https://csharpforums.net/threads/intermittent-the-remote-server-returned-an-error-401-unauthorized-on-clientcontext-executequery.6895/
        public static void SharePointUpload()
        {
            string userName = "chris_giordano@uhc.com";
            string password = "BooWooDooFoo2023!!";
            var securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }

            ClientContext clientContext = new ClientContext("https://uhgazure.sharepoint.com/sites/csapmo/");
            clientContext.AuthenticationMode = ClientAuthenticationMode.Default;
            clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
            Web site = clientContext.Web;
            clientContext.Load(site);
            clientContext.ExecuteQuery();
            Console.WriteLine("Title: {0}", site.Title);







            //string userName = "chris_giordano@uhc.com";
            //string password = "BooWooDooFoo2023!!";
            //var securePassword = new SecureString();
            //foreach (char c in password)
            //{
            //    securePassword.AppendChar(c);
            //}
            //using (var clientContext = new ClientContext("https://uhgazure.sharepoint.com/sites/csapmo/"))
            //{
            //    clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassword);
            //    clientContext.AuthenticationMode = ClientAuthenticationMode.Default;

            //    //FormsAuthenticationLoginInfo formsAuthInfo = new FormsAuthenticationLoginInfo(userName, password);
            //    // clientContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
            //    //clientContext.FormsAuthenticationLoginInfo = formsAuthInfo;



            //    Web web = clientContext.Web;
            //    clientContext.Load(web, a => a.ServerRelativeUrl);
            //    clientContext.ExecuteQuery();
            //    List documentsList = clientContext.Web.Lists.GetByTitle("Contact");

            //    var fileCreationInformation = new FileCreationInformation();
            //    //Assign to content byte[] i.e. documentStream

            //    fileCreationInformation.Content = System.IO.File.ReadAllBytes(@"D:\document.pdf");
            //    //Allow owerwrite of document

            //    fileCreationInformation.Overwrite = true;
            //    //Upload URL

            //    fileCreationInformation.Url = "https://testlz.sharepoint.com/sites/jerrydev/" + "Contact/demo" + "/document.pdf";

            //    Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

            //    //Update the metadata for a field having name "DocType"
            //    uploadFile.ListItemAllFields["Title"] = "UploadedviaCSOM";

            //    uploadFile.ListItemAllFields.Update();
            //    clientContext.ExecuteQuery();

            //}
  
        }





        public static void Flist3()
        {

            //ClientContext clientContext = new ClientContext(@"https://uhgazure.sharepoint.com/sites/SBS/Projects/Medical%20Necessity%20Resource/Medical%20Necessity%20ACIS%20and%20Quarterly%20Reports%20for%20N/Forms/AllItems.aspx?RootFolder=%2Fsites%2FSBS%2FProjects%2FMedical%20Necessity%20Resource%2FMedical%20Necessity%20ACIS%20and%20Quarterly%20Reports%20for%20N%2FMonthly%20ACIS%20Report%20for%20Med%20Nec&FolderCTID=0x012000971FB377F01DFC45947863A959FD81A4&View=%7B96C9AE2C%2DF17B%2D4C3E%2DBFE2%2D8EDDEDDF5BC0%7D");
            SharePointOnlineCredentials credentials = new SharePointOnlineCredentials("chris_giordano@uhc.com", ConvertToSecureString("BooWooDooFoo2023!!"));
            ClientContext clientContext = new ClientContext(@"https://uhgazure.sharepoint.com/SitePages/Home.aspx");
            clientContext.Credentials = credentials;  // passing credentials in case you need to work with Sharepoint Online
            using (clientContext)
            {

                clientContext.ExecuteQuery();



                List list = clientContext.Web.Lists.GetByTitle("Monthly ACIS Report for Med Nec");
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = @"<View Scope='Recursive'><Query></Query></View>";
                Folder ff = list.RootFolder;
                FolderCollection fcol = list.RootFolder.Folders; // here you will save the folder info inside a Folder Collection list
                List<string> lstFile = new List<string>();
                FileCollection ficol = list.RootFolder.Files;   // here you will save the File names inside a file Collection list 
                                                                // ------informational -------
                clientContext.Load(ff);
                clientContext.Load(list);
                clientContext.Load(list.RootFolder);
                clientContext.Load(list.RootFolder.Folders);
                clientContext.Load(list.RootFolder.Files);
                clientContext.ExecuteQuery();
                Console.WriteLine("Root : " + ff.Name + "\r\n");
                Console.WriteLine(" ItemCount : " + ff.ItemCount.ToString());
                Console.WriteLine(" Folder Count : " + ff.Folders.Count.ToString());
                Console.WriteLine(" File Count : " + ff.Files.Count.ToString());

                Console.WriteLine(" URL : " + ff.ServerRelativeUrl);
                //---------------------------
                //---------Here you iterate through the files and not the folders that are in the root folder ------------
                foreach (ClientOM.File f in ficol)
                {
                    Console.WriteLine("Files Name:" + f.Name);
                }
                //-------- here you will iterate through the folders and the files inside the folders that reside in the root folder----
                foreach (Folder f in fcol)
                {
                    Console.WriteLine("Folder Name : " + f.Name);
                    clientContext.Load(f.Files);
                    clientContext.ExecuteQuery();
                    FileCollection fileCol = f.Files;
                    foreach (ClientOM.File file in fileCol)
                    {
                        lstFile.Add(file.Name);
                        Console.WriteLine(" File Name : " + file.Name);

                    }
                }


            }

        }

        private static SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }


    }
}
