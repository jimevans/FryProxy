using System;

namespace FryProxy.Api
{
    public interface IHttpMessageHeaders: IGeneralHttpHeaders, IHttpEntityHeaders
    {

        Boolean Chunked { get; }
    }
}