using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicianFeedbackTracker
{
    class ActiveDirectoryFunctions
    {

        private static string _strADUserName;
        public static string strADUserName
        {
            get { return _strADUserName; }
            set { _strADUserName = value; }
        }

        private static string _strADPassword;
        public static string strADPassword
        {
            get { return _strADPassword; }
            set { _strADPassword = value; }
        }



        public static bool hasAccess(string strUsername, string strGroupName)
        {

            UserAccess ua = GlobalObjects.getUserAccess(strUsername, strGroupName);
            return ua.blHasPermission;

            //bool test = ActiveDirectoryFunctions.isUserInGroups(strUsername, "pei2_users");
            //List<Principal> result1 = ActiveDirectoryFunctions.GetGroupsUsers("pei2_users");
            //List<GroupPrincipal> result2 = ActiveDirectoryFunctions.GetUsersGroups("cgiorda");
        }



        public static bool isUserInGroups(string strUserName, string strGroupName)
        {
            // establish domain context

            PrincipalContext context = new PrincipalContext(ContextType.Domain);

            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.Name, strGroupName);

            bool blUserInGroup = false;
            // if found - grab its groups
            if (group != null)
            {
                var tmp = group.GetMembers().Where(g => g.Name.Contains(strUserName)).FirstOrDefault();
                if (tmp != null)
                {
                    blUserInGroup = true;
                }


            }

            return blUserInGroup;
        }





        public static List<GroupPrincipal> GetUsersGroups(string strUserName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            // establish domain context
            PrincipalContext context = new PrincipalContext(ContextType.Domain, "ms.ds.uhc.com", "cgiorda", "Sigmund23");

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(context, strUserName);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }

            return result;
        }




        public static List<string> GetADInfo(string strUserName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            // establish domain context
            PrincipalContext context = new PrincipalContext(ContextType.Domain, "ms.ds.uhc.com", _strADUserName, _strADPassword);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(context, strUserName);

            List<string> lstADInfo = null;

            // if found - grab its groups
            if (user != null)
            {
                lstADInfo = new List<string>();
                lstADInfo.Add(user.GivenName);
                lstADInfo.Add(user.Surname);
                lstADInfo.Add(user.EmailAddress);
            }

            return lstADInfo;
        }




        public static List<Principal> GetGroupsUsers(string strGroupName)
        {
            List<Principal> result = new List<Principal>();

            // establish domain context
           // PrincipalContext context = new PrincipalContext(ContextType.Domain, "ms.ds.uhc.com", "cgiorda", "xxxxxxxxxxxx");
            PrincipalContext context = new PrincipalContext(ContextType.Domain);

            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, IdentityType.Name, strGroupName);


            // if found - grab its groups
            if (group != null)
            {
                PrincipalSearchResult<Principal> groups = group.GetMembers();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                     result.Add(p);
                }

            }

            return result;
        }


    }
}
