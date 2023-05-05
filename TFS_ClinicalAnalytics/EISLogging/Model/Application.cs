using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    /// <summary>
    /// Information about the UHG application that the event pertains to
    /// </summary>
    public class Application
    {
        public string name { get; set; }
        public string CI { get; set; }
        public string askId { get; set; }

        //public string environment { get; set; }
    }
}
