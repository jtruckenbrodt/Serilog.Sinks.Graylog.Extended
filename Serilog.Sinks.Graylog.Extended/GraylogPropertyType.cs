using System;

namespace Serilog.Sinks.Graylog.Extended
{
    /// <summary>
    /// Contains all supported GELF value types.
    /// </summary>
    [Flags]
    public enum GraylogPropertyType
    {
        /// <summary>
        /// The value type is not defined and thus not supported.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The value is numeric (integer or fractional).
        /// </summary>
        Numeric = 1,
        /// <summary>
        /// The value is of type <see cref="Enum"/>.
        /// <see cref="Enum"/> values will be converted to strings before being send to Graylog.
        /// </summary>
        Enum = 2,
        /// <summary>
        /// The value is of type <see cref="Boolean"/>.
        /// <see cref="Boolean"/> values will be converted to numbers (1 == true - 0 == false) before being send to Graylog.
        /// </summary>
        Boolean = 4,
        /// <summary>
        /// The value is of type <see cref="Object"/> and not of any simple type defined else.
        /// <see cref="Object"/> values will serialized to JSON items before being send to Graylog.
        /// </summary>
        Object = 8,
        /// <summary>
        /// The value is of type <see cref="String"/>.
        /// </summary>
        String = 16
    }
}