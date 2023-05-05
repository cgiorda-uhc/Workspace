using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    /// <summary>
    /// Information about a host that is the source or subject of an event.
    /// </summary>
    public class SourceHost
    {
        /// <summary>
        /// Hostname, preferably FQDN
        /// </summary>
        public string hostname { get; set; }
        /// <summary>
        /// The domain portion of FQDN
        /// </summary>
        public string dnsDomain { get; set; }
        /// <summary>
        /// Windows domain
        /// </summary>
        public string ntDomain { get; set; }
        /// <summary>
        /// IPv4 address
        /// </summary>
        public string ip4 { get; set; }
        /// <summary>
        /// IPv6 address
        /// </summary>
        public string ip6 { get; set; }
        /// <summary>
        /// Process name
        /// </summary>
        public string proc { get; set; }
        /// <summary>
        /// Process id
        /// </summary>
        public int pid { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// MAC address
        /// </summary>
        public string mac { get; set; }
        /// <summary>
        /// IP address after translation by a load balancer, firewall, or proxy. X-Forwarded-For or Forwarded headers contain this information.
        /// </summary>
        public long[] fwdAddr { get; set; }
        /// <summary>
        /// Port after translation by a load balancer, firewall, or proxy. X-Forwarded-Proto or Forwarded headers contain this information.
        /// </summary>
        public int[] fwdPort { get; set; }
       
    }
}
