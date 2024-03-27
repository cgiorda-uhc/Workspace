using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.TAT;
public class TAT_Summary_Model
{
    public string rtype { get; set; }
    public string lob { get; set; }
    public string Modality { get; set; }

    public int metric_id { get; set; }

    public string metric_desc { get; set; }

    public int YTD_Penalty { get; set; }
    public int Jan { get; set; }
    public int Feb { get; set; }
    public int Mar { get; set; }
    public int Apr { get; set; }
    public int May { get; set; }
    public int Jun { get; set; }
    public int Jul { get; set; }
    public int Aug   { get; set; }
    public int Sep   { get; set; }
    public int Oct  { get; set; }
    public int Nov { get; set; }
    public int Dec { get; set; }




}
