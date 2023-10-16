using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VCPortal_Models.Parameters.EDCAdhoc;
public class EDC_Parameters
{
    public string MPIN { get; set; }
    public string TIN { get; set; }
    public string Provider_Id { get; set; }
    public string Date_of_Service_From { get; set; }
    public string Date_of_Service_To { get; set; }
    public string Procedure_Code { get; set; }
    public string Claim_Status{ get; set; }
    public string Service_Current_Indicator { get; set; }
    public string Par_NonPar { get; set; }
    public string Fund_Description { get; set; }
    public string Member_State { get; set; }
    public string Provider_State { get; set; }
    public string Covid_Indicator { get; set; }

}
