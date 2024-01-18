using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class ProcCodeTrendConfig : IProcCodeTrendConfig
{
    public string Name { get; set; }

    public List<APIConfig> APIS { get; set; }

    public List<CommentsConfig> Comments { get; set; }
}
