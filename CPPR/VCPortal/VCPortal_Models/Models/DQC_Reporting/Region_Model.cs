using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.DQC_Reporting;
public class Region_Model : iOpportunityCompliant
{
    public string Region { get; set; }
    public int Previous_Opportunity { get; set; }
    public int Previous_Compliant { get; set; }
    public int Current_Opportunity { get; set; }
    public int Current_Compliant { get; set; }
}

