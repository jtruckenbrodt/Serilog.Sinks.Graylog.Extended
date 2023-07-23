namespace Serilog.Sinks.Graylog.Extended.Gelf
{
    /// <summary>
    /// Contains the supported GELF log levels
    /// </summary>
    public enum GelfLevel
    {
        /// <summary>
        /// Defines the log level of highest priority, everything burns and no one can stop it.
        /// </summary>
        Emergency = 0,
        /// <summary>
        /// Defines the log level of second highest priority, it is amost too late.
        /// </summary>
        Alert = 1,
        /// <summary>
        /// Defines the log level of third highest priority, we still have time and can recover.
        /// </summary>
        Critical = 2,
        /// <summary>
        /// Defines the log level of fourth highest priority, don't panic, it is nothing serious.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Defines the log level of fifth highest priority, there is nothing, really.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Defines the log level of sixth highest priority, we don't care at all.
        /// </summary>
        Notice = 5,
        /// <summary>
        /// Defines the log level of second lowest priority, just for you info.
        /// </summary>
        Informational = 6,
        /// <summary>
        /// Defines the log level of lowest priority, this is just geek info.
        /// </summary>
        Debug = 7
    }
}