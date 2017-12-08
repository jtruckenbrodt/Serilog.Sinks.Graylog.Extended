using System;
using System.Text;

namespace Serilog.Sinks.Graylog.Extended.Gelf
{
    internal sealed class GelfMessageSerializer : IGelfMessageSerializer
    {
        private const string InvalidIdKey = "_id";
        
        public string SerializeToString(GelfMessage message)
        {
            var duration = message.Timestamp.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var result = new JsonObject
            {
                {"version", message.Version},
                {"host", message.Host},
                {"short_message", message.ShortMessage},
                {"full_message", message.FullMessage},
                {"timestamp", Math.Round(duration.TotalSeconds, 3, MidpointRounding.AwayFromZero)},
                {"level", (int)message.Level}
            };
            foreach (var additionalField in message.AdditionalFields)
            {
                var key = additionalField.Key;
                if (key.Equals(InvalidIdKey))
                {
                    Log.Error($"Additional field name '{key}' is not allowed.");
                    continue;
                }
                var value = additionalField.Value is Enum ? additionalField.Value.ToString() : additionalField.Value;
                result.Add(key.StartsWith("_") ? key : "_" + key, value);
            }

            var jsonMsg = result.ToString();
            return result.ToString();
        }

        public byte[] SerializeToStringBytes(GelfMessage message)
        {
            return Encoding.UTF8.GetBytes(SerializeToString(message));
        }

    }
}