using BlazorLogTest.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

namespace BlazorLogTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            AddServices(builder.Services);

            builder.Host.UseSerilog((hostContext, services, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
                    //loggerConfiguration.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3} {ThreadId:d2}] {Message:lj} <{SourceContext}>{NewLine}{Exception}");
                    //loggerConfiguration.Enrich.FromLogContext();
                    //loggerConfiguration.Enrich.WithThreadId();
                });
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }

        private static void AddServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
        }
    }
}