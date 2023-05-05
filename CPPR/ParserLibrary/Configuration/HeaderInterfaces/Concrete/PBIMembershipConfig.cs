using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;
using ProjectManagerLibrary.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete
{
    public class PBIMembershipConfig : IPBIMembershipConfig
    {
        public string Name { get; set; }
        public ProjectType ProjectType { get; set; }
        public string Schedule { get; set; }
        public List<EmailConfig> EmailLists { get; set; }
        public int Delay { get; set; }
        public string LDAPDomain { get; set; }
        public string LDAPPath { get; set; }
        public string LDAPUser { get; set; }
        public string LDAPPW { get; set; }
        public string SearchString { get; set; }
        public SearchType SearchType { get; set; }
        public List<SQLConfig> SQLLists { get; set; }
    }
}
