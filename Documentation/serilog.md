# How To use Serilog
## Overview
Serilog is a diagnostic logging library for .NET applications. It is easy to set up, has a clean API, and runs on all recent .NET platforms. While it's useful even in the simplest applications, Serilog's support for structured logging shines when instrumenting complex, distributed, and asynchronous applications and systems.

## Configuration
Serilog is immutable by default. This mean that you cannot change settings after initialization. From other side you cannot initialize serilog immediately after program beginning as some services are not initialized (like configuration service). If you want to have logging immediately after program beginning you need to use two steps initialization:
1. LoggerConfiguration with CreateBootstrapLoggger() and source code initialization like:
```
var minWarningLogger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .CreateBootstrapLoggger()
```
> **Note**: CreateBootstrapLoggger could be found into `Serilog.Extensions.Hosting` package

2. LoggerConfiguration with CreateLoggger() where all parameters could be read from current configuration:

```
var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //  Microsoft.Extensions.Configuration.FileExtensions
                .AddJsonFile("appsettings.json") // Microsoft.Extensions.Configuration.Json
                .Build();

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
```

> **Note 1**: Configuration extension method could be found into  package. [`Serilog.Settings.Configuration`](https://github.com/serilog/serilog-settings-configuration). You can manage reading configuration over serilog too, see link.

> **Note 2**: [Serilog Example application](https://github.com/serilog/serilog/wiki/Getting-Started#example-application) used generic hosting model. For .NET hosting model
```csharp
var builder = WebApplication.CreateBuilder(args);
```
or
```csharp
var builder = await Host.CreateDefaultBuilder(args)
```

```csharp
Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
//Build
var app = builder.Build();

//...
```

Pay attention that you can use serilog as single standalone debugger (_UseSerilog_) or as additional logging provider (_AddSerilog_) to microsoft logging.
## Sinks
Sinks are how you direct where you want your logs sent. The most popular of the standard sinks are the File and Console targets.

## How to enable Serilog’s own internal debug logging
If you are having any problems with Serilog, you can subscribe to it’s internal events and write them to your debug window or a console.

```
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
Serilog.Debugging.SelfLog.Enable(Console.Error);
```

Please note that the internal logging will not write to any user-defined sinks.
## How ASP .NET Core Logging working

As long as you call `Host.CreateDefaultBuilder` system will [initialize Microsoft logger factory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder) with default logging where you can log to the console, debug, and event source output. 

## TIP: Log extra fields on exceptions!
One of the best uses of structured logging is on exceptions. Trying to figure out why an exception happened is infinitely easier if you know more details about who the user was, input parameters, etc.
    
```
try
    {
        //do something
    }
    catch (Exception ex)
    {
        log.Error("Error trying to do something", new { clientid = 54732, user = "matt" }, ex);
    }
```

## Useful links
[Serilog](https://github.com/serilog/serilog)
[Provided Sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks)
[Developing a sink](https://github.com/serilog/serilog/wiki/Developing-a-sink)
[Bootstrap logging with Serilog](https://nblumhardt.com/2020/10/bootstrap-logger/)
[Serilog blog](https://nblumhardt.com/)
[Logging in .NET Core and ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-7.0)
[ASP.NET Core Blazor logging](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/logging?view=aspnetcore-7.0)
[Serilog Logging in ASP.NET Core](https://referbruv.com/blog/how-to-serilog-logging-in-asp-net-core/)

https://onloupe.com/blog/how-to-config-logger-net6-startup/