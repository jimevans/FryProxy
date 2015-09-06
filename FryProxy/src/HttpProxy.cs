using System;
using FryProxy.Handlers;

namespace FryProxy
{
    public class HttpProxy : AbstractHttpProxy
    {
        private const Int32 DefaultHttpPort = 80;

        private static readonly IHttpMessageReader _httpMessageReader = new DefaultHttpMessageReader();
        private static readonly IHttpMessageWriter _httpMessageWriter = new DefaultHttpMessageWriter();

        private readonly IRemoteEndpointConnector _remoteEndpointConnector;

        public HttpProxy(Int32 defaultPort = DefaultHttpPort) : base(defaultPort)
        {
            _remoteEndpointConnector = new DefaultEndpointConnector(defaultPort, ServerWriteTimeout, ServerReadTimeout);
        }

        protected override IHttpMessageReader HttpMessageReader
        {
            get { return _httpMessageReader; }
        }

        protected override IHttpMessageWriter HttpMessageWriter
        {
            get { return _httpMessageWriter; }
        }

        protected override IRemoteEndpointConnector RemoteEndpointConnector
        {
            get { return _remoteEndpointConnector; }
        }
    }
}