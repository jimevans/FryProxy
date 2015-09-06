using System;
using FryProxy.Handlers;

namespace FryProxy
{
    /// <summary>
    ///     Proxy capable of handling plain HTTP requests.
    /// </summary>
    public class HttpProxy : AbstractHttpProxy
    {
        private const Int32 DefaultHttpPort = 80;

        private static readonly IHttpMessageReader MessageReader = new DefaultHttpMessageReader();
        private static readonly IHttpMessageWriter MessageWriter = new DefaultHttpMessageWriter();

        private readonly IRemoteEndpointConnector _remoteEndpointConnector;

        /// <summary>
        ///     Create new instance of HTTP proxy.
        /// </summary>
        /// <param name="defaultPort">
        ///     Port number on destination server which will be used if not specified in request
        /// </param>
        public HttpProxy(Int32 defaultPort = DefaultHttpPort) : base(defaultPort)
        {
            _remoteEndpointConnector = new DefaultEndpointConnector(defaultPort, ServerWriteTimeout, ServerReadTimeout);
        }

        protected override IHttpMessageReader HttpMessageReader
        {
            get { return MessageReader; }
        }

        protected override IHttpMessageWriter HttpMessageWriter
        {
            get { return MessageWriter; }
        }

        protected override IRemoteEndpointConnector RemoteEndpointConnector
        {
            get { return _remoteEndpointConnector; }
        }
    }
}