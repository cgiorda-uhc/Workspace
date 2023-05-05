using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    /// <summary>
    /// Information about the destination, target, or object of an event
    /// </summary>
    public class DestUser
    {
        /// <summary>
        /// User id. May also be system or service accounts
        /// </summary>
        public string uid { get; set; }

        //Backend ID or UUID of which represents the uid
        public string uuid { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        public object name { get; set; }

        /// <summary>
        /// guest, user, privileged user, administrator, system, or root
        /// </summary>
        public object priv { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        public string role { get; set; }


        /// <summary>
        /// CHRIS ADDED
        /// </summary>

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string tokenIssuer { get; set; }

        public long tokenCreated { get; set; }

        public long tokenExpires { get; set; }

        public string tokenHash { get; set; }




    }
}
