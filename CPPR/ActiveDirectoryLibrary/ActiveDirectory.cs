using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryLibrary;


public class ActiveDirectory
{



    private DirectoryEntry _directoryEntry = null;




    private string _LDAPPath;
    private string _LDAPDomain;
    private string _LDAPUser;
    private string _LDAPPassword;


    public ActiveDirectory()
    {

    }


    public ActiveDirectory(string path, string domain, string username, string password)
    {
        _LDAPPath = path;
        _LDAPDomain = domain;
        _LDAPUser = username;
        _LDAPPassword = password;
    }


    private DirectoryEntry SearchRoot
    {
        get
        {
            if (_directoryEntry == null)
            {
                _directoryEntry = new DirectoryEntry(_LDAPPath, _LDAPUser, _LDAPPassword, AuthenticationTypes.Secure);

            }
            return _directoryEntry;
        }
    }


    public void Dispose() 
    {
        _directoryEntry.Dispose();
        _directoryEntry = null;
    }



    //public string getCurrentUser()
    //{
    //    string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

    //    return user;
    //}


    public List<ADUserModel> UsersByManagerList { get; set; }
    public void GetUsersByManager(string username)
    {

        //_directoryEntry = null;
        using (DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot))
        {
            directorySearch.PageSize = 10000;
            directorySearch.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))";
            directorySearch.SearchScope = SearchScope.Subtree;

            SearchResult result = directorySearch.FindOne();
            using (DirectoryEntry user = new DirectoryEntry(result.Path))
            {

                ADUserModel userInfo = ADUserModel.GetUser(user);
                UsersByManagerList.Add(userInfo);

                foreach (string objProperty in result.Properties["DirectReports"])
                {
                    string emp = objProperty.ToString();
                    string[] setp = new string[1];
                    setp[0] = "DC"; //If your users are in a OU use OU 

                    emp = emp.Split(setp, StringSplitOptions.None)[0];
                    emp = emp.Replace("CN=", "");
                    emp = emp.TrimEnd(',');
                    emp = emp.Replace("\\, ", ", ");
                    emp = emp.Split(',')[0];


                    GetUsersByManager(emp);
                }
            }
                
        }
   
      
    }


    public ADUserModel GetUserByUserName(String userName)
    {
        try
        {

            _directoryEntry = null;
            DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
            directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "))";
            SearchResult results = directorySearch.FindOne();

            if (results != null)
            {
                DirectoryEntry user = new DirectoryEntry(results.Path);// LDAPUser, LDAPPassword);
                var final = ADUserModel.GetUser(user);
                final.Groups = GetGroupsByUserName(userName);
                return final;
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            return null;
        }
    }




    internal List<string> GetGroupsByUserName(String username)
    {
        DirectorySearcher ds = new DirectorySearcher();
        ds.Filter = String.Format("(&(objectClass=user)(sAMAccountName={0}))", username);
        SearchResult sr = ds.FindOne();

        DirectoryEntry user = sr.GetDirectoryEntry();
        user.RefreshCache(new string[] { "tokenGroups" });


        List<string> lstGroups = new List<string>();

        for (int i = 0; i < user.Properties["tokenGroups"].Count; i++)
        {
            SecurityIdentifier sid = new SecurityIdentifier((byte[])user.Properties["tokenGroups"][i], 0);
            NTAccount nt = (NTAccount)sid.Translate(typeof(NTAccount));
            //do something with the SID or name (nt.Value)

            if(!lstGroups.Contains(nt.ToString()))
            {
                lstGroups.Add(nt.ToString());
            }
        }

        return lstGroups;
    }




    public List<string> GetGroupByName(String groupName)
    {
        List<string> lstGroup = new List<string>();


        using (DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot))
        {
            directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
            directorySearch.SearchScope = SearchScope.Subtree;

            using (SearchResultCollection results = directorySearch.FindAll())
            {
                for (int i = 0; i < results.Count; i++)
                {
                    DirectoryEntry de = results[i].GetDirectoryEntry();
                    lstGroup.Add(de.Name);
                }
            } 
        }

        return lstGroup;
    }

    public List<ADUserModel> GetUserFromGroup(String groupName)
    {
        List<ADUserModel> userlist = new List<ADUserModel>();
        try
        {
            using (DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot))
            {

                directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
                SearchResult results = directorySearch.FindOne();
                if (results != null)
                {
                    using (DirectoryEntry deGroup = new DirectoryEntry(results.Path))
                    {
                        System.DirectoryServices.PropertyCollection pColl = deGroup.Properties;
                        int count = pColl["member"].Count;


                        for (int i = 0; i < count; i++)
                        {
                            string respath = results.Path;
                            string[] pathnavigate = respath.Split("CN".ToCharArray());
                            respath = pathnavigate[0];
                            string objpath = pColl["member"][i].ToString();
                            string path = respath + objpath;


                            DirectoryEntry user = new DirectoryEntry(path);//, LDAPUser, LDAPPassword);
                            ADUserModel userobj = ADUserModel.GetUser(user);
                            userlist.Add(userobj);
                            user.Close();
                        }
                    }
 
                }
                return userlist;

            }
        }
        catch (Exception ex)
        {
            return userlist;
        }

    }

    internal ADUserModel GetUserByFullName(String userName)
    {
        try
        {
           
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path);// LDAPUser, LDAPPassword);
                    return ADUserModel.GetUser(user);
                }
                else
                {
                    return null;
                }
           
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private static string _strCurrentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("MS\\", "");
    public static string strCurrentUser
    {
        get
        {
            return _strCurrentUser;
        }
    }



    public ADUserModel GetUserByLoginName(String userName)
    {


        try
        {
           

                // This code runs as the application pool user



                _directoryEntry = null;
                string nn = "LDAP://PRIME.local/DC=PRIME,DC=local";
                DirectoryEntry SearchRoot2 = new DirectoryEntry(nn);

                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(SAMAccountName=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path);//, LDAPUser, LDAPPassword);
                    return ADUserModel.GetUser(user);
                }
                return null;
           

        }

        catch (Exception ex)
        {
            return null;
        }
    }


    public ADUserModel GetUserDetailsByFullName(String FirstName, String MiddleName, String LastName)
    {
        //givenName
        //    initials
        //    sn
        //(initials=" + MiddleName + ")(sn=" + LastName + ")

        try
        {
           
                _directoryEntry = null;
                DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                //directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ") ())";

                if (FirstName != "" && MiddleName != "" && LastName != "")
                {

                    directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ")(initials=" + MiddleName + ")(sn=" + LastName + "))";
                }
                else if (FirstName != "" && MiddleName != "" && LastName == "")
                {
                    directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ")(initials=" + MiddleName + "))";
                }
                else if (FirstName != "" && MiddleName == "" && LastName == "")
                {
                    directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + "))";
                }
                else if (FirstName != "" && MiddleName == "" && LastName != "")
                {
                    directorySearch.Filter = "(&(objectClass=user)(givenName=" + FirstName + ")(sn=" + LastName + "))";
                }
                else if (FirstName == "" && MiddleName != "" && LastName != "")
                {
                    directorySearch.Filter = "(&(objectClass=user)(initials=" + MiddleName + ")(sn=" + LastName + "))";
                }
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path);//, LDAPUser, LDAPPassword);
                    return ADUserModel.GetUser(user);
                }
                return null;
            
        }
        catch (Exception ex)
        {
            return null;
        }
    }


   



    /// <summary>
    /// This function will take a DL or Group name and return list of users
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    


    public List<ADUserModel> GetUsersByName(string strName, int sizeLimit = 10)
    {
    

            //UserProfile user;
            List<ADUserModel> userlist = new List<ADUserModel>();
            // string filter = "";

            _directoryEntry = null;
            DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
            directorySearch.Asynchronous = true;
            directorySearch.CacheResults = true;

            string[] strArr = strName.Split(' ');
            StringBuilder sb = new StringBuilder();
            sb.Append("(&(objectCategory=person)(objectClass=user)");

            string strFirstName = "*";
            string strLastName = null;

            if (strArr.Length == 1)
            {
                if (strArr[0].Trim().Length > 0)
                    strFirstName = strArr[0].Trim() + "*";
            }
            else if (strArr.Length == 2)
            {
                if (strArr[1].Trim().Length > 0)
                {
                    strFirstName = strArr[0].Trim() + "*";
                    strLastName = strArr[1].Trim() + "*";
                }
                else
                    strFirstName = strArr[0].Trim() + "*";

            }
            else if (strArr.Length == 3)
            {
                if (strArr[2].Trim().Length > 0)
                {
                    strFirstName = strArr[0].Trim() + " " + strArr[1].Trim() + "*";
                    strLastName = strArr[2].Trim() + "*";
                }
                else
                {
                    strFirstName = strArr[0].Trim() + "*";
                    strLastName = strArr[1].Trim() + "*";
                }
            }


            if (strLastName != null)
                sb.Append(string.Format("(givenname={0})(sn={1})", strFirstName, strLastName));
            else
                sb.Append(string.Format("(|(givenname={0})(sn={0}))", strFirstName));

            //sb.Append("(givenname=* *)");


            sb.Append("(mail=*))");

            directorySearch.Filter = sb.ToString();
            directorySearch.SizeLimit = 5;
            // directorySearch.PageSize = 1000;
            try
            {
                SearchResultCollection userCollection = directorySearch.FindAll();

                foreach (SearchResult users in userCollection)
                {
                    DirectoryEntry userEntry = new DirectoryEntry(users.Path);//, LDAPUser, LDAPPassword);
                    ADUserModel userInfo = ADUserModel.GetUser(userEntry);

                    userlist.Add(userInfo);

                }
            }
            catch (Exception)
            {

            }


            return userlist;
   
    }

    

}








