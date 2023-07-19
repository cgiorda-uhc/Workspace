using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.DQC_Reporting;
public class LOB_Major_Market_PEG_Model : iOpportunityCompliant
{
    public string LOB { get; set; }

    public string Major_Market { get; set; }

    public string PEG { get; set; }
    public int Previous_Opportunity { get; set; }
    public int Previous_Compliant { get; set; }
    public int Current_Opportunity { get; set; }
    public int Current_Compliant { get; set; }
}
