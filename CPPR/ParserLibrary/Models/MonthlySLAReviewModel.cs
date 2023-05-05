using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class MonthlySLAReviewModel
{
    public string Bucket { get; set; }
    public string Hit { get; set; }
    public string Miss { get; set; }
    public string LOB { get; set; }
    public string Modality { get; set; }
    public float Percentage { get; set; }

    public float SLA { get; set; }

    public float Penalty_SLA { get; set; }
}
