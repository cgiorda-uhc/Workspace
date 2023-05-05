using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class EvicoreScorecardModel
{

    public string Summary_of_Lob { get; set; }
    public string Header { get; set; }
    public int Total_Requests { get; set; }

    public double Per_Call { get; set; }
    public double Per_Website { get; set; }
    public double Per_Fax { get; set; }
    public double Approved { get; set; }
    public double Denied { get; set; }
    public double Withdrawn { get; set; }
    public double Admin_Expired { get; set; }
    public double Expired { get; set; }
    public double Pending { get; set; }
    public double Non_Cert { get; set; }
    public double Requests_per_thou { get; set; }
    public double Approval_per_thou { get; set; }

    public double MOD_3DI { get; set; }
    public double MOD_BONE_DENSITY { get; set; }
    public double MOD_CT_SCAN { get; set; }
    public double MOD_MRA { get; set; }
    public double MOD_MRI { get; set; }
    public double MOD_NOT_COVERED_PROCEDURE { get; set; }
    public double MOD_NUCLEAR_CARDIOLOGY { get; set; }
    public double MOD_NUCLEAR_MEDICINE { get; set; }
    public double MOD_PET_SCAN { get; set; }
    public double MOD_ULTRASOUND { get; set; }
    public double MOD_UNLISTED_PROCEDURE { get; set; }
    public double MOD_CARDIAC_CATHETERIZATION { get; set; }
    public double MOD_CARDIAC_CT_CCTA { get; set; }
    public double MOD_CARDIAC_IMPLANTABLE_DEVICES { get; set; }
    public double MOD_CARDIAC_MRI { get; set; }
    public double MOD_CARDIAC_PET { get; set; }
    public double MOD_ECHO_STRESS { get; set; }
    public double MOD_ECHO_STRESS_ADDON { get; set; }
    public double MOD_ECHOCARDIOGRAPHY { get; set; }
    public double MOD_ECHOCARDIOGRAPHY_ADDON { get; set; }
    public double MOD_NUCLEAR_STRESS { get; set; }
    public double MOD_CCCM_Misc_Cath_Codes { get; set; }


    public string report_type { get; set; }
    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }
}
