using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Shared;

namespace VCPortal_Models.Parameters.MHP;
public class MHP_EI_Parameters
{

    private List<string> _state;
    private string _startDate;
    private string _endDate;
    private List<string> _Finc_Arng_Desc;
    private List<string> _Mkt_Seg_Rllp_Desc;
    private List<string> _LegalEntities;
    private List<string> _Mkt_Typ_Desc;
    private List<string> _Cust_Seg;

    [ValidateEachItem]
    public List<string> State { get => _state; set => _state = value; }
    [Required]
    public string StartDate { get => _startDate; set => _startDate = value; }
    [Required]
    public string EndDate { get => _endDate; set => _endDate = value; }
    [ValidateEachItem]
    public List<string> Finc_Arng_Desc { get => _Finc_Arng_Desc; set => _Finc_Arng_Desc = value; }
    [ValidateEachItem]
    public List<string> Mkt_Seg_Rllp_Desc { get => _Mkt_Seg_Rllp_Desc; set => _Mkt_Seg_Rllp_Desc = value; }
    [ValidateEachItem]
    public List<string> LegalEntities { get => _LegalEntities; set => _LegalEntities = value; }
    public List<string> Mkt_Typ_Desc { get => _Mkt_Typ_Desc; set => _Mkt_Typ_Desc = value; }


    public List<string> Cust_Seg { get => _Cust_Seg; set => _Cust_Seg = value; }

}
