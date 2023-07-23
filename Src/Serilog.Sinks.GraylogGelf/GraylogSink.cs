using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Serilog.Events;
using Serilog.Sinks.Graylog.Extended.Exceptions;
using Serilog.Sinks.Graylog.Extended.Gelf;

namespace Serilog.Sinks.Graylog.Extended
{
    /// <summary>
    /// Represents a single Graylog target into which GELF messages are send synchronously. 
    /// </summary>
    public sealed class GraylogSink : GraylogSinkBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraylogSink"/>. 
        /// </summary>
        /// <param name="config">The <see cref="GraylogSinkConfiguration"/> used to configure the Graylog connection.</param>
        public GraylogSink(GraylogSinkConfiguration config)
            : base(config)
        {
        }

        /// <inheritdoc />
        public override void Emit(LogEvent logEvent)
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                var messageBuilder = CreateGelfMessageBuilder(logEvent.RenderMessage(), logEvent.Timestamp.UtcDateTime, logEvent.Level.ToGelfLevel());
                if (logEvent.Exception != null)
                {
                    messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}isExceptionMessage", 1);
                    messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}exceptionMessage", logEvent.Exception.Message);
                    messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}exceptionStackTrace", logEvent.Exception.StackTrace);
                    if (logEvent.Exception.InnerException != null)
                    {
                        messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}innerExceptionMessage", logEvent.Exception.InnerException.Message);
                        messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}innerExceptionStackTrace", logEvent.Exception.InnerException.StackTrace);
                    }
                }

                if (_config.PropertyCatalog != null)
                {
                    CheckLogProperties(logEvent.Properties, messageBuilder);
                }
                else
                {
                    AddCodeDefinedProperties(logEvent.Properties, messageBuilder);
                }

                try
                {
                    SendMessage(messageBuilder.ToMessage(), _cancellationTokenSource.Token);
                }
                catch (ObjectDisposedException)
                {
                    LogError(null, "Message buffer was disposed while reading from it.");
                }
                catch (GraylogConnectionException ex)
                {
                    LogError(ex, "Error while trying to use transport of type {transportType}.", _config.TransportType);
                }
                catch (GraylogHttpTransportException ex)
                {
                    LogError(ex, "Error while trying to send log message via HTTP transport into Graylog.");
                }
                catch (GraylogSendRetryFailedException ex)
                {
                    LogError(ex, ex.Message);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Error while generating GELF message.");
            }
        }

        protected override void SendErrorMessage(GelfMessage msg)
        {
            SendMessage(msg, _cancellationTokenSource.Token);
        }

        private void AddCodeDefinedProperties(
            IReadOnlyDictionary<string, LogEventPropertyValue> logEventProperties,
            GelfMessageBuilder messageBuilder)
        {
            foreach (var property in logEventProperties)
            {
                if (property.Value != null)
                {
                    string value = property.Value.ToString();
                    value = value.TrimStart('\"');
                    value = value.TrimEnd('\"');
                    messageBuilder.SetAdditionalField($"{_config.PropertyPrefix}{property.Key}", value);
                }
            }
        }

        private void SendMessage(GelfMessage msg, CancellationToken cancelToken)
        {
            var sendRetryCount = _config.RetryCount ?? GraylogConstants.DefaultRetryCount;
            var maxRetries = sendRetryCount;
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    GetTransport()?.Send(msg);
                    return;
                }
                catch (Exception ex)
                {
                    sendRetryCount--;
                    if (sendRetryCount < 1)
                    {
                        throw new GraylogSendRetryFailedException(
                            $"Cannot send log message after {maxRetries} attempts, giving up. See inner Exceptions for details.",
                            ex);
                    }

                    var interval = TimeSpan.FromMilliseconds(_config.RetryIntervalMs ?? GraylogConstants.DefaultRetryIntervalInMs);
                    if (interval <= TimeSpan.Zero)
                    {
                        continue;
                    }

                    Task.Delay(interval, cancelToken).Wait(cancelToken);
                }
            }
        }
    }
}
