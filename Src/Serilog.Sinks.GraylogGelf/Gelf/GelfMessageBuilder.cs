using System;
using System.Collections.Generic;

namespace Serilog.Sinks.Graylog.Extended.Gelf
{
    /// <summary>
    /// Used to generate a single GELF message.
    /// </summary>
    public sealed class GelfMessageBuilder
    {
        private readonly Dictionary<string, object> _additionalFields = new Dictionary<string, object>();
        private readonly string _host;
        private readonly GelfLevel _level;
        private readonly string _message;
        private readonly DateTime _timestamp;

        public GelfMessageBuilder(string message, string host, DateTime timestamp, GelfLevel level)
        {
            _message = message;
            _host = host;
            _timestamp = timestamp;
            _level = level;
        }

        public GelfMessageBuilder SetAdditionalField(string key, object value)
        {
            if (value == null || string.IsNullOrEmpty(key)) return this;
            _additionalFields[key] = value;
            return this;
        }

        public GelfMessage ToMessage()
        {
            return new GelfMessage
            {
                Host = _host,
                FullMessage = _message,
                ShortMessage = _message.Length > 200 ? _message.Substring(0, 200) : _message,
                Level = _level,
                Timestamp = _timestamp,
                AdditionalFields = _additionalFields
            };
        }
    }
}