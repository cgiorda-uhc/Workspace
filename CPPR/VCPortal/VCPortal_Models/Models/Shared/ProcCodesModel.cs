using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.Shared;
public class ProcCodesModel
{
    public string Proc_Cd { get; set; }
    public string Proc_Desc { get; set; }
    public string Proc_Cd_Type { get; set; }

    public DateTime? Proc_Cd_Date { get; set; }

    public string Proc_Cd_Full
    {
        get { return Proc_Cd + " - " + Proc_Desc; }
    }

}
