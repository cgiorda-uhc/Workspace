using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
public class NRX_Cost_UGAPModel
{

    public int ETG_BAS_CLSS_NBR { get; set; }

    public Int16 TRT_CD { get; set; }

    public int MEMBER_COUNT { get; set; }

    public int EPSD_COUNT { get; set; }


    public float ETGD_TOT_ALLW_AMT { get; set; }
    public float ETGD_RX_ALLW_AMT { get; set; }


    public float RX_RATE { get; set; }
}
