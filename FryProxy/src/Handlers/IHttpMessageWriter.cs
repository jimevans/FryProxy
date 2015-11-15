using System.IO;
using System.Net.Http;

namespace FryProxy.Handlers
{
    /// <summary>
    /// Define contract for serializing HTTP messages
    /// </summary>
    public interface IHttpMessageWriter
    {
        /// <summary>
        /// Serialize HTTP response to stream
        /// </summary>
        /// <param name="message">Message to serialize</param>
        /// <param name="stream">Target stream</param>
        void Write(HttpResponseMessage message, Stream stream);

        /// <summary>
        /// Serialize HTTP request to stream
        /// </summary>
        /// <param name="message">Message to serialize</param>
        /// <param name="stream">Target stream</param>
        void Write(HttpRequestMessage message, Stream stream);
    }
}