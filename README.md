# Serilog.Sinks.Graylog.Extended

## Introduction
This projects aims to make Graylog a comfortable logging sink for [Serilog](https://serilog.net)
The project and optional dependencies are all .NetStandard 2.0 libraries, so cross platform usage over .Net platforms is ensured.

## Supported Graylog transports
- UDP (with optional GZIP compression using [SharpCompress](https://github.com/adamhathcock/sharpcompress))
- TCP without encryption and optional Null-Byte delimiter
- TCP with TLS and optional Null-Byte delimiter
- HTTP withou encryption

All transports can be used synchoniously and asynchroniouly.

## Usage

In order to create a new sink one has to create an instance of class `GraylogSinkConfiguration` first. then you could use the fluent syntax and just call `Graylog()` extension method with the prepared config instance.

The following snippet will create a new sink:
```csharp
var graylogConfig = new GraylogSinkConfiguration
{
    TransportType = TransportType.TCP,
    Host = "example.graylog.local",
    Port = 12201,
    UseSecureConnection = true,
    UseAsyncLogging = true
};
using(var log = new LoggerConfiguration()
    .WriteTo.Graylog(graylogConfig)
    .CreateLogger())
{
    log.Information("testing TLS secured {connectionType}", graylogConfig.TransportType);
}
```     

Further examples can be found in the testing projects.

# Status [![Build status](https://travis-ci.org/jtruckenbrodt/Serilog.Sinks.Graylog.Extended.svg?branch=master)](https://travis-ci.org/jtruckenbrodt/Serilog.Sinks.Graylog.Extended) [![NuGet Version](https://img.shields.io/nuget/v/Serilog.Sinks.Graylog.Extended.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Graylog.Extended/)

## License
This project is licensed under [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) license.