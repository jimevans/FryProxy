using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using FryProxy.IO;

namespace FryProxy
{
    /// <summary>
    ///     Basic implementaion of HTTP proxy unaware of SSL.
    /// </summary>
    internal class DefaultHttpProxy : AbstractHttpProxy
    {
        private readonly IHttpMessageReader _messageReader;
        private readonly IHttpMessageWriter _messageWriter;
        private readonly IRemoteEndpointResolver _remoteResolver;
        private readonly IRemoteEndpointConnector _remoteConnector;
        private readonly IStreamFactory _streamFactory;

        public DefaultHttpProxy(
            IHttpMessageReader messageReader, 
            IHttpMessageWriter messageWriter,
            IRemoteEndpointResolver remoteResolver,
            IRemoteEndpointConnector remoteConnector,
            IStreamFactory streamFactory)
        {
            Contract.Requires<ArgumentNullException>(messageReader != null, nameof(messageReader));
            Contract.Requires<ArgumentNullException>(messageWriter != null, nameof(messageWriter));
            Contract.Requires<ArgumentNullException>(remoteResolver != null, nameof(remoteResolver));
            Contract.Requires<ArgumentNullException>(remoteConnector != null, nameof(remoteConnector));
            Contract.Requires<ArgumentNullException>(streamFactory != null, nameof(streamFactory));

            _messageReader = messageReader;
            _messageWriter = messageWriter;
            _remoteResolver = remoteResolver;
            _remoteConnector = remoteConnector;
            _streamFactory = streamFactory;
        }

        protected override Stream CreateClientStream(Socket socket)
        {
            Contract.Requires<ArgumentNullException>(socket != null, nameof(socket));

            return _streamFactory.CreateStream(socket, ClientReadTimeout, ClientWriteTimeout);
        }

        protected override HttpRequestMessage ReceiveRequest(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null, nameof(stream));
            Contract.Requires<ArgumentException>(stream.CanRead, nameof(stream));

            return _messageReader.ReadRequestMessage(stream);
        }

        protected override HttpResponseMessage ReceiveResponse(HttpRequestMessage request)
        {
            Contract.Requires<ArgumentNullException>(request != null, nameof(request));

            var remoteEndpoint = _remoteResolver.Resolve(request);
            Logger.DebugFormat("Remote endpoint resolved: {0}", remoteEndpoint);

            var stream = _streamFactory.CreateStream(
                _remoteConnector.Connect(remoteEndpoint),
                ServerReadTimeout,
                ServerWriteTimeout);
            Logger.DebugFormat("Connected to remote server [{0}]", remoteEndpoint);

            _messageWriter.Write(request, stream);
            Logger.DebugFormat("Request was sent to remote server [{0}]: {1}", remoteEndpoint, request);

            return _messageReader.ReadResponseMessage(stream);
        }

        protected override void SendResponse(HttpResponseMessage response, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(response != null, nameof(response));
            Contract.Requires<ArgumentNullException>(stream != null, nameof(stream));
            Contract.Requires<ArgumentException>(stream.CanWrite, nameof(stream));

            _messageWriter.Write(response, stream);
        }
    }
}