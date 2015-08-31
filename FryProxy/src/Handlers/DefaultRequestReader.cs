using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Sockets;
using FryProxy.Headers;
using FryProxy.Readers;
using FryProxy.Utils;

namespace FryProxy.Handlers
{
    internal class DefaultRequestReader : LoggingRequestHandler, IHttpRequestReader
    {
        public virtual Tuple<HttpRequestHeader, Stream> ReadHttpRequest(Stream requestStream)
        {
            Contract.Requires<ArgumentNullException>(requestStream != null, "requestStream");
            Contract.Requires<ArgumentException>(requestStream.CanRead, "requestStream");

            var headerReader = new HttpHeaderReader(new PlainStreamReader(requestStream));

            HttpRequestHeader requestHeader = null;

            try
            {
                requestHeader = new HttpRequestHeader(headerReader.ReadHttpMessageHeader());

                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Request Received. {0}", TraceUtils.GetHttpTrace(requestHeader));
                }

                if (requestHeader.Headers.Contains(GeneralHeaders.ProxyConnectionHeader))
                {
                    requestHeader.Headers.Remove(GeneralHeaders.ProxyConnectionHeader);
                }

                requestHeader.GeneralHeaders.Connection = "close";
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex, requestHeader))
                {
                    throw;
                }
            }

            return Tuple.Create(requestHeader, requestStream);
        }

        protected Boolean HandleSocketException(Exception ex, HttpRequestHeader requestHeader)
        {
            if (ex.IsSocketException(SocketError.OperationAborted, SocketError.ConnectionReset))
            {
                throw new RequestAbortedException("Terminated by Client", requestHeader);
            }

            if (ex is EndOfStreamException)
            {
                throw new RequestAbortedException("Read Failed", requestHeader);
            }

            if (ex.IsSocketException(SocketError.TimedOut))
            {
                throw new RequestAbortedException("Client Timeout", requestHeader);
            }

            return false;
        }
    }
}