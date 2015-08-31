using System;
using System.IO;
using FryProxy.Headers;

namespace FryProxy.Handlers
{
    internal interface IHttpRequestReader
    {
        Tuple<HttpRequestHeader, Stream> ReadHttpRequest(Stream requestStream);
    }
}