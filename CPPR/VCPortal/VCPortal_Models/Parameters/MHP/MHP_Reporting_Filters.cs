using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Parameters.MHP;
public class MHP_Reporting_Filters
{
    private string _filter_Value;
    private string _filter_Type;
    private string _report_Type;


    [Required]
    public string Filter_Value { get => _filter_Value; set => _filter_Value = value; }
    [Required]
    public string Filter_Type { get => _filter_Type; set => _filter_Type = value; }
    [Required]
    public string Report_Type { get => _report_Type; set => _report_Type = value; }

}
