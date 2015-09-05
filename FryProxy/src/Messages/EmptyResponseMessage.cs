using System;
using FryProxy.Headers;

namespace FryProxy.Messages
{
    public class EmptyResponseMessage : HttpResponseMessage
    {
        public EmptyResponseMessage(Int32 statusCode, String statusMessage)
            : base(new HttpResponseHeader(statusCode, statusMessage))
        {
        }
    }
}