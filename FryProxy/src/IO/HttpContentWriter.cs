using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FryProxy.IO
{
    public class HttpContentWriter
    {
        private static readonly Byte[] LineTerminator = Encoding.ASCII.GetBytes("\r\n");

        private readonly Int32 _bufferSize;

        private readonly Stream _stream;

        public HttpContentWriter(Stream stream) : this(stream, 1024)
        {
        }


        public HttpContentWriter(Stream stream, Int32 bufferSize)
        {
            _bufferSize = bufferSize;
            _stream = stream;
        }

        public void WriteLineTerminator()
        {
            _stream.Write(LineTerminator, 0, LineTerminator.Length);
        }

        public void WriteLine(String content)
        {
            Byte[] contentAsBytes = Encoding.ASCII.GetBytes(content);
            _stream.Write(contentAsBytes, 0, contentAsBytes.Length);
            WriteLineTerminator();
        }

        public void WriteHttpMessageHeader(String firstLine, IEnumerable<String> headers)
        {
            WriteLine(firstLine);

            foreach (String headerLine in headers)
            {
                WriteLine(headerLine);
            }

            WriteLineTerminator();
        }

        public void WritePlainHttpMessageBody(Stream contentStream, Int64 contentLength)
        {
            var buffer = new Byte[_bufferSize];

            Int64 bytesRead = 0;

            while (bytesRead < contentLength)
            {
                int bytesCopied = contentStream.Read(buffer, 0,
                    (Int32) Math.Min(buffer.Length, contentLength - bytesRead));

                _stream.Write(buffer, 0, bytesCopied);

                bytesRead += bytesCopied;
            }

            WriteLineTerminator();
        }

        public void WriteChunckedHttpMessageBody(Stream contentStream)
        {
            var reader = new HttpHeaderReader(contentStream);

            for (int size = reader.ReadNextChunkSize(); size != 0; size = reader.ReadNextChunkSize())
            {
                WriteLine(size.ToString("X"));
                WritePlainHttpMessageBody(contentStream, size);
            }

            WriteLine("0");
            WriteLineTerminator();
        }
    }
}