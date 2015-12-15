using System.IO;
using System.Text;

namespace FryProxy.Tests.Extensions
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string @string)
        {
            var memoryStream = new MemoryStream();

            var stringWriter = new StreamWriter(memoryStream, Encoding.ASCII);
            stringWriter.Write(@string);
            stringWriter.Flush();

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}