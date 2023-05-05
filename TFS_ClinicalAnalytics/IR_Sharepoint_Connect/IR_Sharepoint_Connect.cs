using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class IR_Sharepoint_Connect
{

    public static void getFilesFromPath(string strURL, string strFolderPath)
    {



        //ClientContext ctx = new ClientContext(strURL);


        //ctx.AuthenticationMode = ClientAuthenticationMode.Default;
        //ctx.Credentials = new SharePointOnlineCredentials("cgiorda", "BooWooDooFoo2023!!");  //new System.Net.NetworkCredential("cgiorda", "BooWooDooFoo2023!!", "MS");


        //ctx.ExecuteQuery();

       







        //ClientContext _clientContext = new ClientContext(strURL);
        //Web _web = _clientContext.Web;

        //_clientContext.Load(_web, website => website.Title);
        //_clientContext.Load(_web.Webs);

        //CredentialCache cc = new CredentialCache();
        //cc.Add(new Uri(strURL), "NTLM", new System.Net.NetworkCredential("cgiorda", "BooWooDooFoo2023!!", "MS"));
        //_clientContext.Credentials = cc;
        //_clientContext.AuthenticationMode = ClientAuthenticationMode.Default;

        //_clientContext.ExecuteQuery();









        //ClientContext ctx = new ClientContext(strURL);



        //ctx.AuthenticationMode = ClientAuthenticationMode.Default;
        //ctx.Credentials = System.Net.CredentialCache.DefaultCredentials;  //new System.Net.NetworkCredential("cgiorda", "BooWooDooFoo2023!!", "MS");


        //ctx.ExecuteQuery();

        ////FormsAuthenticationLoginInfo login = new FormsAuthenticationLoginInfo(@"ms\cgiorda", "BooWooDooFoo2023!!");
        ////ctx.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
        ////ctx.FormsAuthenticationLoginInfo = login;

        


        ////cxt.Credentials = new System.Net.NetworkCredential(@"cgiorda", "BooWooDooFoo2023!!");

        //// cxt.AuthenticationMode = ClientAuthenticationMode.Default;
        ////cxt.Credentials = System.Net.CredentialCache.DefaultCredentials;



        //List list = ctx.Web.Lists.GetByTitle("Monthly ACIS Report for Med Nec");

        //ctx.Load(list);
        //ctx.Load(list.RootFolder);
        //ctx.Load(list.RootFolder.Folders);
        //ctx.Load(list.RootFolder.Files);
        //ctx.ExecuteQuery();
        //FolderCollection fcol = list.RootFolder.Folders;
        //List<string> lstFile = new List<string>();
        //foreach (Folder f in fcol)
        //{
        //    if (f.Name == "filename")
        //    {
        //        ctx.Load(f.Files);
        //        ctx.ExecuteQuery();
        //        FileCollection fileCol = f.Files;
        //        foreach (File file in fileCol)
        //        {
        //            lstFile.Add(file.Name);
        //        }
        //    }
        //}






        //using (var ctx = new ClientContext(strURL))
        //{
        //    Web web = ctx.Web;
        //    List list = ctx.Web.Lists.GetByTitle("Monthly ACIS Report for Med Nec");
        //    Folder folder = web.GetFolderByServerRelativeUrl(strFolderPath);

        //    ctx.Load(ctx.Web);
        //    ctx.Load(list);
        //    ctx.Load(folder);
        //    ctx.ExecuteQuery();
        //}
    }

}

