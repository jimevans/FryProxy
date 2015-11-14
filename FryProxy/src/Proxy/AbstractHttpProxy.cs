using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using FryProxy.Api;
using FryProxy.Handlers;
using FryProxy.Messages;

namespace FryProxy.Proxy
{
    internal abstract class AbstractHttpProxy : IHttpProxy
    {

        protected readonly IHttpMessageWriter MessageWriter;

        /// <summary>
        ///     Client socket read timeout
        /// </summary>
        public TimeSpan ClientReadTimeout { get; set; }

        /// <summary>
        ///     Client socket write timeout
        /// </summary>
        public TimeSpan ClientWriteTimeout { get; set; }

        /// <summary>
        ///     Server socket read timeout
        /// </summary>
        public TimeSpan ServerReadTimeout { get; set; }

        /// <summary>
        ///     Server socket write timeout
        /// </summary>
        public TimeSpan ServerWriteTimeout { get; set; }

        public event ConnectionAccepted ConnectionAcceped;
        public event RequestReceived RequestReceived;
        public event ResponseReceived ResponseReceived;
        public event ResponseSent ResponseSent;

        protected AbstractHttpProxy(IHttpMessageWriter messageWriter)
        {
            MessageWriter = messageWriter;
        }

        public void AcceptSocket(Socket socket)
        {
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage = null;

            try
            {
                if (ConnectionAcceped != null)
                {
                   ConnectionAcceped(socket.LocalEndPoint as IPEndPoint);
                }

                var clientStream = new NetworkStream(socket, true)
                {
                    ReadTimeout = (int) ClientReadTimeout.TotalMilliseconds,
                    WriteTimeout = (int) ClientWriteTimeout.TotalMilliseconds
                };

                requestMessage = ReadRequest(clientStream);

                if (RequestReceived != null)
                {
                    RequestReceived(requestMessage);
                }

                responseMessage = ReceiveResponse(requestMessage);

                if (ResponseReceived != null)
                {
                    ResponseReceived(requestMessage, responseMessage);
                }

                MessageWriter.WriteHttpMessage(responseMessage, clientStream);

                if (ResponseSent != null)
                {
                    ResponseSent(requestMessage, responseMessage);
                }
            }
            finally
            {
                if (responseMessage != null)
                {
                    responseMessage.Dispose();
                }

                if (requestMessage != null)
                {
                    requestMessage.Dispose();
                }
            }
        }

        protected abstract HttpRequestMessage ReadRequest(Stream stream);

        protected abstract HttpResponseMessage ReceiveResponse(HttpRequestMessage request);
    }
}