using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    /// <summary>
    /// Information about the device and software that objected the event
    /// </summary>
    public class Device
    {
        public string vendor { get; set; }
        public string product { get; set; }
        public string version { get; set; }
        public string hostname { get; set; }
        public string ip4 { get; set; }

        public string CI { get; set; }
        public int pid { get; set; }
        public object proc { get; set; }
    }
}
