using System;

namespace FryProxy.Api
{
    public interface IHttpEntityHeaders
    {
        String Allow { get; set; }
        String Expires { get; set; }
        String LastModified { get; set; }
        String ContentMD5 { get; set; }
        String ContentType { get; set; }
        String ContentRange { get; set; }
        Int64? ContentLength { get; set; }
        String ContentLanguage { get; set; }
        String ContentLocation { get; set; }
        String ContentEncoding { get; set; }
    }
}