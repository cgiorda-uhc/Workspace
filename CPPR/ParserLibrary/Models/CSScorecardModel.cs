using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models
{
    public class CSScorecardModel : IFileDetailsModel
    {

        public string State { get; set; }
        public string Modality { get; set; }
        public int Phone { get; set; }
        public int Web { get; set; }
        public int Fax { get; set; }
        public double RequestsPer1000 { get; set; }
        public double ApprovalsPer1000 { get; set; }
        public int Approved { get; set; }
        public int Auto_Approved { get; set; }
        public int Denied { get; set; }
        public int Withdrawn { get; set; }
        public int Expired { get; set; }
        public int Others { get; set; }
        public int Routine_Cases { get; set; }

        public int Compliant_Routine_Cases { get; set; }


        public int Urgent_Cases { get; set; }


        public int Compliant_Urgent_Cases { get; set; }


        public bool is_ignored { get; set; }

        public string ignore_reason { get; set; }

        public string report_type { get; set; }
        public int file_month { get; set; }
        public int file_year { get; set; }
        public DateTime file_date { get; set; }
        public string sheet_name { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }



    }

}
