using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Configuration.RoleInterfaces
{
    public class ProjectConfig : IProjectConfig
    {
        public string Name { get; set; }

        public ProjectType ProjectType { get; set; }

        public string Schedule { get; set; }

        public int Delay { get; set; }

        public List<EmailConfig> EmailLists { get; set; }
    }
}
