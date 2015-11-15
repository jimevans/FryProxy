using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using log4net;

namespace FryProxy
{
    /// <summary>
    ///     Handler for <see cref="AbstractHttpProxy.ConnectionAcceped" /> event
    /// </summary>
    /// <param name="endPoint">Client IP address and port number</param>
    public delegate void ConnectionAcceptedEventHandler(EndPoint endPoint);

    /// <summary>
    ///     Handler for events associated with receiving client request
    /// </summary>
    /// <seealso cref="AbstractHttpProxy.RequestReceived" />
    /// <param name="requestMessage">HTTP message recevied from client</param>
    public delegate void RequestReceivedEventHandler(HttpRequestMessage requestMessage);

    /// <summary>
    ///     Handler for events associated with receiving response from remote server
    /// </summary>
    /// <seealso cref="AbstractHttpProxy.ResponseReceived" />
    /// <seealso cref="AbstractHttpProxy.ResponseSent" />
    /// <param name="responseMessage"></param>
    public delegate void ResponseReceivedEventHandler(HttpResponseMessage responseMessage);

    /// <summary>
    ///     Defines basic request procssing flow which include following:
    ///     - fire <see cref="ConnectionAcceped" /> event once connectin is received
    ///     - receive client request and fire <see cref="RequestReceived" /> event
    ///     - reply client request to destination server
    ///     - receive respons from destination server and fire <see cref="ResponseReceived" /> event
    ///     - reply to client with server response and send <see cref="ResponseSent" /> event
    /// </summary>
    public abstract class AbstractHttpProxy
    {
        /// <summary>
        ///     Instance level logger
        /// </summary>
        protected readonly ILog Logger;

        /// <summary>
        ///     Create new proxy instance initializing logger.
        /// </summary>
        protected AbstractHttpProxy()
        {
            Logger = LogManager.GetLogger(GetType());
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
        ///     Event fired when proxy receives client connection before any processing is done
        /// </summary>
        public event ConnectionAcceptedEventHandler ConnectionAcceped;

        /// <summary>
        ///     Event fired once request headers received from client
        /// </summary>
        public event RequestReceivedEventHandler RequestReceived;

        /// <summary>
        ///     Event fired after response headers recevied from remote server
        /// </summary>
        public event ResponseReceivedEventHandler ResponseReceived;

        /// <summary>
        ///     Event fired after server response was send to client
        /// </summary>
        public event ResponseReceivedEventHandler ResponseSent;

        /// <summary>
        ///     Start request prcessing flow.
        /// </summary>
        /// <param name="socket">Socket opened by proxy client</param>
        public void AcceptSocket(Socket socket)
        {
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage = null;

            try
            {
                var clientStream = CreateClientStream(socket);
                Logger.DebugFormat("Connection accepted. Remote ednpoint: {0}", socket.RemoteEndPoint);
                ConnectionAcceped?.Invoke(socket.RemoteEndPoint);

                requestMessage = ReceiveRequest(clientStream);
                Logger.DebugFormat("Request received: {0}", requestMessage);
                RequestReceived?.Invoke(requestMessage);

                responseMessage = ReceiveResponse(requestMessage);
                Logger.DebugFormat("Response received: {0}", responseMessage);
                ResponseReceived?.Invoke(responseMessage);

                SendResponse(responseMessage, clientStream);
                Logger.DebugFormat("Response sent: {0}", responseMessage);
                ResponseSent?.Invoke(responseMessage);
            }
            catch (Exception ex)
            {
                var message = new StringBuilder()
                    .AppendLine("Error during request processing.")
                    .AppendFormat("Request: {0}", requestMessage).AppendLine()
                    .AppendFormat("Response: {0}", responseMessage).AppendLine()
                    .ToString();

                Logger.Error(message, ex);
            }
            finally
            {
                responseMessage?.Dispose();
                requestMessage?.Dispose();
            }
        }

        protected abstract Stream CreateClientStream(Socket socket);

        /// <summary>
        ///     Read request from client stream
        /// </summary>
        /// <param name="stream">Stream containing client request</param>
        /// <returns>HTTP request read from stream</returns>
        protected abstract HttpRequestMessage ReceiveRequest(Stream stream);

        /// <summary>
        ///     Send request message to destination server and read it's reponse
        /// </summary>
        /// <param name="request">Client HTTP request</param>
        /// <returns>Server HTTP response</returns>
        protected abstract HttpResponseMessage ReceiveResponse(HttpRequestMessage request);

        /// <summary>
        ///     Send server response back to client
        /// </summary>
        /// <param name="response">Server HTTP response</param>
        /// <param name="stream">Cient stream</param>
        protected abstract void SendResponse(HttpResponseMessage response, Stream stream);
    }
}