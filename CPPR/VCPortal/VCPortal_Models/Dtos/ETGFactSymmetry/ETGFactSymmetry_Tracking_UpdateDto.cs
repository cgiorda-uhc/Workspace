using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Dtos.ETGFactSymmetry;
public class ETGFactSymmetry_Tracking_UpdateDto
{

    public long ETG_Fact_Symmetry_id { get; set; }


    public bool? Is_Mapped { get; set; }

    public bool? Has_Commercial { get; set; }
    public bool? Has_Medicare { get; set; }
    public bool? Has_Medicaid { get; set; }


    public bool? Has_RX { get; set; }
    public bool? Has_NRX { get; set; }

    public string PC_Treatment_Indicator { get; set; }

    public string PC_Attribution { get; set; }


    public string PC_Change_Comments { get; set; }

    public string Patient_Centric_Mapping { get; set; }

    public string Patient_Centric_Change_Comments { get; set; }


    public string EC_Treatment_Indicator { get; set; }

    public string EC_Mapping { get; set; }

    public string EC_Change_Comments { get; set; }


    public DateTime? update_date { get; set; }

    public string username { get; set; }


}
