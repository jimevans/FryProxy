using System;
using FryProxy.Api;

namespace FryProxy.Headers {

    public sealed class EntityHeaders : IHttpEntityHeaders
    {

        public const String AllowHeader = "Allow";

        public const String ExpiresHeader = "Expires";

        public const String LastModifiedHeader = "Last-Modified";

        public const String ContentMD5Header = "Content-MD5";
        public const String ContentTypeHeader = "Content-Type";
        public const String ContentRangeHeader = "Content-Range";
        public const String ContentLengthHeader = "Content-Length";
        public const String ContentLanguageHeader = "Content-Language";
        public const String ContentLocationHeader = "Content-Location";
        public const String ContentEncodingHeader = "Content-Encoding";

        private readonly HttpHeadersCollection _headersCollection;

        public EntityHeaders(HttpHeadersCollection headersCollection) {
            _headersCollection = headersCollection;
        }

        public String Allow {
            get { return _headersCollection[AllowHeader]; }
            set { _headersCollection[AllowHeader] = value; }
        }

        public String Expires {
            get { return _headersCollection[ExpiresHeader]; }
            set { _headersCollection[ExpiresHeader] = value; }
        }

        public String LastModified {
            get { return _headersCollection[LastModifiedHeader]; }
            set { _headersCollection[LastModifiedHeader] = value; }
        }

        public String ContentMD5 {
            get { return _headersCollection[ContentMD5Header]; }
            set { _headersCollection[ContentMD5Header] = value; }
        }

        public String ContentType {
            get { return _headersCollection[ContentTypeHeader]; }
            set { _headersCollection[ContentTypeHeader] = value; }
        }

        public String ContentRange {
            get { return _headersCollection[ContentRangeHeader]; }
            set { _headersCollection[ContentRangeHeader] = value; }
        }

        public Int64? ContentLength {
            get {
                var contentLength = _headersCollection[ContentLengthHeader];

                if (contentLength != null) {
                    return Int64.Parse(contentLength);
                }

                return null;
            }
            set { _headersCollection[ContentLengthHeader] = value.HasValue ? value.Value.ToString() : null; }
        }

        public String ContentLanguage {
            get { return _headersCollection[ContentLanguageHeader]; }
            set { _headersCollection[ContentLanguageHeader] = value; }
        }

        public String ContentLocation {
            get { return _headersCollection[ContentLocationHeader]; }
            set { _headersCollection[ContentLocationHeader] = value; }
        }

        public String ContentEncoding {
            get { return _headersCollection[ContentEncodingHeader]; }
            set { _headersCollection[ContentEncodingHeader] = value; }
        }

    }

}