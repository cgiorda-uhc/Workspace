using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Shared;

namespace VCPortal_Models.Parameters.MHP;
public class MHP_IFP_Parameters
{
    private List<string> _state;
    private string _startDate;
    private string _endDate;
    private List<string> _ProductCodes;


    [ValidateEachItem]
    public List<string> State { get => _state; set => _state = value; }
    [Required]
    public string StartDate { get => _startDate; set => _startDate = value; }
    [Required]
    public string EndDate { get => _endDate; set => _endDate = value; }
    [ValidateEachItem]
    public List<string> ProductCodes { get => _ProductCodes; set => _ProductCodes = value; }

}
