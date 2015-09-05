using System.IO;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    public interface IHttpMessageReader
    {
        void ReadHttpRequest(HttpRequestMessage message, Stream stream);

        void ReadHttpResponse(HttpResponseMessage message, Stream stream);

    }
}