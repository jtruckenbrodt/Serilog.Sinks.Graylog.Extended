using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using Serilog.Events;
using Serilog.Sinks.GraylogGelf.Exceptions;
using Serilog.Sinks.GraylogGelf.Gelf;

namespace Serilog.Sinks.GraylogGelf
{
    /// <summary>
    /// Represents a single Graylog target into which GELF messages are send asynchronously. 
    /// </summary>
    public sealed class GraylogSinkAsync : GraylogSinkBase
    {
        private readonly BlockingCollection<GelfMessage> _buffer;

        private readonly Thread _threadForAsyncProcessing;

        /// <summary>
        /// Creates a new instance of <see cref="GraylogSinkAsync"/>. 
        /// </summary>
        /// <param name="config">The <see cref="GraylogSinkConfiguration"/> used to configure the Graylog connection.</param>
        public GraylogSinkAsync(GraylogSinkConfiguration config)
            : base(config)
        {
            _buffer = new BlockingCollection<GelfMessage>();
            _threadForAsyncProcessing = new Thread(ListenForMessage)
            {
                IsBackground = true,
                Name = "GraylogMessagingThread"
            };
            _threadForAsyncProcessing.Start();
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

                _buffer.Add(messageBuilder.ToMessage());
            }
            catch (Exception ex)
            {
                LogError(ex, "Error while generating GELF message.");
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!isDisposing || _isDisposed)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            var joinTimeSpan = TimeSpan.FromSeconds(1);
            if (!_threadForAsyncProcessing.Join(joinTimeSpan))
            {
                LogError(null, "Gralog logging thread did not stop gracefully within time span {timeSpan}.", joinTimeSpan);
            }

            base.Dispose(true);
        }

        protected override void SendErrorMessage(GelfMessage msg)
        {
            _buffer.Add(msg);
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

        private void ListenForMessage()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var msg in _buffer.GetConsumingEnumerable(cancellationToken))
                    {
                        SendMessage(msg, cancellationToken);
                    }
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
                    return;
                }
                catch (Exception ex)
                {
                    LogError(ex, ex.Message);
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

                    Thread.Sleep(_config.RetryIntervalMs ?? GraylogConstants.DefaultRetryIntervalInMs);
                }
            }
            base.Dispose(true);
        }
    }
}