using System;
using System.IO;
using System.Net;
using System.Net.Http;
using FryProxy.Handlers;

namespace FryProxy
{
    internal class SimpleHttpProxy : AbstractHttpProxy
    {
        private readonly int _defaultPort;

        internal SimpleHttpProxy(IHttpMessageWriter messageWriter, int defaultPort) : base(messageWriter)
        {
            _defaultPort = defaultPort;
        }

        protected override HttpRequestMessage ReadRequest(Stream stream)
        {
            return new HttpRequestMessage(HttpMethod.Get, new Uri(""))
            {
                Content = new StreamContent(stream, 1)
            };
        }

        protected override HttpResponseMessage ReceiveResponse(HttpRequestMessage request)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}