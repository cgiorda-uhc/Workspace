using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class ChemotherapyPXConfig : IChemotherapyPXConfig
{
    public string Name { get; set; }

    public List<APIConfig> APIS { get; set; }
}
