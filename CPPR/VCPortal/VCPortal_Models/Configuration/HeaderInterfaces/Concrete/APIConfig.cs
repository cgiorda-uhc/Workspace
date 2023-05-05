using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class APIConfig : IAPIConfig
{
    public string Name { get; set; }
    public string Display { get; set; }
    public string Url { get; set; }
    public string BaseUrl { get; set; }

}
