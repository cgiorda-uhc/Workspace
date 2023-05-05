using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ActiveDirectory;
public class PBIMembershipModel
{
    public string userid { get; set; }
    public string email { get; set; }
    public string global_group { get; set; }
    public string department { get; set; }
}