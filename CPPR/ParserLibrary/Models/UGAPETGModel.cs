using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models
{
    public class UGAPETGModel
    {
        public string MPC_NBR { get; set; }

        public int? ETG_BAS_CLSS_NBR { get; set; }

        public char? ALWAYS { get; set; }
        public char? ATTRIBUTED { get; set; }

        public string ERG_SPCL_CATGY_CD { get; set; }

        public Int16? TRT_CD { get; set; }

        public char? RX { get; set; }
        public char? NRX { get; set; }

        public string RISK_MDL { get; set; }

    }

    public class UGAPMPCNBRModel
    {
        public string MPC_NBR { get; set; }

        public int ETG_BAS_CLSS_NBR { get; set; }

    }

}
