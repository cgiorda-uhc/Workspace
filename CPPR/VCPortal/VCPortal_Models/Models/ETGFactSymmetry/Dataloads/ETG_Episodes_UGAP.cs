﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
public class ETG_Episodes_UGAP
{
    public string EPSD_NBR { get; set; }
    public float TOT_ALLW_AMT { get; set; }
    public char SVRTY { get; set; }
    public string ETG_BAS_CLSS_NBR { get; set; }
    public Int16 ETG_TX_IND { get; set; }
    public int PROV_MPIN { get; set; }
    public float TOT_NP_ALLW_AMT { get; set; }
    public Int16 LOB_ID { get; set; }

    public float PD_Version { get; set; }

}
