using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.DQC_Reporting;
public interface iOpportunityCompliant
{
    int Previous_Opportunity { get; set; }

    int Previous_Compliant { get; set; }

    int Current_Opportunity { get; set; }

    int Current_Compliant { get; set; }
}
