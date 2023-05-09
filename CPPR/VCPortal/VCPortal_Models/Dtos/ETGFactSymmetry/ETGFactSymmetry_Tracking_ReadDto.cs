using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Dtos.ETGFactSymmetry;
public class ETGFactSymmetry_Tracking_ReadDto
{
    public long Tracker_Id { get; set; }

    public string ETG_Base_Class { get; set; }

    public string ETG_Description { get; set; }

    public string Premium_Specialty { get; set; }

    public bool? Has_Commercial { get; set; }
    public bool? Has_Commercial_Previous { get; set; }
    public bool? Has_Medicare { get; set; }
    public bool? Has_Medicare_Previous { get; set; }
    public bool? Has_Medicaid { get; set; }

    public bool? Has_Medicaid_Previous { get; set; }
    public bool? Has_RX { get; set; }

    public bool? Has_RX_Previous { get; set; }
    public bool? Has_NRX { get; set; }
    public bool? Has_NRX_Previous { get; set; }
    public string PC_Treatment_Indicator { get; set; }
    public string PC_Treatment_Indicator_Previous { get; set; }
    public string PC_Attribution { get; set; }
    public string PC_Attribution_Previous { get; set; }

    public string PC_Change_Comments { get; set; }

    public string PC_Change_Comments_Previous { get; set; }

    public string Patient_Centric_Mapping { get; set; }

    public string Patient_Centric_Mapping_Previous { get; set; }

    public string Patient_Centric_Change_Comments { get; set; }

    public string Patient_Centric_Change_Comments_Previous { get; set; }


    public string EC_Treatment_Indicator { get; set; }

    public string EC_Treatment_Indicator_Previous { get; set; }

    public string EC_Mapping { get; set; }

    public string EC_Mapping_Previous { get; set; }
    public string EC_Change_Comments { get; set; }

    public string EC_Change_Comments_Previous { get; set; }
    public DateTime? update_date { get; set; }

    public string username { get; set; }
}
