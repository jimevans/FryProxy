namespace FryProxy.Api
{
    /// <summary>
    ///     Contract for request HTTP message
    /// </summary>
    public interface IHttpRequestMessage : IHttpMessage
    {

        /// <summary>
        ///     HTTP method
        /// </summary>
        HttpMethods RequestMethod { get; set; }

        /// <summary>
        ///     Request method
        /// </summary>
        string RequestMethodRaw { get; set; }

        /// <summary>
        ///     Request path
        /// </summary>
        string RequestUri { get; set; }

        /// <summary>
        ///     Destination host
        /// </summary>
        string TargetHost { get; set; }

        /// <summary>
        ///     Request-specific HTTP headers
        /// </summary>
        IHttpRequestHeaders RequestHeaders { get; }
    }
}