using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Configuration.RoleInterfaces;

public interface IGeneric
{
    public string Name { get; set; }
    public string FileStagingArea { get; set; }
}
