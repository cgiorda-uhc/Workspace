using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EISLogging
{
    /// <summary>
    /// Information about a user that is the source or subject of an event
    /// </summary>
    public class SourceUser
    {
        /// <summary>
        /// User id. May also be system or service accounts
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// Name of user
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// guest, user, privileged user, administrator, system, or root enum
        /// </summary>
        public string priv { get; set; }
        /// <summary>
        /// User role
        /// </summary>
        public string role { get; set; }
    }
}
