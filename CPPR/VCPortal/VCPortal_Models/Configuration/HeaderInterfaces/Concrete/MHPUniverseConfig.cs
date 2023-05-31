using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class MHPUniverseConfig : IMHPUniverseConfig
{
    public string Name { get; set; }

    public List<APIConfig> APIS { get; set; }
}
