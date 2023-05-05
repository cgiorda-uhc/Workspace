using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

public class EviCoreMRMembershipDetailsConfig : IEviCoreMRMembershipDetailsConfig
{
    public string Name { get; set; }
    public ProjectType ProjectType { get; set; }
    public string Schedule { get; set; }
    public List<EmailConfig> EmailLists { get; set; }
    public int Delay { get; set; }

    public List<FileExcelConfig> FileLists { get; set; }

    public List<SQLConfig> SQLLists { get; set; }
}
