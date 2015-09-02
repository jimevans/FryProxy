using System;
using FryProxy.Headers;

namespace FryProxy.Messages
{
    public class EmptyHttpResponseMessage : HttpResponseMessage
    {
        public EmptyHttpResponseMessage(Int32 statusCode, String statusMessage)
            : base(new HttpResponseHeader(statusCode, statusMessage))
        {
        }
    }
}