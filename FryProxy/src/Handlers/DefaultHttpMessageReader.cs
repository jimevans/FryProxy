using System;
using System.Diagnostics.Contracts;
using System.IO;
using FryProxy.Headers;
using FryProxy.Messages;
using FryProxy.Utils;

namespace FryProxy.Handlers
{
    internal class DefaultHttpMessageReader : HandlerSkeleton, IHttpMessageReader
    {
        public virtual void ReadHttpRequest(HttpRequestMessage message, Stream stream)
        {
            ReadHttpMessage(message, stream);

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Request Received. {0}", TraceUtils.GetHttpTrace(message.MessageHeader));
            }

            if (message.Headers.Contains(GeneralHeaders.ProxyConnectionHeader))
            {
                message.Headers.Remove(GeneralHeaders.ProxyConnectionHeader);
            }

            message.GeneralHeaders.Connection = "close";
        }

        public void ReadHttpResponse(HttpResponseMessage message, Stream stream)
        {
            ReadHttpMessage(message, stream);
            
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Response Received. {0}", TraceUtils.GetHttpTrace(message.MessageHeader));
            }
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