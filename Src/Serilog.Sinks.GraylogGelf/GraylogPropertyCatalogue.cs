using System;
using System.Collections.Generic;

using Serilog.Events;

namespace Serilog.Sinks.GraylogGelf
{
    /// <summary>
    /// A <see cref="SortedList{TKey,TValue}"/> containing keys and data types to be used when checking for mismatching properties.
    /// As soon as a property has been send the first time to the current Graylog index, it is assocated with the send data vale type.
    /// If a second property arrives at Graylog having a different type, it will be dropped (the complete GELF message gets dropped).
    /// In case of property missmatch, a log entry is written in Graylog. 
    /// </summary>
    public class GraylogPropertyCatalogue : SortedList<string, GraylogPropertyType>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GraylogPropertyCatalogue"/> using an
        /// <see cref="StringComparer.InvariantCultureIgnoreCase"/> comparer to compare the keys. 
        /// </summary>
        public GraylogPropertyCatalogue() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GraylogPropertyCatalogue"/> using an
        /// <see cref="StringComparer.InvariantCultureIgnoreCase"/> comparer to compare the keys. 
        /// </summary>
        /// <param name="supportedProperties">The inital keys and their assigned data value types.</param>
        public GraylogPropertyCatalogue(IDictionary<string, GraylogPropertyType> supportedProperties) :
            base(supportedProperties, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Checks a <see cref="LogEventPropertyValue"/> to have the correct data value type.
        /// </summary>
        /// <param name="receivedPropertyName">The key to use when writing the data value as GELF property.</param>
        /// <param name="property">The <see cref="LogEventPropertyValue"/> containing the data value.</param>
        /// <returns>A <see cref="GraylogPropertyCheckResult"/> with the check result.</returns>
        public GraylogPropertyCheckResult CheckProperty(string receivedPropertyName, LogEventPropertyValue property)
        {
            var res = new GraylogPropertyCheckResult { ReceivedPropertyName = receivedPropertyName };
            var pos = IndexOfKey(receivedPropertyName);
            if (pos == -1)
            {
                res.CheckResult |= GraylogPropertyCheckResultType.ErrorNameNotRegistered;
                return res;
            }
            res.ExpectedPropertyName = Keys[pos];
            if (!receivedPropertyName.Equals(res.ExpectedPropertyName))
            {
                res.CheckResult |= GraylogPropertyCheckResultType.ErrorNameSpelling;
            }
            res.PropertyValue = GetValue(property, out var receivedType);
            var expectedType = Values[pos];
            if (expectedType == GraylogPropertyType.Object && receivedType != GraylogPropertyType.String || !expectedType.HasFlag(receivedType))
            {
                res.CheckResult |= GraylogPropertyCheckResultType.ErrorValueTypeNotMatching;
                res.ExpectedValueType = expectedType;
                res.ReceivedValueType = receivedType;
            }
            return res;
        }

        private static object GetValue(LogEventPropertyValue value, out GraylogPropertyType propType)
        {
            // as GELF currently only allows string or numeric values, we'll have to convert if type is not matching
            if (value == null)
            {
                propType = GraylogPropertyType.String;
                return string.Empty;
            }
            if (!(value is ScalarValue scalar))
            {
                propType = GraylogPropertyType.String;
                return value.ToString();
            }
            if (scalar.Value == null)
            {
                propType = GraylogPropertyType.String;
                return value.ToString();
            }
            propType = GetPropertyType(scalar.Value);
            return propType == GraylogPropertyType.Numeric
                ? scalar.Value
                : scalar.Value.ToString();
        }

        private static GraylogPropertyType GetPropertyType(object value)
        {
            switch (value)
            {
                case Enum _:
                    return GraylogPropertyType.Enum;
                case int _:
                case long _:
                case decimal _:
                case double _:
                case float _:
                case short _:
                case sbyte _:
                case uint _:
                case ulong _:
                case ushort _:
                case byte _:
                    return GraylogPropertyType.Numeric;
                case bool _:
                    return GraylogPropertyType.Boolean;
            }
            return GraylogPropertyType.String;
        }
    }
}
