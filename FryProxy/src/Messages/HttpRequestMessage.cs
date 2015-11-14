using System;
using System.IO;
using System.Text.RegularExpressions;
using FryProxy.Api;
using FryProxy.Headers;

namespace FryProxy.Messages
{
    public class HttpRequestMessage : HttpMessage, IHttpRequestMessage
    {
        private static readonly Regex RequestLineRegex = new Regex(
            @"(?<method>\w+)\s(?<uri>.+)\sHTTP/(?<version>\d\.\d)", RegexOptions.Compiled
            );

        public HttpRequestMessage()
        {
        }

        public HttpRequestMessage(HttpRequestHeaders messageHeader) : base(messageHeader)
        {
        }

        public HttpRequestMessage(HttpRequestHeaders messageHeader, Stream body) : base(messageHeader, body)
        {
        }

        public HttpRequestHeaders RequestHeader
        {
            get { return MessageHeader as HttpRequestHeaders; }
        }

        public HttpMethods RequestMethod
        {
            get { return RequestHeader.MethodType; }
        }
        public HttpMethods MethodType
        {
            get
            {
                HttpMethods methodType;

                var rawHttpMethod = Method;

                if (HttpMethods.TryParse(Method, false, out methodType))
                {
                    return methodType;
                }

                throw new InvalidOperationException(string.Format("Unknown method type: [{0}]", rawHttpMethod));
            }

            set
            {
                Method = Enum.GetName(typeof(HttpMethods), value);
            }
        }
        protected override HttpMessageHeaders ReadHeader(Stream stream)
        {
            return new HttpRequestHeaders(base.ReadHeader(stream));
        }
    }
}