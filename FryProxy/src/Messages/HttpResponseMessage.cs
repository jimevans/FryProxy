using System;
using System.IO;
using FryProxy.Headers;
using FryProxy.IO;

namespace FryProxy.Messages
{
    public class HttpResponseMessage : HttpMessage
    {
        public HttpResponseMessage(HttpResponseHeader header) : base(header)
        {
        }

        public HttpResponseMessage(HttpResponseHeader header, Stream body) : base(header, body)
        {
        }

        public HttpResponseHeader ResponseHeader
        {
            get { return Header as HttpResponseHeader; }
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
    }
}