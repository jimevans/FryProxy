using FryProxy.Api;

namespace FryProxy.Headers
{
    public class HttpResponseHeader : HttpMessageHeaders, IHttpResponseHeaders
    {
        public const string AgeHeader = "Age";

        public const string EtagHeader = "Etag";

        public const string VaryHeader = "Vary";

        public const string ServerHeader = "Server";

        public const string LocationHeader = "Location";

        public const string RetryAfterHeader = "Retry-After";

        public const string AcceptRangesHeader = "Accept-Ranges";

        public const string WWWAuthenticateHeader = "WWW-Authenticate";
        public const string ProxyAuthenticateHeader = "Proxy-Authenticate";

        public HttpResponseHeader(HttpHeadersCollection headersCollection) : base(headersCollection)
        {
        }

        public HttpResponseHeader()
        {
        }

        public string Age
        {
            get { return HeadersCollection[AgeHeader]; }
            set { HeadersCollection[AgeHeader] = value; }
        }

        public string Etag
        {
            get { return HeadersCollection[EtagHeader]; }
            set { HeadersCollection[EtagHeader] = value; }
        }

        public string Vary
        {
            get { return HeadersCollection[VaryHeader]; }
            set { HeadersCollection[VaryHeader] = value; }
        }

        public string Server
        {
            get { return HeadersCollection[ServerHeader]; }
            set { HeadersCollection[ServerHeader] = value; }
        }

        public string Location
        {
            get { return HeadersCollection[LocationHeader]; }
            set { HeadersCollection[LocationHeader] = value; }
        }

        public string RetryAfter
        {
            get { return HeadersCollection[RetryAfterHeader]; }
            set { HeadersCollection[RetryAfterHeader] = value; }
        }

        public string AcceptRanges
        {
            get { return HeadersCollection[AcceptRangesHeader]; }
            set { HeadersCollection[AcceptRangesHeader] = value; }
        }

        public string WWWAuthenticate
        {
            get { return HeadersCollection[WWWAuthenticateHeader]; }
            set { HeadersCollection[WWWAuthenticateHeader] = value; }
        }

        public string ProxyAuthenticate
        {
            get { return HeadersCollection[ProxyAuthenticateHeader]; }
            set { HeadersCollection[ProxyAuthenticateHeader] = value; }
        }
    }
}