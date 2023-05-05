using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models
{
    public class FileDateModel
    {
        public string name { get; set; }
        public DateTime file_date { get; set; }
        public int file_month { get; set; }
        public int file_year { get; set; }
    }
}
