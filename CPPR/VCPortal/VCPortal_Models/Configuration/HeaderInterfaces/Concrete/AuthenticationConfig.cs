using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class AuthenticationConfig
{
    public string Name { get; set; }

    public APIConfig API { get; set; }
}
