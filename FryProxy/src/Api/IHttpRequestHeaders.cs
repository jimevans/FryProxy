using System;

namespace FryProxy.Api
{
    public interface IHttpRequestHeaders : IHttpMessageHeaders
    {
        /// <summary>
        ///     Host header value
        /// </summary>
        String Host { get; set; }

        /// <summary>
        ///     Referer header value
        /// </summary>
        String Referer { get; set; }

        String TE { get; set; }
        String Range { get; set; }
        String From { get; set; }
        String Expect { get; set; }
        String UserAgent { get; set; }
        String MaxForwards { get; set; }
        String Authorization { get; set; }
        String ProxyAuthorization { get; set; }
        String Accept { get; set; }
        String AcceptCharset { get; set; }
        String AcceptEncoding { get; set; }
        String AcceptLanguage { get; set; }
        String IfMatch { get; set; }
        String IfRange { get; set; }
        String IfNoneMatch { get; set; }
        String IfModifiedSince { get; set; }
        String IfUnmodifiedSince { get; set; }
    }
}