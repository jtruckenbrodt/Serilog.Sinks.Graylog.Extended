namespace Serilog.Sinks.GraylogGelf
{
    /// <summary>
    /// Contains the result of checking a key-value pair against a <see cref="GraylogPropertyCatalogue"/>.
    /// </summary>
    public class GraylogPropertyCheckResult
    {
        /// <summary>
        /// The <see cref="GraylogPropertyCheckResultType"/> of the concrete check.
        /// </summary>
        public GraylogPropertyCheckResultType CheckResult = GraylogPropertyCheckResultType.Ok;
        /// <summary>
        /// The expected value type.
        /// </summary>
        public GraylogPropertyType ExpectedValueType = GraylogPropertyType.Undefined;
        /// <summary>
        /// The received value type.
        /// </summary>
        public GraylogPropertyType ReceivedValueType = GraylogPropertyType.Undefined;
        /// <summary>
        /// The received value.
        /// </summary>
        public object PropertyValue;
        /// <summary>
        /// The received property name/key.
        /// </summary>
        public string ReceivedPropertyName;
        /// <summary>
        /// The expected property name/key.
        /// </summary>
        public string ExpectedPropertyName;
    }
}