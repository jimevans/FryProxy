using System.IO;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    internal interface IHttpMessageWriter
    {
        void WriteHttpMessage(HttpMessage httpMessage, Stream outputStream);
    }
}