using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Shared;

namespace VCPortal_Models.Parameters.MHP;
public class MHP_CS_Parameters
{
    private List<string> _state;
    private string _startDate;
    private string _endDate;
    private List<string> _CS_Tadm_Prdct_Map;
    private List<string> _GroupNumbers;

    [ValidateEachItem]
    public List<string> State { get => _state; set => _state = value; }
    [Required]
    public string StartDate { get => _startDate; set => _startDate = value; }
    [Required]
    public string EndDate { get => _endDate; set => _endDate = value; }
    [ValidateEachItem]
    public List<string> CS_Tadm_Prdct_Map { get => _CS_Tadm_Prdct_Map; set => _CS_Tadm_Prdct_Map = value; }

    public List<string> GroupNumbers { get => _GroupNumbers; set => _GroupNumbers = value; }

}
