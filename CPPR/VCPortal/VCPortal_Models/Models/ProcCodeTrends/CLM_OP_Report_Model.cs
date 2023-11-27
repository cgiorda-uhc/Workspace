using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ProcCodeTrends;
public class CLM_OP_Report_Model
{
    public List<Events_Model> events { get; set; }
    public List<Unique_Individual_Model> unique_individual { get; set; }

    public List<Claims_Model> claims { get; set; }

    public List<Allowed_Model> allowed { get; set; }

    public List<Allowed_PMPM_Model> allowed_pmpm { get; set; }

    public List<Member_Month_Model> member_month { get; set; }


    public List<Utilization000_Model> utilization000 { get; set; }

    public List<Unit_Cost1_Model> unit_cost1 { get; set; }

    public List<Unit_Cost2_Model> unit_cost2 { get; set; }
}
