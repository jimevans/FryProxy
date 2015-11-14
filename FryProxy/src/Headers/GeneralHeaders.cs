using System;
using FryProxy.Api;

namespace FryProxy.Headers {

    public class GeneralHeaders : IGeneralHttpHeaders
    {

        public const String PragmaHeader = "Pragma";

        public const String ConnectionHeader = "Connection";

        public const String ProxyConnectionHeader = "Proxy-Connection";

        public const String CacheControlHeader = "Cache-Control";

        public const String TransferEncodingHeader = "Transfer-Encoding";

        public const String TrailerHeader = "Trailer";

        private readonly HttpHeadersCollection _httpHeadersCollection;

        public GeneralHeaders(HttpHeadersCollection headersCollection) {
            _httpHeadersCollection = headersCollection;
        }

        /// <summary>
        ///     Cache-Control header value
        /// </summary>
        public String CacheControl {
            get { return _httpHeadersCollection[CacheControlHeader]; }
            set { _httpHeadersCollection[CacheControlHeader] = value; }
        }

        /// <summary>
        ///     Connection header value
        /// </summary>
        public String Connection {
            get { return _httpHeadersCollection[ConnectionHeader]; }
            set { _httpHeadersCollection[ConnectionHeader] = value; }
        }

        public String ProxyConnection
        {
            get { return _httpHeadersCollection[ProxyConnectionHeader]; }
            set { _httpHeadersCollection[ProxyConnectionHeader] = value; }
        }

        /// <summary>
        ///     Pragma header value
        /// </summary>
        public String Pragma {
            get { return _httpHeadersCollection[PragmaHeader]; }
            set { _httpHeadersCollection[PragmaHeader] = value; }
        }

        public String TransferEncoding {
            get { return _httpHeadersCollection[TransferEncodingHeader]; }
            set { _httpHeadersCollection[TransferEncodingHeader] = value; }
        }

        public String Trailer {
            get { return _httpHeadersCollection[TrailerHeader]; }
            set { _httpHeadersCollection[TrailerHeader] = value; }
        }

    }

}