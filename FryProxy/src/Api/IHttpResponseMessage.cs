using System;

namespace FryProxy.Api
{
    public interface IHttpResponseMessage : IHttpMessage
    {
        Boolean IsRedirect { get; }

        /// <summary>
        ///     HTTP response status code
        /// </summary>
        Int32 StatusCode { get; set; }

        /// <summary>
        ///     HTTP respnse status message
        /// </summary>
        String Reason { get; set; }

        IHttpResponseHeaders ResponseHeaders { get; }

    }
}