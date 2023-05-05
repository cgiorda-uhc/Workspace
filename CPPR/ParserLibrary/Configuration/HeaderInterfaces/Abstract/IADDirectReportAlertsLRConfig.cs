

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;

public interface IADDirectReportAlertsLRConfig
{
    int Delay { get; set; }
    List<EmailConfig> EmailLists { get; set; }
    string LDAPDomain { get; set; }
    string LDAPPath { get; set; }
    string LDAPPW { get; set; }
    string LDAPUser { get; set; }
    string Name { get; set; }
    ProjectType ProjectType { get; set; }
    string Schedule { get; set; }
    public List<string> SearchString { get; set; }
    SearchType SearchType { get; set; }
    List<SQLConfig> SQLLists { get; set; }
}