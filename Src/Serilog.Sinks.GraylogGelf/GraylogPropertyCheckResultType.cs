using System;

namespace Serilog.Sinks.GraylogGelf
{
    /// <summary>
    /// Indicates the result of a single property-value check against a <see cref="GraylogPropertyCatalogue"/>.
    /// </summary>
    [Flags]
    public enum GraylogPropertyCheckResultType
    {
        /// <summary>
        /// The property name matches exactly.
        /// The value type also matches.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// The property name is not identical but matches if compared case insensitive.
        /// The value type also matches
        /// </summary>
        ErrorNameSpelling = 1,
        /// <summary>
        /// The property name matches exactly.
        /// The value type does not match.
        /// </summary>
        ErrorValueTypeNotMatching = 2,
        /// <summary>
        /// The property name is not registered at all.
        /// The value type cannot be compared as it is not registered.
        /// </summary>
        ErrorNameNotRegistered = 4
    }
}