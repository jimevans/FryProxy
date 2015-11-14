using System;
using System.IO;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestRepeater : IHttpMessageReader
    {
        private readonly int _retryCount;

        private readonly IHttpMessageReader _messageReader;

        private IRemoteEndpointConnector _connector;

        private IHttpMessageWriter _messageWriter;

        public RequestRepeater(
            IHttpMessageReader messageReader, 
            IRemoteEndpointConnector connector, 
            IHttpMessageWriter messageWriter,
            int retryCount = 0)
        {
            _messageReader = messageReader;
            _messageWriter = messageWriter;
            _connector = connector;

            _retryCount = retryCount;
        }

        public HttpRequestMessage ReadHttpRequest(Stream stream)
        {
            return _messageReader.ReadHttpRequest(stream);
        }

        public HttpResponseMessage ReadHttpResponse(Stream stream)
        {
            RequestAbortedException exception = null;

            for (var i = 0; i < _retryCount; i++)
            {
                try
                {
                    return _messageReader.ReadHttpResponse(stream);
                }
                catch (RequestAbortedException ex)
                {
                    if (i < _retryCount)
                    {
                        exception = ex;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new RequestAbortedException(
                String.Format("Failed to receive HTTP response after {0} attempts", _retryCount), 
                exception
            );
        }

    }
}