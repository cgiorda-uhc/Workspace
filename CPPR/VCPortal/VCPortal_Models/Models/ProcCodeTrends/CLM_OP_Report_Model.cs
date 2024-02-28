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

    public List<Op_Claims_Model> claims_op { get; set; }

    public List<Allowed_Model> allowed_op { get; set; }

    public List<Allowed_PMPM_Model> allowed_pmpm_op { get; set; }

    public List<Member_Month_Model> member_month_op { get; set; }


    public List<Utilization000_Model> utilization000_op { get; set; }

    public List<Unit_Cost1_Model> unit_cost1_op { get; set; }

    public List<Unit_Cost2_Model> unit_cost2_op { get; set; }

    public List<Events000_Model> events000_op { get; set; }


    public List<YearQuarter_Model> year_quarter_phys { get; set; }


    public List<Events_Model> events_phys { get; set; }
    public List<Unique_Individual_Model> unique_individual_phys { get; set; }

    public List<Phys_Claims_Model> claims_phys { get; set; }

    public List<Allowed_Model> allowed_phys { get; set; }

    public List<Allowed_PMPM_Model> allowed_pmpm_phys { get; set; }

    public List<Member_Month_Model> member_month_phys { get; set; }


    public List<Utilization000_Model> utilization000_phys { get; set; }

    public List<Unit_Cost1_Model> unit_cost1_phys { get; set; }

    public List<Unit_Cost2_Model> unit_cost2_phys { get; set; }

    public List<Events000_Model> events000_phys { get; set; }


    public List<YearQuarter_Model> year_quarter_total { get; set; }
    public List<Events_Model> events_total { get; set; }
    public List<Unique_Individual_Model> unique_individual_total { get; set; }

    public List<Total_Claims_Model> claims_total { get; set; }

    public List<Allowed_Model> allowed_total { get; set; }

    public List<Allowed_PMPM_Model> allowed_pmpm_total { get; set; }

    public List<Member_Month_Model> member_month_total { get; set; }


    public List<Utilization000_Model> utilization000_total { get; set; }

    public List<Unit_Cost1_Model> unit_cost1_total { get; set; }

    public List<Unit_Cost2_Model> unit_cost2_total { get; set; }

    public List<Events000_Model> events000_total { get; set; }


    public string member_month_comment { get; set; }

    public string events_op_comment { get; set; }

    public string events_op_cost_comment { get; set; }

    public string unique_individual_op_comment { get; set; }

    public string claims_op_comment { get; set; }

    public string allowed_op_comment { get; set; }

    public string allowed_pmpm_op_comment { get; set; }


    public string utilization000_op_comment { get; set; }

    public string events000_op_comment { get; set; }


    public string unit_cost_op_comment { get; set; }


    public string year_quarter_phys_comment { get; set; }

    public string events_phys_comment { get; set; }

    public string events_phys_cost_comment { get; set; }

    public string unique_individual_phys_comment { get; set; }

    public string claims_phys_comment { get; set; }

    public string allowed_phys_comment { get; set; }

    public string allowed_pmpm_phys_comment { get; set; }

    public string utilization000_phys_comment { get; set; }

    public string events000_phys_comment { get; set; }

    public string unit_cost_phys_comment { get; set; }


    public string year_quarter_total_comment { get; set; }

    public string events_total_comment { get; set; }

    public string events_total_cost_comment { get; set; }

    public string unique_individual_total_comment { get; set; }

    public string claims_total_comment { get; set; }

    public string allowed_total_comment { get; set; }

    public string allowed_pmpm_total_comment { get; set; }

    public string utilization000_total_comment { get; set; }

    public string unit_cost_total_comment { get; set; }

    public string events000_total_comment { get; set; }




}
