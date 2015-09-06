using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    public class RetryingMessageReader :IHttpMessageReader
    {
        private readonly Int32 _responseReadRetryCount;

        private readonly IHttpMessageReader _wrappedReader;

        public RetryingMessageReader(IHttpMessageReader wrappedReader, Int32 responseReadRetryCount = 0)
        {
            _wrappedReader = wrappedReader;
            _responseReadRetryCount = responseReadRetryCount;
        }

        public void ReadHttpRequest(HttpRequestMessage message, Stream stream)
        {
            _wrappedReader.ReadHttpRequest(message, stream);
        }

        public void ReadHttpResponse(HttpResponseMessage message, Stream stream)
        {
            for (var i = _responseReadRetryCount; i >= 0; i--)
            {
                try
                {
                    _wrappedReader.ReadHttpResponse(message, stream);
                }
                catch (RequestAbortedException)
                {
                    if (i < 0)
                    {
                        throw;
                    }

                    continue;
                }

                break;
            }
        }
    }
}
