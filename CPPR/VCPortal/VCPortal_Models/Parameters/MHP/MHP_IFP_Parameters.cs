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
    private DateTime _startDate;
    private DateTime _endDate;
    private List<string> _Finc_Arng_Desc;
    private List<string> _Mkt_Seg_Rllp_Desc;
    private List<string> _LegalEntities;
    private List<string> _Mkt_Typ_Desc;

    [ValidateEachItem]
    public List<string> State { get => _state; set => _state = value; }
    [Required]
    public DateTime StartDate { get => _startDate; set => _startDate = value; }
    [Required]
    public DateTime EndDate { get => _endDate; set => _endDate = value; }
    [ValidateEachItem]
    public List<string> Finc_Arng_Desc { get => _Finc_Arng_Desc; set => _Finc_Arng_Desc = value; }
    [ValidateEachItem]
    public List<string> Mkt_Seg_Rllp_Desc { get => _Mkt_Seg_Rllp_Desc; set => _Mkt_Seg_Rllp_Desc = value; }
    [ValidateEachItem]
    public List<string> LegalEntities { get => _LegalEntities; set => _LegalEntities = value; }
    public List<string> Mkt_Typ_Desc { get => _Mkt_Typ_Desc; set => _Mkt_Typ_Desc = value; }
}
