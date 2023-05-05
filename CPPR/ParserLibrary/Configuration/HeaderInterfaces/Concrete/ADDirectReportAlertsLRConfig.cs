

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete
{
    public class ADDirectReportAlertsLRConfig : IADDirectReportAlertsLRConfig
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
        public List<string> SearchString { get; set; }
        public SearchType SearchType { get; set; }
        public List<SQLConfig> SQLLists { get; set; }
    }
}
