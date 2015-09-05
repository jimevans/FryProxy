using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FryProxy.Handlers;
using FryProxy.Headers;
using FryProxy.IO;
using FryProxy.Messages;
using FryProxy.Utils;
using FryProxy.Writers;
using log4net;
using HttpContentWriter = FryProxy.Writers.HttpContentWriter;
using HttpRequestHeader = FryProxy.Headers.HttpRequestHeader;
using HttpResponseHeader = FryProxy.Headers.HttpResponseHeader;

namespace FryProxy
{
    /// <summary>
    ///     Process incoming HTTP request and provides interface for intercepting it at different stages.
    /// </summary>
    public class HttpProxy : AbstractHttpProxy
    {
        protected const Int32 DefaultHttpPort = 80;

        private readonly ProcessingPipeline _pipeline;

        /// <summary>
        ///     Creates new instance of <see cref="HttpProxy" /> using default HTTP port (80).
        /// </summary>
        public HttpProxy() : this(DefaultHttpPort)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="HttpProxy" /> using provided default port and internal buffer size.
        /// </summary>
        /// <param name="defaultPort">
        ///     Port number on destination server which will be used if not specified in request
        /// </param>
        public HttpProxy(Int32 defaultPort) : base(defaultPort)
        {
            _pipeline = new ProcessingPipeline(new Dictionary<ProcessingStage, Action<ProcessingContext>>
            {
                {ProcessingStage.ReceiveRequest, ReceiveRequest + _onRequestReceivedWrapper},
                {ProcessingStage.ConnectToServer, ConnectToServer + _onServerConnectedWrapper},
                {ProcessingStage.ReceiveResponse, ReceiveResponse + _onResponseReceivedWrapper},
                {ProcessingStage.Completed, CompleteProcessing + _onProcessingCompleteWrapper},
                {ProcessingStage.SendResponse, SendResponse + _onResponseSentWrapper}
            });
        }

        /// <summary>
        ///     Accept client connection, create <see cref="ProcessingContext" /> and <see cref="ProcessingContext.ClientStream" />
        ///     and start processing request.
        /// </summary>
        /// <param name="clientSocket">Socket opened by the client</param>
        public void HandleClient(Socket clientSocket)
        {
            Contract.Requires<ArgumentNullException>(clientSocket != null, "clientSocket");

            IHttpMessageReader reader = new DefaultHttpMessageReader();
 
            var context = new ProcessingContext
            {
                ClientSocket = clientSocket,
                ClientStream = new NetworkStream(clientSocket, true)
                {
                    ReadTimeout = (Int32) ClientReadTimeout.TotalMilliseconds,
                    WriteTimeout = (Int32) ClientWriteTimeout.TotalMilliseconds
                }
            };

            _pipeline.Start(context);
            
            if (context.Exception != null)
            {
                var errorMessage = new StringBuilder("Request processing failed.").AppendLine();

                if (context.RequestHeader != null)
                {
                    errorMessage.AppendLine("Request:");
                    errorMessage.WriteHttpTrace(context.RequestHeader);
                }

                if (context.ResponseHeader != null)
                {
                    errorMessage.AppendLine("Response:");
                    errorMessage.WriteHttpTrace(context.ResponseHeader);
                }

                errorMessage.AppendLine("Exception:");
                errorMessage.AppendLine(context.Exception.ToString());

                Logger.Error(errorMessage.ToString());
            }
        }

        /// <summary>
        ///     Read <see cref="ProcessingContext.RequestHeader" /> from <see cref="ProcessingContext.ClientStream" />.
        ///     <see cref="ProcessingContext.ClientStream" /> should be defined at this point.
        /// </summary>
        /// <param name="context">current request context</param>
        protected virtual void ReceiveRequest(ProcessingContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<InvalidContextException>(context.ClientStream != null, "ClientStream");

            var headerReader = new HttpHeaderReader(new NonBufferedStreamReader(context.ClientStream));

            try
            {
                context.RequestHeader = new HttpRequestHeader(headerReader.ReadHttpMessageHeader());

                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Request Received. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));
                }

                if (context.RequestHeader.Headers.Contains(GeneralHeaders.ProxyConnectionHeader))
                {
                    context.RequestHeader.Headers.Remove(GeneralHeaders.ProxyConnectionHeader);
                }

                context.RequestHeader.GeneralHeaders.Connection = "close";
            }
            catch (IOException ex)
            {
                if (ex.IsSocketException(SocketError.OperationAborted, SocketError.ConnectionReset))
                {
                    Logger.WarnFormat("Request was terminated by client. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));
                } 
                else if (ex is EndOfStreamException)
                {
                    Logger.ErrorFormat("Failed to read request. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));
                } 
                else if(ex.IsSocketException(SocketError.TimedOut))
                {
                    Logger.WarnFormat("Client request time out. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));    
                }
                else
                {
                    throw;
                }

                context.StopProcessing();
            }
        }

        /// <summary>
        ///     Resolve <see cref="ProcessingContext.ServerEndPoint" /> based on <see cref="ProcessingContext.RequestHeader" />,
        ///     establish connection to destination server and open <see cref="ProcessingContext.ServerStream" />.
        ///     <see cref="ProcessingContext.RequestHeader" /> should be defined.
        /// </summary>
        /// <param name="context">current request context</param>
        protected virtual void ConnectToServer(ProcessingContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<InvalidContextException>(context.RequestHeader != null, "RequestHeader");

            context.ServerEndPoint = DnsUtils.ResolveRequestEndpoint(context.RequestHeader, _defaultPort);

            context.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = (Int32) ServerReadTimeout.TotalMilliseconds,
                SendTimeout = (Int32) ServerWriteTimeout.TotalMilliseconds
            };

            context.ServerSocket.Connect(context.ServerEndPoint.Host, context.ServerEndPoint.Port);

            context.ServerStream = new NetworkStream(context.ServerSocket, true);

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Connection Established: {0}:{1}",
                    context.ServerEndPoint.Host,
                    context.ServerEndPoint.Port
                );
            }
        }

        /// <summary>
        ///     Send <see cref="ProcessingContext.RequestHeader" /> to server,
        ///     copy rest of the <see cref="ProcessingContext.ClientStream" /> to <see cref="ProcessingContext.ServerStream" />
        ///     and read <see cref="ProcessingContext.ResponseHeader" /> from <see cref="ProcessingContext.ServerStream" />.
        ///     Expects <see cref="ProcessingContext.ServerStream" />, <see cref="ProcessingContext.RequestHeader" /> and
        ///     <see cref="ProcessingContext.ClientStream" /> to be defined.
        /// </summary>
        /// <param name="context">current request context</param>
        protected virtual void ReceiveResponse(ProcessingContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<InvalidContextException>(context.ServerStream != null, "ServerStream");
            Contract.Requires<InvalidContextException>(context.RequestHeader != null, "RequestHeader");
            Contract.Requires<InvalidContextException>(context.ClientStream != null, "ClientStream");
            Contract.Requires<InvalidContextException>(context.ClientSocket != null, "ClientSocket");

            var requestWriter = new HttpContentWriter(context.ServerStream);
            var responseReader = new HttpHeaderReader(new NonBufferedStreamReader(context.ServerStream));

            try
            {
                requestWriter.Write(context.RequestHeader, context.ClientStream, context.ClientSocket.Available);
                context.ResponseHeader = new HttpResponseHeader(responseReader.ReadHttpMessageHeader());

                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Response Received: {0}", TraceUtils.GetHttpTrace(context.ResponseHeader));
                }
            }
            catch (IOException ex)
            {
                var responseWriter = new HttpResponseWriter(context.ClientStream);

                if (ex.IsSocketException(SocketError.TimedOut))
                {
                    Logger.WarnFormat("Request to remote server has timed out. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));

                    responseWriter.WriteGatewayTimeout();
                }
                else
                {
                    throw;
                }

                context.StopProcessing();
            }
        }

        /// <summary>
        ///     Send respose to <see cref="ProcessingContext.ClientStream" /> containing
        ///     <see cref="ProcessingContext.ResponseHeader" />
        ///     and rest of<see cref="ProcessingContext.ServerStream" />.
        ///     Expect <see cref="ProcessingContext.ServerStream" />, <see cref="ProcessingContext.ClientStream" /> and
        ///     <see cref="ProcessingContext.ResponseHeader" /> to be defined.
        /// </summary>
        /// <param name="context">current request context</param>
        protected virtual void SendResponse(ProcessingContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<InvalidContextException>(context.ServerStream != null, "ServerStream");
            Contract.Requires<InvalidContextException>(context.ResponseHeader != null, "ResponseHeader");
            Contract.Requires<InvalidContextException>(context.ClientStream != null, "ClientStream");
            Contract.Requires<InvalidContextException>(context.ServerSocket != null, "ServerSocket");

            var responseWriter = new HttpResponseWriter(context.ClientStream);

            try
            {
                responseWriter.Write(context.ResponseHeader, context.ServerStream, context.ServerSocket.Available);

                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Response Sent. {0}", TraceUtils.GetHttpTrace(context.ResponseHeader));
                }
            }
            catch (IOException ex)
            {
                if (ex.IsSocketException(SocketError.TimedOut))
                {
                    Logger.WarnFormat("Request to remote server has timed out. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));

                    responseWriter.WriteGatewayTimeout();
                }
                else if (ex.IsSocketException(SocketError.ConnectionReset, SocketError.ConnectionAborted))
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("Request Aborted. {0}", TraceUtils.GetHttpTrace(context.RequestHeader));
                    }
                }
                else
                {
                    throw;
                }

                context.StopProcessing();
            }
        }

        /// <summary>
        ///     Close client and server connections.
        ///     Expect <see cref="ProcessingContext.ClientStream" /> and <see cref="ProcessingContext.ServerStream" /> to be
        ///     defined.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void CompleteProcessing(ProcessingContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            if (context.ClientStream != null)
            {
                context.ClientStream.Close();
            }

            if (context.ServerStream != null)
            {
                context.ServerStream.Close();
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("[{0}] processed", context.RequestHeader.StartLine);
            }
        }
    }
}