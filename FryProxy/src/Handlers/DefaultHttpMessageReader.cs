using System;
using System.Diagnostics.Contracts;
using System.IO;
using FryProxy.Headers;
using FryProxy.IO;
using FryProxy.Utils;

namespace FryProxy.Handlers
{
    internal class DefaultHttpMessageReader : RequestHandlerSkeleton, IHttpMessageReader
    {
        public virtual Tuple<HttpRequestHeader, Stream> ReadHttpRequest(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream");
            Contract.Requires<ArgumentException>(stream.CanRead, "stream");

            HttpRequestHeader requestHeader = null;

            try
            {
                requestHeader = new HttpRequestHeader(ReadHttpMessageHeader(stream));

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
                if (!HandleSocketException(ex))
                {
                    throw;
                }
            }

            return Tuple.Create(requestHeader, stream);
        }

        public Tuple<HttpResponseHeader, Stream> ReadHttpResponse(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream");
            Contract.Requires<ArgumentException>(stream.CanRead, "stream");

            HttpResponseHeader header = null;

            try
            {
                header = new HttpResponseHeader(ReadHttpMessageHeader(stream));
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex))
                {
                    throw;
                }
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Response Received: {0}", TraceUtils.GetHttpTrace(header));
            }

            return Tuple.Create(header, stream);
        }

        private static HttpMessageHeader ReadHttpMessageHeader(Stream stream)
        {
            return new HttpHeaderReader(new NonBufferedStreamReader(stream)).ReadHttpMessageHeader();
        }
    }
}