using System;
using System.Collections.Generic;

namespace Serilog.Sinks.Graylog.Extended.Gelf
{
    /// <summary>
    /// Represnets a single GELF message.
    /// </summary>
    public sealed class GelfMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="GelfMessage"/>.
        /// </summary>
        public GelfMessage()
        {
            AdditionalFields = new Dictionary<string, object>();
        }

        /// <summary>
        /// The default GELF version to set in each GELF message.
        /// Its value is <see cref="GelfConstants.DefaultGelfVersion"/>.
        /// </summary>
        public string Version => GelfConstants.DefaultGelfVersion;
        /// <summary>
        /// The name or ip address of the machine sending the GELF message.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// The short message to be send. This value must not be NULL or equal to <see cref="String.Empty"/>.
        /// Otherwise the GELF message will be dropped silently on Graylog side.
        /// </summary>
        public string ShortMessage { get; set; }
        /// <summary>
        /// The full message to be send. This value is optional.
        /// </summary>
        public string FullMessage { get; set; }
        /// <summary>
        /// The UTC time stamp when log event reflected by this GELF message was created.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The <see cref="GelfLevel"/> defining the priority of the GELF message.
        /// </summary>
        public GelfLevel Level { get; set; }
        /// <summary>
        /// Any optnal additional fields to be atteched to the GELF message.
        /// </summary>
        public Dictionary<string, object> AdditionalFields { get; set; }
    }
}