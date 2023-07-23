using Microsoft.Extensions.Configuration;

using Serilog;

namespace ConsoleTestConfig
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Test Graylog Sink with file configuration");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //  Microsoft.Extensions.Configuration.FileExtensions
                .AddJsonFile("appsettings.json") // Microsoft.Extensions.Configuration.Json
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();
            //var graylogConfig = new GraylogSinkConfiguration
            //                        {
            //                            GraylogTransportType = GraylogTransportType.Udp,
            //                            Host = "192.168.2.87",
            //                            Port = 12201,
            //                            //UseSecureConnection = true,
            //                            UseAsyncLogging = true
            //                        };
            Log.Logger = new LoggerConfiguration()
                //.WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("team", "something")
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            ILogger logger = Log.ForContext<Program>();
            //log.Information("Testing TLS secured {connectionType}", graylogConfig.GraylogTransportType);
            //log.Information("Testing Graylog Information with config {connectionType}", graylogConfig.GraylogTransportType);
            logger.Debug("Test Graylog debug");
            logger.Information("Test Graylog information");
            logger.Error("Test Graylog error as text");
            logger.Error(new Exception("Test Graylog error with exception"), "Test Graylog error with exception");
            try
            {
                int test = int.Parse("test");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Test Graylog error with exception and call stack");
            }
            Console.WriteLine("Press any key to exit");
            //Console.ReadKey();
        }
    }
}
