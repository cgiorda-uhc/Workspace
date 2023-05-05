using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    public class SecureLog
    {
        public string receivedTime { get; set; }
        public string logClass { get; set; }
        public string applicationName { get; set; }
        public string CI { get; set; }
        public string askId { get; set; }
        public string ip4 { get; set; }
        public string vendor { get; set; }
        public string product { get; set; }

        public string hostname { get; set; }
        public string msg { get; set; }

    }
}
