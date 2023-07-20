using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.DQC_Reporting;
public class LOB_Spec_Quality_Metric_Model : iOpportunityCompliant
{
    public string LOB { get; set; }

    public string Specialty { get; set; }

    public string Quality_Metric { get; set; }

    public int Previous_Opportunity { get; set; }
    public int Previous_Compliant { get; set; }
    public int Current_Opportunity { get; set; }
    public int Current_Compliant { get; set; }
}
