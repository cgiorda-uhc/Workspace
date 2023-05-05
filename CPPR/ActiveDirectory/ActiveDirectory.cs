using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ActiveDirectoryLibrary
{

    public class ActiveDirectory
    {


        private DirectoryEntry _directoryEntry = null;

        private DirectoryEntry SearchRoot
        {
            get
            {

                if (_directoryEntry == null)
                {

                    _directoryEntry = new DirectoryEntry(LDAPPath, LDAPUser, LDAPPassword, AuthenticationTypes.Secure);//, LDAPUser, LDAPPassword, AuthenticationTypes.Secure);

                    //var token = System.Security.Principal.WindowsIdentity.GetCurrent().AccessToken;

                    //WindowsIdentity.RunImpersonatedAsync(token, async () =>
                    //{
                    //_directoryEntry = new DirectoryEntry(LDAPPath);//, LDAPUser, LDAPPassword, AuthenticationTypes.Secure);
                    //});

                }
                return _directoryEntry;
            }
        }


        public String LDAPPath { get; set; }
        public String LDAPDomain { get; set; }
        public String LDAPUser { get; set; }
        public String LDAPPassword { get; set; }

        public List<string> GetGroupByName(String groupName)
        {
            List<string> lstGroup = new List<string>();

            _directoryEntry = null;
            DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);

            directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
            directorySearch.SearchScope = SearchScope.Subtree;

            SearchResultCollection results = directorySearch.FindAll();

            for (int i = 0; i < results.Count; i++)
            {
                DirectoryEntry de = results[i].GetDirectoryEntry();
                lstGroup.Add(de.Name);

                //TODO with "de"
            }



            // find all matches
            //foreach (var found in srch.FindAll())
            //{
            //    GroupPrincipal foundGroup = found as GroupPrincipal;

            //    if (foundGroup != null)
            //    {
            //        lstGroup.Add(found.DisplayName);
            //    }


            //}




            return lstGroup;

        }




        internal ADUserModel GetUserByFullName(String userName)
        {
            try
            {
                //using (HostingEnvironment.Impersonate())
                //{
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
                //}
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
               // using (HostingEnvironment.Impersonate())
               // {

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
                //}

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
                //using (HostingEnvironment.Impersonate())
               // {
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
                //}
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
            public List<ADUserModel> GetUserFromGroup(String groupName)
        {
            List<ADUserModel> userlist = new List<ADUserModel>();
            try
            {
                //using (HostingEnvironment.Impersonate())
                //{
                    _directoryEntry = null;
                    DirectorySearcher directorySearch = new DirectorySearcher(SearchRoot);
                    directorySearch.Filter = "(&(objectClass=group)(SAMAccountName=" + groupName + "))";
                    SearchResult results = directorySearch.FindOne();
                    if (results != null)
                    {

                        DirectoryEntry deGroup = new DirectoryEntry(results.Path);//, LDAPUser, LDAPPassword);
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
                    return userlist;
                //}
            }
            catch (Exception ex)
            {
                return userlist;
            }

        }

    
        public List<ADUserModel> GetUsersByName(string strName, int sizeLimit = 10)
        {
            //using (HostingEnvironment.Impersonate())
            //{

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
            //}
        }

        public bool AddUserToGroup(string userlogin, string groupName)
        {
            try
            {
                //using (HostingEnvironment.Impersonate())
                //{
                    _directoryEntry = null;
                    ADManager admanager = new ADManager(LDAPDomain);//, LDAPUser, LDAPPassword);
                    admanager.AddUserToGroup(userlogin, groupName);
                    return true;
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveUserToGroup(string userlogin, string groupName)
        {
            try
            {
                //using (HostingEnvironment.Impersonate())
               // {
                    _directoryEntry = null;
                    ADManager admanager = new ADManager("xxx");// LDAPUser, LDAPPassword);
                    admanager.RemoveUserFromGroup(userlogin, groupName);
                    return true;
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }



}








