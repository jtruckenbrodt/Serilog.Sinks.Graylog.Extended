using System;
using Serilog.Events;
using Serilog.Sinks.Graylog.Extended.Gelf;

namespace Serilog.Sinks.Graylog.Extended
{
    internal static class Utils
    {
        internal static void SafeDispose(this IDisposable disposable)
        {
            if (disposable == null)
                return;
            try
            {
                disposable.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        internal static GelfLevel ToGelfLevel(this LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Verbose:
                    return GelfLevel.Notice;
                case LogEventLevel.Debug:
                    return GelfLevel.Debug;
                case LogEventLevel.Information:
                    return GelfLevel.Informational;
                case LogEventLevel.Warning:
                    return GelfLevel.Warning;
                case LogEventLevel.Error:
                    return GelfLevel.Error;
                case LogEventLevel.Fatal:
                    return GelfLevel.Critical;
                default:
                    return GelfLevel.Debug;
            }
        }
    }
}