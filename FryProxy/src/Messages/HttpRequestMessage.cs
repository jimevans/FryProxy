using System.IO;
using FryProxy.Headers;

namespace FryProxy.Messages
{
    public class HttpRequestMessage : HttpMessage
    {
        public HttpRequestMessage()
        {
        }

        public HttpRequestMessage(HttpRequestHeader messageHeader) : base(messageHeader)
        {
        }

        public HttpRequestMessage(HttpRequestHeader messageHeader, Stream contentStream) : base(messageHeader, contentStream)
        {
        }

        public HttpRequestHeader RequestHeader
        {
            get { return MessageHeader as HttpRequestHeader; }
        }

        public RequestMethods RequestMethod
        {
            get { return RequestHeader.MethodType; }
        }

        protected override HttpMessageHeader ReadHeader(Stream stream)
        {
            return new HttpRequestHeader(base.ReadHeader(stream));
        }
    }
}