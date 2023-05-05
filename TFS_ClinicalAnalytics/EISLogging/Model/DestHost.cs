using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    /// <summary>
    /// Information about the destination, target, or object of an event.
    /// </summary>
    public class DestHost
    {
        /// <summary>
        /// Hostname, preferably FQDN
        /// </summary>
        public object hostname { get; set; }

        /// <summary>
        /// The domain portion of FQDN
        /// </summary>
        public string dnsDomain { get; set; }

        /// <summary>
        /// Windows domain
        /// </summary>
        public string ntDomain { get; set; }

        public string ip4 { get; set; }

        public string ip6 { get; set; }

        /// <summary>
        /// Process name
        /// </summary>
        public string proc { get; set; }

        /// <summary>
        /// Process id
        /// </summary>
        public int? pid { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int? port { get; set; }

        /// <summary>
        /// MAC address
        /// </summary>
        public string mac { get; set; }

        public long[] fwdAddr { get; set; }
        public int[] fwdPort { get; set; }
    }
}
