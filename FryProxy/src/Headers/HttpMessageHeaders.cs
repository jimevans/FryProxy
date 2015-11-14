using System;
using System.Diagnostics.Contracts;
using System.Text;
using FryProxy.Api;

namespace FryProxy.Headers
{
    public class HttpMessageHeaders : IHttpMessageHeaders
    {
        private const string ChunkedTransferEncoding = "chunked";

        private readonly EntityHeaders _entityHeaders;

        private readonly GeneralHeaders _generalHeaders;

        protected readonly HttpHeadersCollection HeadersCollection;

        public HttpMessageHeaders(HttpHeadersCollection headersCollection)
        {
            Contract.Requires<ArgumentNullException>(headersCollection != null, "headers");

            _generalHeaders = new GeneralHeaders(headersCollection);
            _entityHeaders = new EntityHeaders(headersCollection);
        }

        public HttpMessageHeaders() : this(new HttpHeadersCollection())
        {
        }

        public bool Chunked
        {
            get
            {
                if (string.IsNullOrEmpty(TransferEncoding))
                {
                    return false;
                }

                return (TransferEncoding).Contains(ChunkedTransferEncoding);
            }
        }

        public string CacheControl
        {
            get { return _generalHeaders.CacheControl; }
            set { _generalHeaders.CacheControl = value; }
        }

        public string Connection
        {
            get { return _generalHeaders.Connection; }
            set { _generalHeaders.Connection = value; }
        }

        public string ProxyConnection
        {
            get { return _generalHeaders.ProxyConnection; }
            set { _generalHeaders.ProxyConnection = value; }
        }

        public string Pragma
        {
            get { return _generalHeaders.Pragma; }
            set { _generalHeaders.Pragma = value; }
        }

        public string TransferEncoding
        {
            get { return _generalHeaders.TransferEncoding; }
            set { _generalHeaders.TransferEncoding = value; }
        }

        public string Trailer
        {
            get { return _generalHeaders.Trailer; }
            set { _generalHeaders.Trailer = value; }
        }

        public string Allow
        {
            get { return _entityHeaders.Allow; }
            set { _entityHeaders.Allow = value; }
        }

        public string Expires
        {
            get { return _entityHeaders.Expires; }
            set { _entityHeaders.Expires = value; }
        }

        public string LastModified
        {
            get { return _entityHeaders.LastModified; }
            set { _entityHeaders.LastModified = value; }
        }

        public string ContentMD5
        {
            get { return _entityHeaders.ContentMD5; }
            set { _entityHeaders.ContentMD5 = value; }
        }

        public string ContentType
        {
            get { return _entityHeaders.ContentType; }
            set { _entityHeaders.ContentType = value; }
        }

        public string ContentRange
        {
            get { return _entityHeaders.ContentRange; }
            set { _entityHeaders.ContentRange = value; }
        }

        public long? ContentLength
        {
            get { return _entityHeaders.ContentLength; }
            set { _entityHeaders.ContentLength = value; }
        }

        public string ContentLanguage
        {
            get { return _entityHeaders.ContentLanguage; }
            set { _entityHeaders.ContentLanguage = value; }
        }

        public string ContentLocation
        {
            get { return _entityHeaders.ContentLocation; }
            set { _entityHeaders.ContentLocation = value; }
        }

        public string ContentEncoding
        {
            get { return _entityHeaders.ContentEncoding; }
            set { _entityHeaders.ContentEncoding = value; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var header in HeadersCollection.Raw)
            {
                sb.AppendLine(header);
            }

            return sb.ToString();
        }
    }
}