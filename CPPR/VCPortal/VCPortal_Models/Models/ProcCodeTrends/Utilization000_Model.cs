﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ProcCodeTrends;
public class Utilization000_Model
{
    public string px { get; set; }
    public string px_desc { get; set; }

    public float? Y1Q1_util000 { get; set; }

    public float? Y1Q2_util000 { get; set; }

    public float? Y1Q3_util000 { get; set; }

    public float? Y1Q4_util000 { get; set; }



    public float? Y2Q1_util000 { get; set; }


    public float? Y2Q2_util000 { get; set; }


    public float? Y2Q3_util000 { get; set; }


    public float? Y2Q4_util000 { get; set; }

    public double? Y1Q1_Y2Q1_trend { get; set; }

    public double? Y1Q2_Y2Q2_trend { get; set; }

    public double? Y1Q3_Y2Q3_trend { get; set; }

    public double? Y1Q4_Y2Q4_trend { get; set; }

}
