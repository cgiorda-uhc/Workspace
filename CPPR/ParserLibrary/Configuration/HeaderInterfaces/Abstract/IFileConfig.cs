using Teradata.Client.Provider;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;

public interface IFileConfig
{
    string ZippedFile { get; set; }
    public string ZippedMatch { get; set; }
    string FileName { get; set; }
    string FilePath { get; set; }
    FileFormat FileFormat { get; set; }
    string Destination { get; set; }

}
