using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models
{
    public class NICEUHCWestEligibilityModel
    {

        public string Contract_Number { get; set; }
        public string PBP { get; set; }
        public string Company_State { get; set; }
        public int Member_Count { get; set; }

        public string report_type { get; set; }
        public int file_month { get; set; }
        public int file_year { get; set; }
        public DateTime file_date { get; set; }
        public string sheet_name { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
    }
}
