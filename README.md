# Serilog.Sinks.GraylogGelf

## Introduction
This projects aims to make Graylog a comfortable logging sink for [Serilog](https://serilog.net)
The project and optional dependencies are all .NetStandard 2.0 libraries, so cross platform usage over .Net platforms is ensured.
More information about [Graylog](Documentation/graylog.md) or [Serilog](Documentation/serilog.md).

## Features
- Support for Graylog Extended Log Format (GELF) 1.1
- Suport configuration via code or appsettings.json
- Support user defined fiedls

## Supported Graylog transports
- UDP (with optional GZIP compression using [SharpCompress](https://github.com/adamhathcock/sharpcompress))
- TCP without encryption and optional Null-Byte delimiter
- TCP with TLS and optional Null-Byte delimiter
- HTTP without encryption

All transports can be used synchoniously and asynchroniouly.

## Usage

Samples are only for your understandig of base usase you must not use exact same code into your application.

### Configuration via code

In order to create a new sink one has to create an instance of class `GraylogSinkConfiguration` first. 
Then you could use the fluent syntax and just call `Graylog()` extension method with the prepared config instance.

The following snippet will create a new sink:
```csharp
var graylogConfig = new GraylogSinkConfiguration
{
    TransportType = TransportType.Tcp,
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

### Configuration via appsettings.json
If you need variable configuration settings, you could use the `Serilog.Settings.Configuration` package 
and read confuguration from `appsettings.json` file. As appsettings.json is a JSON external file, you could modify it in any time.
You can change values into pipeline or after deplotment without recompiling your application.

Sample for .NET Core console application:

```csharp
var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) //  Microsoft.Extensions.Configuration.FileExtensions
                .AddJsonFile("appsettings.json") // Microsoft.Extensions.Configuration.Json
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();
Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger()
```  

appsettings.json:

```json
{
  "Serilog": {
	"Using": [ "Serilog.Sinks.GraylogGelf" ],
	"MinimumLevel": "Debug",
	"WriteTo": [
	  {
		"Name": "Graylog",
		"Args": {
		  "hostnameOrAddress": "example.graylog.local",
		  "port": 12201,
		  "transportType": "Tcp",
		  "useAsyncLogging": true
		}
	  }
	]
  }
}
```
How to add user defined fields for every log item:

```json
{
  "Serilog": {
	"Using": [ "Serilog.Sinks.GraylogGelf" ],
	"MinimumLevel": "Debug",
	"WriteTo": [
	  {
		"Name": "Graylog",
		"Args": {
		  "hostnameOrAddress": "example.graylog.local",
		  "port": 12201,
		  "transportType": "Tcp",
		  "useAsyncLogging": true,
		  "additionalFields": {
			"application": "MyApplication",
			"environment": "Production"
		  }
		}
	  }
	]
  }
}
```

Further examples can be found in the sample projects.

## Possible json settings

For compatibility to different sinks project we using not the same name for code settings and json settings.
Possible we could change it as users wants.

| Name | Description | Sample |
| --- | --- | --- |
| hostnameOrAddress | Graylog server hostname or IP address | example.graylog.local |
| port | Graylog server port | 12201 |
| transportType | Transport type | Tcp, Udp, Http |
| useAsyncLogging | Use async logging | true, false |
| useSecureConnectionInHttp | Use TLS for TCP transport | true, false |
| retryCount | Number of retries for TCP transport | 5 |
| retryIntervalMs | Interval between retries for TCP transport | 150 |
| useGzipCompression | Use GZIP compression for UDP transport | true, false |
| useNullDelimiter | Use Null-Byte delimiter for TCP transport | true, false |
| additionalFields | User defined fields | { "application": "MyApplication" } |
| minimumLogEventLevel | Minimum log event level | Debug, Information, Warning, Error, Fatal |
| propertyPrefix | Prefix for all fields | MyVar_ |
| maxMessageSizeInUdp | Maximum message size for UDP transport | 8192 |
| minUdpMessageSizeForCompression | Minimum message size for UDP compression | 512 |


## Versions
V1.0.0 - Initial release

## License
This project is licensed under [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) license.