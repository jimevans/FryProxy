using System;

namespace FryProxy.Api
{
    public interface IGeneralHttpHeaders
    {
        /// <summary>
        ///     Cache-Control header value
        /// </summary>
        String CacheControl { get; set; }

        /// <summary>
        ///     Connection header value
        /// </summary>
        String Connection { get; set; }

        String ProxyConnection { get; set; }

        /// <summary>
        ///     Pragma header value
        /// </summary>
        String Pragma { get; set; }

        String TransferEncoding { get; set; }

        String Trailer { get; set; }
    }
}