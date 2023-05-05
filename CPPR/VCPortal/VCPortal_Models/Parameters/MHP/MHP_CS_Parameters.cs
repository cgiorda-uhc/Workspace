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
    private DateTime _startDate;
    private DateTime _endDate;
    private List<string> _CS_Tadm_Prdct_Map;

    [ValidateEachItem]
    public List<string> State { get => _state; set => _state = value; }
    [Required]
    public DateTime StartDate { get => _startDate; set => _startDate = value; }
    [Required]
    public DateTime EndDate { get => _endDate; set => _endDate = value; }
    [ValidateEachItem]
    public List<string> CS_Tadm_Prdct_Map { get => _CS_Tadm_Prdct_Map; set => _CS_Tadm_Prdct_Map = value; }

}
