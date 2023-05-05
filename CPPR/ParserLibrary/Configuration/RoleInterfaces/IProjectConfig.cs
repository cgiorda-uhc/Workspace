
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Configuration.RoleInterfaces
{
    public interface IProjectConfig
    {
        public string Name { get; set; }

        public ProjectType ProjectType { get; set; }

        public string Schedule { get; set; }

        public int Delay { get; set; }

        public List<EmailConfig> EmailLists { get; set; }

    }
}
