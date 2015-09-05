using System;

namespace FryProxy.Headers
{
    public interface IHttpMessageHeader
    {
        Boolean Chunked { get; }
        
        String StartLine { get; }
        
        HttpHeaders Headers { get; }
        
        GeneralHeaders GeneralHeaders { get; }
        
        EntityHeaders EntityHeaders { get; }
    }
}