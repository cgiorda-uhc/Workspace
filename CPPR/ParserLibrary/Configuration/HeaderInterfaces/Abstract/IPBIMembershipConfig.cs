using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract
{
    public interface IPBIMembershipConfig : IProjectConfig, IADConfig, ISearchConfig
    {
        string Name { get; set; }
        ProjectType ProjectType { get; set; }
        string Schedule { get; set; }
        List<EmailConfig> EmailLists { get; set; }
        int Delay { get; set; }
        string LDAPDomain { get; set; }
        string LDAPPath { get; set; }
        string LDAPUser { get; set; }
        string LDAPPW { get; set; }
        string SearchString { get; set; }
        SearchType SearchType { get; set; }
        List<SQLConfig> SQLLists { get; set; }

    }
}
