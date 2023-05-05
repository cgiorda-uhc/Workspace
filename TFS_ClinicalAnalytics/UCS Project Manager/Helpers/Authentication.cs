using SASOMI;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace UCS_Project_Manager.Helpers
{
    internal class Authentication
    {

        public static string getUser()
        {
            return WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        }


        public  static bool isMemberOf(string groupName)
        {
           var arr = WindowsIdentity.GetCurrent().Name.Split('\\');
            string domain = arr[0];
            string userName = arr[1];


            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain);

            // find a user
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

            // find the group in question
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);

            if (user == null)
                throw new ApplicationException(string.Format("User {0} not found.", userName));

            if (group == null)
                throw new ApplicationException(string.Format("Group {0} not found.", groupName));


            var foundUsers = group.GetMembers(true).Where(p => p.SamAccountName.Equals(user.SamAccountName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            // check if user is member of that group
            if (foundUsers != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
