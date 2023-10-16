using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.EDCAdhoc;
public class EDC_TOTAL
{
    public int MPIN { get; set; }
    public int DOS_YEAR { get; set; }
    public int DOS_MNTH { get; set; }
    public string BILLED_PROCEDURE_CODE { get; set; }

    public string REPROCESSED_PROCEDURE_CODE { get; set; }
    public int TotalCount { get; set; }

}
