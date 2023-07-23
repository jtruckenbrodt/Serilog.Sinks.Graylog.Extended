using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.GraylogGelf
{    
    /// <summary>
    /// Provides fluent syntax for creating a Graylog sink.
    /// </summary>
    public static class GraylogSinkFluentExtension
    {
        /// <summary>
        /// Appends the Graylog sink defined through the provided <see cref="GraylogSinkConfiguration"/> settings to a
        /// <see cref="LoggerSinkConfiguration"/> instance.
        /// </summary>
        /// <param name="loggerSinkConfig">A <see cref="LoggerSinkConfiguration"/> to which a new Graylog sink is to be attached.</param>
        /// <param name="config">Contains the settings used to configure the new Graylog sink.</param>
        /// <param name="minLogLevel">Defines the minimum log level to be logged into the new Graylog sink.
        /// The default value is <see cref="LogEventLevel.Information"/>.</param>
        /// <returns>A <see cref="LoggerConfiguration"/> item to be used in the next fluent call, contianing the new Graylog sink.</returns>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfig,
            GraylogSinkConfiguration config, LogEventLevel minLogLevel = LogEventLevel.Information)
        {
            var sink = config.UseAsyncLogging
                ? (GraylogSinkBase)new GraylogSinkAsync(config)
                : new GraylogSink(config);
            return loggerSinkConfig.Sink(sink, minLogLevel);
        }
    }
}