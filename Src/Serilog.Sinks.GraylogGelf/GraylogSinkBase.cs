using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Graylog.Extended.Gelf;
using Serilog.Sinks.Graylog.Extended.Transports;

namespace Serilog.Sinks.Graylog.Extended
{
    /// <summary>
    /// The base class for Graylog sinks containing shared functionality.
    /// </summary>
    public abstract class GraylogSinkBase : ILogEventSink, IDisposable
    {
        private ITransport _transport;
        protected bool _isDisposed;
        protected readonly GraylogSinkConfiguration _config;
        protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected GraylogSinkBase(GraylogSinkConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Emits a single log event to Graylog.
        /// </summary>
        /// <param name="logEvent">The <see cref="LogEvent"/> to be converted into a GELF message and finally send to Graylog.</param>
        public abstract void Emit(LogEvent logEvent);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing || _isDisposed) return;
            _transport?.Dispose();
            _transport = null;
            _isDisposed = true;
        }

        protected void CheckLogProperties(IReadOnlyDictionary<string, LogEventPropertyValue> logEventProperties, GelfMessageBuilder messageBuilder)
        {
            foreach (var property in logEventProperties)
            {
                var propCheckResult = _config.PropertyCatalog.CheckProperty(property.Key, property.Value);
                switch (propCheckResult.CheckResult)
                {
                    case GraylogPropertyCheckResultType.Ok:
                        messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}{property.Key}", propCheckResult.PropertyValue);
                        break;
                    case GraylogPropertyCheckResultType.ErrorNameNotRegistered:
                        // property not registered as Graylog property - so do not append
                        continue;
                    default:
                        if (propCheckResult.ExpectedValueType == GraylogPropertyType.String &&
                            propCheckResult.CheckResult.HasFlag(GraylogPropertyCheckResultType.ErrorValueTypeNotMatching))
                        {
                            // try to correct by converting value into string
                                var conv = propCheckResult.PropertyValue as IConvertible;
                                var strValue = conv?.ToString(CultureInfo.InvariantCulture) ?? propCheckResult.PropertyValue.ToString();
                                // correct property value by converting to string
                                messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}{property.Key}", strValue);
                                break;
                        }
                        // send value type mismatch event
                        messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}isGelfPropertyError", 1);
                        var msg = CreatePropertyErrorMessage(propCheckResult);
                        SendErrorMessage(msg);
                        break;
                }
            }
        }

        private GelfMessage CreatePropertyErrorMessage(GraylogPropertyCheckResult propCheckResult)
        {
            var propertyErrorMessageBuilder = CreateGelfMessageBuilder("Error in property name/type check.", DateTime.UtcNow, GelfLevel.Error);
            if (string.IsNullOrEmpty(propCheckResult.ExpectedPropertyName)) propertyErrorMessageBuilder.SetAdditionalField($"{_config.PropertyPrefix}expectedPropertyName", propCheckResult.ExpectedPropertyName);
            if (string.IsNullOrEmpty(propCheckResult.ReceivedPropertyName)) propertyErrorMessageBuilder.SetAdditionalField($"{_config.PropertyPrefix}receivedPropertyName", propCheckResult.ReceivedPropertyName);
            if (propCheckResult.ExpectedValueType != GraylogPropertyType.Undefined) propertyErrorMessageBuilder.SetAdditionalField($"{_config.PropertyPrefix}expectedPropertyType", propCheckResult.ExpectedValueType.ToString());
            if (propCheckResult.ReceivedValueType != GraylogPropertyType.Undefined) propertyErrorMessageBuilder.SetAdditionalField($"{_config.PropertyPrefix}receivedPropertyType", propCheckResult.ReceivedValueType.ToString());
            return propertyErrorMessageBuilder.ToMessage();
        }
 
        protected abstract void SendErrorMessage(GelfMessage msg);

        protected ITransport GetTransport()
        {
            if (_transport != null)
            {
                return _transport;
            }
            if (_isDisposed) return null;
            switch (_config.GraylogTransportType)
            {
                case GraylogTransportType.Http:
                    return _transport = new HttpTransport(new Uri(_config.Host), new GelfMessageSerializer());
                case GraylogTransportType.Tcp:
                    return _transport = new TcpTransport(_config.Host, _config.Port, _config.UseSecureConnection,
                        _config.UseNullByteDelimiter, new GelfMessageSerializer());
                case GraylogTransportType.Udp:
                    return _transport = new UdpTransport(_config.Host, _config.Port, new GelfMessageSerializer(),
                        new GelfChunkEncoder(_config.MaxUdpMessageSize ?? GelfConstants.MaxUdpMessageSize),
                            _config.MinUdpMessageSizeForCompression ?? GelfConstants.MinUdpMessageSizeForCompression);
                default:
                    throw new NotSupportedException($"Transport type '{_config.GraylogTransportType}' is not supported!");
            }
        }

        protected GelfMessageBuilder CreateGelfMessageBuilder(string message, DateTime timeStamp, GelfLevel logLevel)
        {
            var messageBuilder = new GelfMessageBuilder(message, Environment.MachineName.ToUpperInvariant(), timeStamp, logLevel);
            if (_config.AdditionalProperties == null) return messageBuilder;
            foreach (var kvp in _config.AdditionalProperties)
            {
                messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}{kvp.Key}", kvp.Value);
            }
            return messageBuilder;
        }

        protected void LogError(Exception ex, string message, params object[] parameters)
        {
            if (_config.LogErrorSink!= null)
            {
                var errorMsg = "An error occured while trying to log to Graylog." +
                               Environment.NewLine + string.Format(message, parameters);
                _config.LogErrorSink.Emit(new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Error, ex,
                    new MessageTemplate(errorMsg, Enumerable.Empty<MessageTemplateToken>()), Enumerable.Empty<LogEventProperty>()));
            }
            else if (ex != null)
            {
                Log.Error(ex, message, parameters);
            }
            else
            {
                Log.Error(message, parameters);
            }
        }
    }
}