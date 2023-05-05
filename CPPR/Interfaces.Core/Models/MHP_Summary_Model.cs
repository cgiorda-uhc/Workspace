using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Core.Models
{
    public class MHP_Summary_Model
    {

        public int ExcelRow
        {
            get  ; 
            set  ; 
        }

        public int? cnt_in_ip
        {
            get;
            set;
        }

        public int? cnt_on_ip
        {
            get;
            set;
        }

        public int? cnt_in_op
        {
            get;
            set;
        }

        public int? cnt_on_op
        {
            get;
            set;
        }


        public string StartDate
        {
            get;
            set;
        }

        public string EndDate
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }


        public string LegalEntity
        {
            get;
            set;
        }
    }
}
