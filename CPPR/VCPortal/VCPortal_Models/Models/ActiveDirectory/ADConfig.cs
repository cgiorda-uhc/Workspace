using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ActiveDirectory;
public class ADConfig : IADConfig
{
    public string Name { get; set; }

    public string LDAPDomain { get; set; }
    public string LDAPPath { get; set; }
    public string LDAPUser { get; set; }
    public string LDAPPW { get; set; }
    public string SearchString { get; set; }

}
