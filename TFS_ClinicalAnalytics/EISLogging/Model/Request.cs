using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    /// <summary>
    /// Information about events describing web requests
    /// </summary>
    public class Request
    {
        /// <summary>
        /// URL requested
        /// </summary>
        public string request { get; set; }
        /// <summary>
        /// The user-agent making the request
        /// </summary>
        public string userAgent { get; set; }
        /// <summary>
        /// Method used for request (GET, POST, etc)
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// Identifier to associate events in a single user session. Not an actual session cookie that could be replayed.
        /// </summary>
        public string cookies { get; set; }
        /// <summary>
        /// Contents of Optum_CID_Ext header
        /// </summary>
        public string Optum_CID_Ext { get; set; }
        /// <summary>
        /// Contents of Referer header
        /// </summary>
        public string referer { get; set; }
        ///// <summary>
        ///// Bytes transferred from subject(source) to object(destination) or device.
        ///// </summary>
        //public long? inb { get; set; }
        ///// <summary>
        ///// Bytes transferred from device or object(destination) to subject(source)
        ///// </summary>
        //public long? outb { get; set; }
    }
}
