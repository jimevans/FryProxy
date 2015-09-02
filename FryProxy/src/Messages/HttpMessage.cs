using System;
using System.Diagnostics.Contracts;
using System.IO;
using FryProxy.Headers;
using FryProxy.IO;

namespace FryProxy.Messages
{
    public abstract class HttpMessage : HttpMessageHeader
    {
        protected HttpMessage(HttpMessageHeader header)
        {
            Contract.Requires<ArgumentNullException>(header != null, "header");

            Header = header;
        }

        protected HttpMessage(HttpMessageHeader header, Stream body)
        {
            Body = body;
            Header = header;
        }

        public HttpMessageHeader Header { get; private set; }

        public Stream Body { get; set; }

        public void WriteTo(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream");

            var writer = new HttpContentWriter(stream);

            writer.WriteHttpMessageHeader(Header.StartLine, Header.Headers.Lines);

            if (Body != null)
            {
                WriteBody(writer);    
            }
        }

        protected virtual void WriteBody(HttpContentWriter writer)
        {
            if (Header.Chunked)
            {
                writer.WriteChunckedHttpMessageBody(Body);
            }
            else if (Header.EntityHeaders.ContentLength.HasValue)
            {
                writer.WritePlainHttpMessageBody(Body, Header.EntityHeaders.ContentLength.Value);
            }
        }
    }
}