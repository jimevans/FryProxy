using System;
using System.IO;
using FryProxy.Headers;

namespace FryProxy.Handlers
{
    internal interface IHttpMessageReader
    {
        Tuple<HttpRequestHeader, Stream> ReadHttpRequest(Stream stream);

        Tuple<HttpResponseHeader, Stream> ReadHttpResponse(Stream stream);
    }
}