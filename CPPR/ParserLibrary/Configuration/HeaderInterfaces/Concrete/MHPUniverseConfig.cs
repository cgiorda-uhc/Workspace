


namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

public class MHPUniverseConfig : IMHPUniverseConfig
{
    public int Delay { get; set; }
    public List<EmailConfig> EmailLists { get; set; }
    public List<FileExcelConfig> FileLists { get; set; }
    public string Name { get; set; }
    public ProjectType ProjectType { get; set; }
    public string Schedule { get; set; }
    public List<SQLConfig> SQLLists { get; set; }
}
