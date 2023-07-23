using System.Collections.Generic;

using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.GraylogGelf
{
    /// <summary>
    /// Class LoggerConfigurationGrayLogExtensions.
    /// </summary>
    public static class LoggerConfigurationGrayLogExtensions
    {
        /// <summary>
        /// Graylogs configuration auto setup. Function name must be equal section name
        /// </summary>
        /// <param name="loggerSinkConfiguration">The logger sink configuration.</param>
        /// <param name="hostnameOrAddress">The hostname or address.</param>
        /// <param name="port">The port.</param>
        /// <param name="transportType">Type of the transport.</param>
        /// <param name="additionalFields"></param>
        /// <param name="minimumLogEventLevel">The minimum log event level.</param>
        /// <param name="maxMessageSizeInUdp">The maximum message size in UDP.</param>
        /// <param name="propertyPrefix">The property prefix.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryIntervalMs">The retry interval ms.</param>
        /// <param name="useAsyncLogging">if set to <c>true</c> [use asynchronous logging].</param>
        /// <param name="useSecureConnectionInHttp">if set to <c>true</c> [use secure connection in HTTP].</param>
        /// <param name="minUdpMessageSizeForCompression">The minimum UDP message size for compression.</param>
        /// <returns>LoggerConfiguration.</returns>
        public static LoggerConfiguration GraylogGelf(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string hostnameOrAddress,
            int port,
            TransportType transportType,
            IDictionary<string, object>? additionalFields = null,
            LogEventLevel minimumLogEventLevel = LevelAlias.Minimum,
            int? maxMessageSizeInUdp = null,
            //string? usernameInHttp = null,
            //string? passwordInHttp = null,
            string propertyPrefix = "",
            int? retryCount = null,
            int? retryIntervalMs = null,
            bool useAsyncLogging = false,
            bool useSecureConnectionInHttp = false,
            int? minUdpMessageSizeForCompression = null
        )
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var options = new GraylogSinkConfiguration
                              {
                                  Host = hostnameOrAddress,
                                  Port = port,
                                  TransportType = transportType,
                                  MinimumLogEventLevel = minimumLogEventLevel,
                                  MinUdpMessageSizeForCompression = minUdpMessageSizeForCompression,
                                  MaxMessageSizeInUdp = maxMessageSizeInUdp,
                                  // UsernameInHttp = usernameInHttp,
                                  // PasswordInHttp = passwordInHttp,
                                  PropertyPrefix = propertyPrefix,
                                  RetryCount = retryCount,
                                  RetryIntervalMs = retryIntervalMs,
                                  UseAsyncLogging = useAsyncLogging,
                                  UseSecureConnection = useSecureConnectionInHttp,
                                  AdditionalFields = additionalFields,
                              };

            return loggerSinkConfiguration.Graylog(options, options.MinimumLogEventLevel);
        }
    }
}
