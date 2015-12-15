using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FryProxy.IO
{
    internal class DefaultMessageReader : IHttpMessageReader
    {
        private static readonly MediaTypeHeaderValue RequestMediaTypeHeader = MediaTypeHeaderValue.Parse("application/http; msgtype=request");
        private static readonly MediaTypeHeaderValue ResponseMediaTypeHeader = MediaTypeHeaderValue.Parse("application/http; msgtype=response");

        public HttpResponseMessage ReadResponseMessage(Stream stream)
        {
            var httpContent = new StreamContent(stream);
            httpContent.Headers.ContentType = ResponseMediaTypeHeader;

            return httpContent.ReadAsHttpResponseMessageAsync().Result;
        }

        public HttpRequestMessage ReadRequestMessage(Stream stream)
        {
            var httpContent = new StreamContent(stream);
            httpContent.Headers.ContentType = RequestMediaTypeHeader;

            return httpContent.ReadAsHttpRequestMessageAsync().Result;
        }
    }
}