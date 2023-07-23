using System;

namespace Serilog.Sinks.GraylogGelf.Exceptions
{
    /// <summary>
    /// Indicates, that a HTTP transport connection could not be established to Graylog.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class GraylogHttpTransportException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogHttpTransportException"/> class.
        /// </summary>
        public GraylogHttpTransportException()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogHttpTransportException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public GraylogHttpTransportException(string message) : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogHttpTransportException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        /// <param name="exception">The <see cref="Exception"/> which led to the current error.</param>
        public GraylogHttpTransportException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}