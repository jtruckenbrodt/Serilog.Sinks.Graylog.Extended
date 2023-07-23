namespace Serilog.Sinks.GraylogGelf
{
    /// <summary>
    /// Defines constants and default values used throughout this project.
    /// </summary>
    public static class GraylogConstants
    {
        /// <summary>
        /// The retry interval in milliseconds to wait before trying again to send a GELF message.
        /// </summary>
        public static readonly int DefaultRetryIntervalInMs = 150;
        /// <summary>
        /// The amount of tries to send a GELF message before giving up and silently dropping it.
        /// </summary>
        public static readonly int DefaultRetryCount = 5;
    }
}