namespace Serilog.Sinks.Graylog.Extended
{
    /// <summary>
    /// Defines the supported transports used to connect to Graylog.
    /// </summary>
    public enum TransportType
    {
        /// <summary>
        /// Connect to Graylog via HTTP protocol (not HTTPS).
        /// </summary>
        Http,

        /// <summary>
        /// Connect to Graylog via raw TCP (socket) protocol, either TLS secured or without encryption.
        /// </summary>
        Tcp,

        /// <summary>
        /// Connect to Graylog via raw UDP protocol.
        /// </summary>
        Udp
    }
}
