using System;
using Serilog.Sinks.Graylog.Extended.Gelf;

namespace Serilog.Sinks.Graylog.Extended.Transports
{
    /// <summary>
    /// Represents a single connection to Graylog.
    /// </summary>
    public interface ITransport : IDisposable
    {
        /// <summary>
        /// Sends the specified GELF message to Graylog.
        /// </summary>
        /// <param name="message">The <see cref="GelfMessage"/> to be send.</param>
        void Send(GelfMessage message);
    }
}