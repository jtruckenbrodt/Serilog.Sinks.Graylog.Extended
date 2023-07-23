using System;
using System.Net;
using System.Net.Http;

using Serilog.Sinks.GraylogGelf.Exceptions;
using Serilog.Sinks.GraylogGelf.Gelf;

namespace Serilog.Sinks.GraylogGelf.Transports
{
    internal sealed class HttpTransport : ITransport, IDisposable
    {
        private readonly HttpClient _client;

        private readonly IGelfMessageSerializer _messageSerializer;

        private readonly Uri _uri;

        public HttpTransport(Uri uri, IGelfMessageSerializer messageSerializer)
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
            _client = new HttpClient();
            _client.DefaultRequestHeaders.ExpectContinue = false; // important, cannot be handled by GRAYLOG
            var sp = ServicePointManager.FindServicePoint(_uri);
            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute
            _client.Timeout = TimeSpan.FromSeconds(15);
        }

        public void Dispose()
        {
            _client.SafeDispose();
        }

        public void Send(GelfMessage message)
        {
            var msg = _messageSerializer.SerializeToString(message);
            var stringContent = new StringContent(msg, System.Text.Encoding.UTF8);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = _client.PostAsync(_uri, stringContent);
            if (response.Result.StatusCode == HttpStatusCode.Accepted)
            {
                throw new GraylogHttpTransportException($"Received status code '{response.Result.StatusCode}' from GrayLog.");
            }
        }
    }
}
