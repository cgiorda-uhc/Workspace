using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.DQC_Reporting;
public class Spec_Minor_Market_PEG_Subcat_Model : iOpportunityCompliant
{
    public string Specialty { get; set; }

    public string Minor_Market { get; set; }

    public string PEG_with_Subcategory { get; set; }

    public int Previous_Opportunity { get; set; }
    public int Previous_Compliant { get; set; }
    public int Current_Opportunity { get; set; }
    public int Current_Compliant { get; set; }
}
