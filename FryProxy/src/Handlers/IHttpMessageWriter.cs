using System.IO;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    public interface IHttpMessageWriter
    {
        void WriteHttpMessage(HttpMessage httpMessage, Stream outputStream);
    }
}