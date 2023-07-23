using Serilog;
using Serilog.Sinks.Graylog.Extended;

namespace ConsoleTestNoConfig
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Test Graylog Sink without file configuration");
            var graylogConfig = new GraylogSinkConfiguration
                                    {
                                        TransportType = TransportType.Tcp,
                                        Host = "192.168.2.87",
                                        Port = 12201,
                                        //UseSecureConnection = true,
                                        UseAsyncLogging = true
                                    };
            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Graylog(graylogConfig)
                .CreateLogger();

            //log.Information("Testing TLS secured {connectionType}", graylogConfig.GraylogTransportType);
            log.Information("Testing Graylog Information {connectionType}", graylogConfig.TransportType);
            Console.WriteLine("Press any key to exit");
        }
    }
}
