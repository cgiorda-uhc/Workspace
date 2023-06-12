using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract
{
    public interface IAppsettings : IProjectConfig, IADConfig, ISearchConfig
    {
        string Name { get; set; }
        ProjectType ProjectType { get; set; }
        string Schedule { get; set; }
        List<EmailConfig> EmailLists { get; set; }
        int Delay { get; set; }
        string LDAPDomain { get; set; }
        string LDAPPath { get; set; }
        string LDAPUser { get; set; }
        string LDAPPW { get; set; }//BooWooDooFoo2023!!
        string SearchString { get; set; }
        SearchType SearchType { get; set; }
        List<SQLConfig> SQLLists { get; set; }
    }
}
