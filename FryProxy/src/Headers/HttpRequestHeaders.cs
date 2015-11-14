using FryProxy.Api;

namespace FryProxy.Headers
{
    public class HttpRequestHeaders : HttpMessageHeaders, IHttpRequestHeaders
    {
        public const string TEHeader = "TE";

        public const string RangeHeader = "Range";

        public const string FromHeader = "From";
        public const string HostHeader = "Host";
        public const string RefererHeader = "Referer";
        public const string ExpectHeader = "Expect";

        public const string UserAgentHeader = "User-Agent";

        public const string MaxForwardsHeader = "Max-Forwards";

        public const string AuthorizationHeader = "Authorization";
        public const string ProxyAuthorizationHeader = "Proxy-Authorization";

        public const string AcceptHeader = "Accept";
        public const string AcceptCharsetHeader = "Accept-Charset";
        public const string AcceptEncodingHeader = "Accept-Encoding";
        public const string AcceptLanguageHeader = "Accept-Language";

        public const string IfMatchHeader = "If-Match";
        public const string IfRangeHeader = "If-Range";
        public const string IfNoneMatchHeader = "If-None-Match";
        public const string IfModifiedSinceHeader = "If-Modified-Since";
        public const string IfUnmodifiedSinceHeader = "If-Unmodified-Since";

        public HttpRequestHeaders(HttpHeadersCollection headersCollection) : base(headersCollection)
        {
        }

        public HttpRequestHeaders()
        {
        }

        /// <summary>
        ///     Host header value
        /// </summary>
        public string Host
        {
            get { return HeadersCollection[HostHeader]; }
            set { HeadersCollection[HostHeader] = value; }
        }

        /// <summary>
        ///     Referer header value
        /// </summary>
        public string Referer
        {
            get { return HeadersCollection[RefererHeader]; }
            set { HeadersCollection[RefererHeader] = value; }
        }

        public string TE
        {
            get { return HeadersCollection[TEHeader]; }
            set { HeadersCollection[TEHeader] = value; }
        }

        public string Range
        {
            get { return HeadersCollection[RangeHeader]; }
            set { HeadersCollection[RangeHeader] = value; }
        }

        public string From
        {
            get { return HeadersCollection[FromHeader]; }
            set { HeadersCollection[FromHeader] = value; }
        }

        public string Expect
        {
            get { return HeadersCollection[ExpectHeader]; }
            set { HeadersCollection[ExpectHeader] = value; }
        }

        public string UserAgent
        {
            get { return HeadersCollection[UserAgentHeader]; }
            set { HeadersCollection[UserAgentHeader] = value; }
        }

        public string MaxForwards
        {
            get { return HeadersCollection[MaxForwardsHeader]; }
            set { HeadersCollection[MaxForwardsHeader] = value; }
        }

        public string Authorization
        {
            get { return HeadersCollection[AuthorizationHeader]; }
            set { HeadersCollection[AuthorizationHeader] = value; }
        }

        public string ProxyAuthorization
        {
            get { return HeadersCollection[ProxyAuthorizationHeader]; }
            set { HeadersCollection[ProxyAuthorizationHeader] = value; }
        }

        public string Accept
        {
            get { return HeadersCollection[AcceptHeader]; }
            set { HeadersCollection[AcceptHeader] = value; }
        }

        public string AcceptCharset
        {
            get { return HeadersCollection[AcceptCharsetHeader]; }
            set { HeadersCollection[AcceptCharsetHeader] = value; }
        }

        public string AcceptEncoding
        {
            get { return HeadersCollection[AcceptEncodingHeader]; }
            set { HeadersCollection[AcceptEncodingHeader] = value; }
        }

        public string AcceptLanguage
        {
            get { return HeadersCollection[AcceptLanguageHeader]; }
            set { HeadersCollection[AcceptLanguageHeader] = value; }
        }

        public string IfMatch
        {
            get { return HeadersCollection[IfMatchHeader]; }
            set { HeadersCollection[IfMatchHeader] = value; }
        }

        public string IfRange
        {
            get { return HeadersCollection[IfRangeHeader]; }
            set { HeadersCollection[IfRangeHeader] = value; }
        }

        public string IfNoneMatch
        {
            get { return HeadersCollection[IfNoneMatchHeader]; }
            set { HeadersCollection[IfNoneMatchHeader] = value; }
        }

        public string IfModifiedSince
        {
            get { return HeadersCollection[IfModifiedSinceHeader]; }
            set { HeadersCollection[IfModifiedSinceHeader] = value; }
        }

        public string IfUnmodifiedSince
        {
            get { return HeadersCollection[IfUnmodifiedSinceHeader]; }
            set { HeadersCollection[IfUnmodifiedSinceHeader] = value; }
        }
    }
}