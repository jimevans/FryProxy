using System.IO;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    /// <summary>
    /// Responsible for reading HTTP messages from from a given stream.
    /// </summary>
    public interface IHttpMessageReader
    {
        /// <summary>
        /// Read HTTP request message from given sream.
        /// </summary>
        /// <param name="stream">Stream containing HTTP request message</param>
        HttpRequestMessage ReadHttpRequest(Stream stream);

        /// <summary>
        /// Read HTTP response message from given stream.
        /// </summary>
        /// <param name="stream">Stream containing HTTP reponse message</param>
        HttpResponseMessage ReadHttpResponse(Stream stream);

    }
}