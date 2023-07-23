using System;

namespace Serilog.Sinks.Graylog.Extended.Exceptions
{
    /// <summary>
    /// Indicates, that a transport connection (TCP or UDP) could not be established to Graylog.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class GraylogConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogConnectionException"/> class.
        /// </summary>
        public GraylogConnectionException()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public GraylogConnectionException(string message) : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        /// <param name="exception">The <see cref="Exception"/> which led to the current error.</param>
        public GraylogConnectionException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}