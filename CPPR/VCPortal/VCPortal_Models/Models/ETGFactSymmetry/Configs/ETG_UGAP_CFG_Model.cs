using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Configs;
public class ETG_UGAP_CFG_Model
{
    public string MPC_NBR { get; set; }

    public int? ETG_BAS_CLSS_NBR { get; set; }

    public char? ALWAYS { get; set; }
    public char? ATTRIBUTED { get; set; }

    public string ERG_SPCL_CATGY_CD { get; set; }

    public Int16? TRT_CD { get; set; }

    public char? RX { get; set; }
    public char? NRX { get; set; }

    public string RISK_Model { get; set; }

    public Int16? LOW_MONTH { get; set; }

    public Int16? HIGH_MONTH { get; set; }

}
