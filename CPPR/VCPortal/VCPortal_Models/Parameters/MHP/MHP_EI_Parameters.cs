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

    private string _state;
    private string _startDate;
    private string _endDate;
    private string _Finc_Arng_Desc;
    private string _Mkt_Seg_Rllp_Desc;
    private List<string> _LegalEntities;
    private string _Mkt_Typ_Desc;
    private string _Cust_Seg;


    public string State { get => _state; set => _state = value; }
    [Required]
    public string StartDate { get => _startDate; set => _startDate = value; }
    [Required]
    public string EndDate { get => _endDate; set => _endDate = value; }

    public string Finc_Arng_Desc { get => _Finc_Arng_Desc; set => _Finc_Arng_Desc = value; }

    public string Mkt_Seg_Rllp_Desc { get => _Mkt_Seg_Rllp_Desc; set => _Mkt_Seg_Rllp_Desc = value; }

    public List<string> LegalEntities { get => _LegalEntities; set => _LegalEntities = value; }
    public string Mkt_Typ_Desc { get => _Mkt_Typ_Desc; set => _Mkt_Typ_Desc = value; }


    public string Cust_Seg { get => _Cust_Seg; set => _Cust_Seg = value; }

}
