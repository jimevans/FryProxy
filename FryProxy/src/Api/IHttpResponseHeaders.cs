using System;

namespace FryProxy.Api
{
    public interface IHttpResponseHeaders : IHttpMessageHeaders
    {
        String Age { get; set; }
        String Etag { get; set; }
        String Vary { get; set; }
        String Server { get; set; }
        String Location { get; set; }
        String RetryAfter { get; set; }
        String AcceptRanges { get; set; }
        String WWWAuthenticate { get; set; }
        String ProxyAuthenticate { get; set; }
        Boolean Chunked { get; }
    }
}