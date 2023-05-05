using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EISLogging
{
    public class LogData
    {
        /// <summary>
        /// Time the event was received by the application
        /// </summary>
        public string receivedTime { get; set; }

        /// <summary>
        /// Information about the UHG application that the event pertains to
        /// </summary>
        public Application application { get; set; }

        /// <summary>
        /// Information about the device and software that objected the event
        /// </summary>
        public Device device { get; set; }

        /// <summary>
        /// Type of log
        /// </summary>
        public LogClass logClass { get; set; }

        /// <summary>
        /// 0-7 logging levels EMERG, ALERT, CRIT, ERR, WARNING, NOTICE, INFO, DEBUG
        /// </summary>
        public severity? severity { get; set; }

        /// <summary>
        /// Event class IDs set by the application team. Separate from security event tags, described above
        /// </summary>
        public string eventClass { get; set; }

        /// <summary>
        /// Unique log object identifier from the source system. Often a string or number that increments with each even produced
        /// </summary>
        public string externalId { get; set; }

        /// <summary>
        /// Descriptive name of event type
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// String attribute that gives human-readable details about the event. Multi-line entries should use \\n as the newline separator
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// Information about the destination, target, or object of an event.
        /// </summary>
        public DestHost destHost { get; set; }

        /// <summary>
        /// Information about the destination, target, or object of an event
        /// </summary>
        public DestUser destUser { get; set; }
        /// <summary>
        /// Information about a host that is the source or subject of an event.
        /// </summary>
        public SourceHost sourceHost { get; set; }
        /// <summary>
        /// Information about a user that is the source or subject of an event.
        /// </summary>
        public SourceUser sourceUser { get; set; }
        /// <summary>
        /// Information about events describing web requests
        /// </summary>
        public Request request { get; set; }
        public string start { get; set; }

        /// <summary>
        /// End time of event. For example, end of a session
        /// </summary>
        public string end { get; set; }
        /// <summary>
        /// Action taken by device.
        /// </summary>
        public string act { get; set; }
        /// <summary>
        /// Success or failure, if event is a request.
        /// </summary>
        public outcome? outcome { get; set; }
        /// <summary>
        /// Reason for failure, if known. For example: bad credentials.
        /// </summary>
        public object reason { get; set; }
        /// <summary>
        /// Application level protocol, example values are: HTTP, HTTPS, SSHv2, Telnet, POP, IMAP, IMAPS, etc
        /// </summary>
        public object appProto { get; set; }
        /// <summary>
        /// Layer-4 protocol used. Most commonly TCP or UDP
        /// </summary>
        public string txProto { get; set; }
        /// <summary>
        /// Optional tags for the event.
        /// </summary>
        public string[] tags { get; set; }
        /// <summary>
        /// Optional fields for the event.
        /// </summary>
        public object additionalFields { get; set; }



    }
    public enum LogClass
    {
        SECURITY_SUCCESS,
        SECURITY_FAILURE,
        SECURITY_AUDIT,
        NONSECURITY,
        UNCATEGORIZED,
        E1,
        E2,
        E3,
        E4,
        E5,
        E6,
        E7,
        E8,
        E9,
        E10,
        E11
    }

    public enum severity
    {
        EMERG,
        ALERT,
        CRIT,
        ERR,
        WARNING,
        NOTICE,
        INFO,
        DEBUG,
        TRACE
    }

    public enum outcome
    {
        SUCCESS,
        FAILURE
    }
}
