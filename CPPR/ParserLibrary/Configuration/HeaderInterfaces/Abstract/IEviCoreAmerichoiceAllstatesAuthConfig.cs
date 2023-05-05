using ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract
{
    public interface IEviCoreAmerichoiceAllstatesAuthConfig
    {
        int Delay { get; set; }
        List<EmailConfig> EmailLists { get; set; }
        List<FileExcelConfig> FileLists { get; set; }
        string Name { get; set; }
        ProjectType ProjectType { get; set; }
        string Schedule { get; set; }
        List<SQLConfig> SQLLists { get; set; }
    }
}