using System;
using System.IO;
using FryProxy.Headers;
using FryProxy.IO;

namespace FryProxy.Messages
{
    public class HttpResponseMessage : HttpMessage
    {
        public HttpResponseMessage()
        {
        }

        public HttpResponseMessage(HttpResponseHeader messageHeader) : base(messageHeader)
        {
        }

        public HttpResponseMessage(HttpResponseHeader messageHeader, Stream contentStream) : base(messageHeader, contentStream)
        {
        }

        public HttpResponseHeader ResponseHeader
        {
            get { return MessageHeader as HttpResponseHeader; }
        }

        public Boolean IsRedirect
        {
            get
            {
                return ResponseHeader != null
                       && (ResponseHeader.StatusCode >= 300 || ResponseHeader.StatusCode < 400)
                       && !String.IsNullOrEmpty(ResponseHeader.Location);
            }
        }

        protected override void WriteBody(HttpContentWriter writer)
        {
            if (IsRedirect)
            {
                return;
            }

            base.WriteBody(writer);
        }

        protected override HttpMessageHeader ReadHeader(Stream stream)
        {
            return new HttpResponseHeader(base.ReadHeader(stream));
        }
    }
}