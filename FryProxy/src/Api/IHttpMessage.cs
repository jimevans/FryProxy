using System;
using System.IO;
using FryProxy.Headers;

namespace FryProxy.Api
{
    public interface IHttpMessage : IDisposable
    {
        /// <summary>
        ///     HTTP protocol version
        /// </summary>
        string ProtocolVersion { get; set; }

        String StartLine { get; }
        
        Stream Body { get; set; }

        HttpHeadersCollection HeadersCollection { get; }
    }
}