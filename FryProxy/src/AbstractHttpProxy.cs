using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FryProxy.Handlers;
using FryProxy.Utils;
using log4net;

namespace FryProxy
{
    /// <summary>
    ///     Basic HTTP proxy. Implements request processing flow 
    ///     and delegates interaction with network protocols to descendants.
    /// </summary>
    public abstract class AbstractHttpProxy
    {
        protected readonly ILog Logger;

        private readonly Int32 _defaultPort;

        /// <summary>
        ///     Create new instance of HTTP proxy. All timeouts will be initialized with default values.
        /// </summary>
        /// <param name="defaultPort">
        ///     Port number on destination server which will be used if not specified in request
        /// </param>
        protected AbstractHttpProxy(int defaultPort)
        {
            Contract.Requires<ArgumentOutOfRangeException>(
                defaultPort > IPEndPoint.MinPort
                && defaultPort < IPEndPoint.MaxPort, "defaultPort"
                );

            _defaultPort = defaultPort;

            ClientReadTimeout = TimeSpan.FromSeconds(5);
            ClientWriteTimeout = TimeSpan.FromSeconds(5);
            ServerReadTimeout = TimeSpan.FromSeconds(15);
            ServerWriteTimeout = TimeSpan.FromSeconds(15);

            Logger = LogManager.GetLogger(GetType());
        }

        /// <summary>
        ///     Called when all other stages of request processing are done.
        ///     All <see cref="ProcessingContext" /> information should be available now.
        /// </summary>
        public Action<ProcessingContext> OnProcessingComplete { get; set; }

        /// <summary>
        ///     Called when request from client is received by proxy.
        ///     <see cref="ProcessingContext.RequestHeader" /> and <see cref="ProcessingContext.ClientStream" /> are available at
        ///     this stage.
        /// </summary>
        public Action<ProcessingContext> OnRequestReceived { get; set; }

        /// <summary>
        ///     Called when response from destination server is received by proxy.
        ///     <see cref="ProcessingContext.ResponseHeader" /> is added at this stage.
        /// </summary>
        public Action<ProcessingContext> OnResponseReceived { get; set; }

        /// <summary>
        ///     Called when server response has been relayed to client.
        ///     All <see cref="ProcessingContext" /> information should be available.
        /// </summary>
        public Action<ProcessingContext> OnResponseSent { get; set; }

        /// <summary>
        ///     Called when proxy has established connection to destination server.
        ///     <see cref="ProcessingContext.ServerEndPoint" /> and <see cref="ProcessingContext.ServerStream" /> are defined at
        ///     this stage.
        /// </summary>
        public Action<ProcessingContext> OnServerConnected { get; set; }

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
        ///     Port number on destination server which will be used if not specified in request
        /// </summary>
        public Int32 DefaultPort
        {
            get { return _defaultPort; }
        }

        /// <summary>
        ///     Object used for reading request and response HTTP messages
        /// </summary>
        protected abstract IHttpMessageReader HttpMessageReader { get; }

        /// <summary>
        ///     Object used for writing request and reponse HTTP messages
        /// </summary>
        protected abstract IHttpMessageWriter HttpMessageWriter { get; }

        /// <summary>
        ///     Object used for opening sockets to remote servers and wrapping opened socket in stream
        /// </summary>
        protected abstract IRemoteEndpointConnector RemoteEndpointConnector { get; }

        /// <summary>
        ///     Handle request by reading client request, connecting to original destination endpoint,
        ///     relay message to it and relay response back to client. Also trigger custom request 
        ///     handlers on appropriate processing stages.
        /// </summary>
        /// <param name="socket">socket opened by proxy client</param>
        public void AcceptClientSocket(Socket socket)
        {
            var ctx = new ProcessingContext
            {
                ClientSocket = socket
            };

            Socket serverSocket = null;
            Stream serverStream = null;
            Stream clientStream = null;

            try
            {
                ctx.ClientStream = clientStream = new NetworkStream(socket, true)
                {
                    ReadTimeout = (Int32) ClientReadTimeout.TotalMilliseconds,
                    WriteTimeout = (Int32) ClientWriteTimeout.TotalMilliseconds
                };

                var requestMessage = HttpMessageReader.ReadHttpRequest(ctx.ClientStream);
                ctx.RequestHeader = requestMessage.RequestHeader;
                ctx.ClientStream = requestMessage.Body;

                if (InvokeHanlder(ctx, OnRequestReceived))
                {
                    return;
                }

                var serverSocketAndStream = RemoteEndpointConnector.EstablishConnection(requestMessage.RequestHeader);
                serverSocket = serverSocketAndStream.Item1;
                ctx.ServerStream = serverStream = serverSocketAndStream.Item2;

                if (InvokeHanlder(ctx, OnServerConnected))
                {
                    return;
                }

                HttpMessageWriter.WriteHttpMessage(requestMessage, ctx.ServerStream);
                
                var responseMessage = HttpMessageReader.ReadHttpResponse(ctx.ServerStream);
                ctx.ResponseHeader = responseMessage.ResponseHeader;

                if (InvokeHanlder(ctx, OnResponseReceived))
                {
                    return;
                }

                HttpMessageWriter.WriteHttpMessage(responseMessage, ctx.ClientStream);
                
                InvokeHanlder(ctx, OnResponseSent);
            }
            catch (RequestAbortedException ex)
            {
                Logger.Warn("Request Aborted", ex);
            }
            catch (Exception ex)
            {
                LogException(ex, ctx);
            }
            finally
            {
                ctx.StopProcessing();

                try
                {
                    foreach (var stream in new[] {clientStream, serverStream, ctx.ClientStream, ctx.ServerStream})
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                    }

                    if (serverSocket != null)
                    {
                        serverSocket.Close();
                    }

                    InvokeHanlder(ctx, OnProcessingComplete);
                }
                catch (Exception ex)
                {
                    LogException(ex, ctx);
                }
            }
        }

        private void LogException(Exception ex, ProcessingContext ctx)
        {
            var errorMessage = new StringBuilder("Request processing failed.");

            errorMessage.AppendLine();

            if (ctx.RequestHeader != null)
            {
                errorMessage.AppendLine("Request:");
                errorMessage.WriteHttpTraceMessage(ctx.RequestHeader);
            }

            if (ctx.ResponseHeader != null)
            {
                errorMessage.AppendLine("Response:");
                errorMessage.WriteHttpTraceMessage(ctx.ResponseHeader);
            }

            errorMessage.AppendLine("Exception:");
            errorMessage.AppendLine(ex.ToString());

            Logger.Error(errorMessage.ToString());
        }

        private static Boolean InvokeHanlder(ProcessingContext context, Action<ProcessingContext> handler)
        {
            if (handler == null)
            {
                return false;
            }

            handler(context);

            return context.Processed;
        }
    }
}