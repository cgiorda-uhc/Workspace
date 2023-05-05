using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

public class FileConfig : IFileConfig
{
    public string ZippedFile { get; set; }
    public string ZippedMatch { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public FileFormat FileFormat { get; set; }
    public string Destination { get; set; }
}
