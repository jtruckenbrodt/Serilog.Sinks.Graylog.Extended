using System.Collections.Generic;

using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Graylog.Extended
{
    /// <summary>
    /// Contains the settings for a single Graylog connection.
    /// </summary>
    public sealed class GraylogSinkConfiguration
    {
        /// <summary>
        /// Additional properties to be attached to every GELF message.
        /// </summary>
        public IDictionary<string, object> AdditionalFields { get; set; }

        /// <summary>
        /// The IP address or host name or complete URI of the Graylog target.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The optional <see cref="ILogEventSink"/> used to log internal failures (e.g. Console sink).
        /// </summary>
        public ILogEventSink LogErrorSink { get; set; }

        /// <summary>
        /// Defines maximum size in bytes a single UDP message can be before it is chunked.
        /// </summary>
        public int? MaxMessageSizeInUdp { get; set; }

        /// <summary>
        /// Gets or sets the minimum log event level.
        /// </summary>
        /// <value>The minimum log event level.</value>
        public LogEventLevel MinimumLogEventLevel { get; set; } // TODO: Implement

        /// <summary>
        /// Defines minimum size in bytes a single UDP message must be or exceed in order to use compression (GZIP).
        /// </summary>
        public int? MinUdpMessageSizeForCompression { get; set; }

        /// <summary>
        /// The port to be used when connecting to the Graylog target.
        /// This value is only used for UDP and TCP connections.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// An optional <see cref="GraylogPropertyCatalogue"/> used to check property value types.
        /// </summary>
        public GraylogPropertyCatalogue PropertyCatalog { get; set; }

        /// <summary>
        /// An optional prefix to be added to each user defined GELF property key.
        /// </summary>
        public string PropertyPrefix { get; set; }

        /// <summary>
        /// An optional value defining the number of tries to send again a message after it failed. 
        /// </summary>
        public int? RetryCount { get; set; }

        /// <summary>
        /// An optional value defining the milliseconds to wait before a message is send again after it failed. 
        /// </summary>
        public int? RetryIntervalMs { get; set; }

        /// <summary>
        /// Defines the <see cref="TransportType"/> used to connect to Graylog.
        /// </summary>
        public TransportType TransportType { get; set; }

        /// <summary>
        /// Defines whether to use the synchronious or asynchronious sink.
        /// </summary>
        public bool UseAsyncLogging { get; set; }

        /// <summary>
        /// Only used for TCP connections: defines whether a single GELF message should be send
        /// with a single NULL byte appended to signal end of message or not.
        /// </summary>
        public bool UseNullByteDelimiter { get; set; } = true;

        /// <summary>
        /// Only used for TCP connections: defines whether the TCP connection should be TLS secured or not.
        /// </summary>
        public bool UseSecureConnection { get; set; }
    }
}
