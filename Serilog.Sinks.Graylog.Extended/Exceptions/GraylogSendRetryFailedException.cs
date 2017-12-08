using System;

namespace Serilog.Sinks.Graylog.Extended.Exceptions
{
    /// <summary>
    /// Indicates, that a GELF message could not be send after certain amount of retries at all.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class GraylogSendRetryFailedException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogSendRetryFailedException"/> class.
        /// </summary>
        public GraylogSendRetryFailedException()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogSendRetryFailedException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        public GraylogSendRetryFailedException(string message) : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GraylogSendRetryFailedException"/> class.
        /// </summary>
        /// <param name="message">The message describing the error.</param>
        /// <param name="exception">The <see cref="Exception"/> which led to the current error.</param>
        public GraylogSendRetryFailedException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}