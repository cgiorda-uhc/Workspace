using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.TAT;
public class TAT_Model
{
    public string lob { get; set; }
    public string rpt_Modality { get; set; }

    public float? pct { get; set; }

    public float? SLA { get; set; }

    public float? Penalty_SLA { get; set; }

    public string section { get; set; }

}
