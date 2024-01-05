using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ProcCodeTrends;
public class CLM_OP_Report_Model
{

    public List<YearQuarter_Model> year_quarter_op { get; set; }

    public List<Events_Model> events_op { get; set; }
    public List<Unique_Individual_Model> unique_individual_op { get; set; }

    public List<Claims_Model> claims_op { get; set; }

    public List<Allowed_Model> allowed_op { get; set; }

    public List<Allowed_PMPM_Model> allowed_pmpm_op { get; set; }

    public List<Member_Month_Model> member_month_op { get; set; }


    public List<Utilization000_Model> utilization000_op { get; set; }

    public List<Unit_Cost1_Model> unit_cost1_op { get; set; }

    public List<Unit_Cost2_Model> unit_cost2_op { get; set; }

    public List<YearQuarter_Model> year_quarter_phys { get; set; }


    public List<Events_Model> events_phys { get; set; }
    public List<Unique_Individual_Model> unique_individual_phys { get; set; }

    public List<Claims_Model> claims_phys { get; set; }

    public List<Allowed_Model> allowed_phys { get; set; }

    public List<Allowed_PMPM_Model> allowed_pmpm_phys { get; set; }

    public List<Member_Month_Model> member_month_phys { get; set; }


    public List<Utilization000_Model> utilization000_phys { get; set; }

    public List<Unit_Cost1_Model> unit_cost1_phys { get; set; }

    public List<Unit_Cost2_Model> unit_cost2_phys { get; set; }


}
