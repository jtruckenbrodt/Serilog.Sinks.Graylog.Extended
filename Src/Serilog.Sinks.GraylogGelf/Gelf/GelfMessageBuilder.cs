using System;
using System.Collections.Generic;

namespace Serilog.Sinks.GraylogGelf.Gelf
{
    /// <summary>
    /// Used to generate a single GELF message.
    /// </summary>
    public sealed class GelfMessageBuilder
    {
        private const int MaxMessageLength = 200;

        private readonly Dictionary<string, object> _additionalFields = new Dictionary<string, object>();
        private readonly string _host;
        private readonly GelfLevel _level;
        private readonly string _message;
        private readonly DateTime _timestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="GelfMessageBuilder"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="host">The host.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="level">The level.</param>
        public GelfMessageBuilder(string message, string host, DateTime timestamp, GelfLevel level)
        {
            _message = message;
            _host = host;
            _timestamp = timestamp;
            _level = level;
        }

        /// <summary>
        /// Sets the additional field.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>GelfMessageBuilder.</returns>
        public GelfMessageBuilder SetAdditionalField(string key, object? value)
        {
            if (value == null || string.IsNullOrEmpty(key)) return this;
            _additionalFields[key] = value;
            return this;
        }

        /// <summary>
        /// Converts to message.
        /// </summary>
        /// <returns>GelfMessage.</returns>
        public GelfMessage ToMessage()
        {
            return new GelfMessage
            {
                Host = _host,
                FullMessage = _message,
                ShortMessage = _message.Length > MaxMessageLength ? _message.Substring(0, MaxMessageLength) : _message,
                Level = _level,
                Timestamp = _timestamp,
                AdditionalFields = _additionalFields
            };
        }
    }
}