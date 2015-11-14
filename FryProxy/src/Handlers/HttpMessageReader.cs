using System;
using System.Diagnostics.Contracts;
using System.IO;
using FryProxy.Headers;
using FryProxy.Messages;
using FryProxy.Utils;

namespace FryProxy.Handlers
{
    internal class HttpMessageReader : HandlerSkeleton, IHttpMessageReader
    {
        public virtual HttpRequestMessage ReadHttpRequest(Stream stream)
        {
            var message = new HttpRequestMessage();

            ReadHttpMessage(message, stream);

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Request Received. {0}", TraceUtils.GetHttpTraceMessage(message.MessageHeader));
            }

            if (message.HeadersCollection.Contains(GeneralHeaders.ProxyConnectionHeader))
            {
                message.HeadersCollection.Remove(GeneralHeaders.ProxyConnectionHeader);
            }

            message.GeneralHeaders.Connection = "close";

            return message;
        }

        public HttpResponseMessage ReadHttpResponse(Stream stream)
        {
            var message = new HttpResponseMessage();

            ReadHttpMessage(message, stream);
            
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Response Received. {0}", TraceUtils.GetHttpTraceMessage(message.MessageHeader));
            }

            return message;
        }

        private void ReadHttpMessage(HttpMessage message, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(message != null, "message");
            Contract.Requires<ArgumentNullException>(stream != null, "stream");
            Contract.Requires<ArgumentException>(stream.CanRead, "stream");

            try
            {
                message.Read(stream);
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex))
                {
                    throw;
                }
            }
        }
    }
}