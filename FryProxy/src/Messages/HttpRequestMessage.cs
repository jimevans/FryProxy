using System.IO;
using FryProxy.Headers;

namespace FryProxy.Messages
{
    public class HttpRequestMessage : HttpMessage
    {
        public HttpRequestMessage(HttpRequestHeader header) : base(header)
        {
        }

        public HttpRequestMessage(HttpRequestHeader header, Stream body) : base(header, body)
        {
        }

        public HttpRequestHeader RequestHeader
        {
            get { return Header as HttpRequestHeader; }
        }
    }
}