using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using FryProxy.Handlers;

namespace FryProxy
{
    /// <summary>
    /// Handler for <see cref="AbstractHttpProxy.ConnectionAcceped"/> event
    /// </summary>
    /// <param name="endPoint">Client IP address and port number</param>
    public delegate void ConnectionAcceptedEventHandler(IPEndPoint endPoint);

    /// <summary>
    /// Handler for events associated with receiving client request
    /// </summary>
    /// <seealso cref="AbstractHttpProxy.RequestReceived"/>
    /// <param name="requestMessage">HTTP message recevied from client</param>
    public delegate void RequestReceivedEventHandler(HttpRequestMessage requestMessage);

    /// <summary>
    /// Handler for events associated with receiving response from remote server
    /// </summary>
    /// <seealso cref="AbstractHttpProxy.ResponseReceived"/>
    /// <seealso cref="AbstractHttpProxy.ResponseSent"/>
    /// <param name="responseMessage"></param>
    public delegate void ResponseReceivedEventHandler(HttpResponseMessage responseMessage);

    /// <summary>
    /// Defines basic request procssing flow which include following:
    /// - fire <see cref="ConnectionAcceped"/> event once connectin is received
    /// - receive client request and fire <see cref="RequestReceived"/> event
    /// - reply client request to destination server
    /// - receive respons from destination server and fire <see cref="ResponseReceived"/> event
    /// - reply to client with server response and send <see cref="ResponseSent"/> event
    /// </summary>
    public abstract class AbstractHttpProxy
    {
        protected readonly IHttpMessageWriter MessageWriter;

        protected AbstractHttpProxy(IHttpMessageWriter messageWriter)
        {
            MessageWriter = messageWriter;
        }

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

        /// <summary>
        /// Event fired when proxy receives client connection before any processing is done
        /// </summary>
        public event ConnectionAcceptedEventHandler ConnectionAcceped;

        /// <summary>
        /// Event fired once request headers received from client
        /// </summary>
        public event RequestReceivedEventHandler RequestReceived;

        /// <summary>
        /// Event fired after response headers recevied from remote server
        /// </summary>
        public event ResponseReceivedEventHandler ResponseReceived;

        /// <summary>
        /// Event fired after server response was send to client
        /// </summary>
        public event ResponseReceivedEventHandler ResponseSent;

        public void AcceptSocket(Socket socket)
        {
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage = null;

            try
            {
                ConnectionAcceped?.Invoke(socket.RemoteEndPoint as IPEndPoint);

                var clientStream = new NetworkStream(socket, true)
                {
                    ReadTimeout = (int) ClientReadTimeout.TotalMilliseconds,
                    WriteTimeout = (int) ClientWriteTimeout.TotalMilliseconds
                };

                requestMessage = ReadRequest(clientStream);

                RequestReceived?.Invoke(requestMessage);

                responseMessage = ReceiveResponse(requestMessage);

                ResponseReceived?.Invoke(responseMessage);

                MessageWriter.Write(responseMessage, clientStream);

                ResponseSent?.Invoke(responseMessage);
            }
            finally
            {
                responseMessage?.Dispose();
                requestMessage?.Dispose();
            }
        }

        protected abstract HttpRequestMessage ReadRequest(Stream stream);

        protected abstract HttpResponseMessage ReceiveResponse(HttpRequestMessage request);
    }
}