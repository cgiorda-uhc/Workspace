using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASConnectionLibrary
{
    public class SASConnection_Model
    {

        public string SASHost { get; set; }
        public int SASPort { get; set; }
        public string SASClassIdentifier { get; set; }
        public string SASUserName { get; set; }
        public string SASPassword { get; set; }
        public string SASUserNameUnix { get; set; }
        public string SASPasswordUnix { get; set; }
        public string SASUserNameOracle { get; set; }
        public string SASPasswordOracle { get; set; }


    }
}
