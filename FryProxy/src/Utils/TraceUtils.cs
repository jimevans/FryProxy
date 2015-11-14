using System;
using System.Linq;
using System.Text;
using FryProxy.Headers;

namespace FryProxy.Utils
{
    public static class TraceUtils
    {
        public static void WriteHttpTraceMessage(this StringBuilder stringBuilder, HttpMessageHeaders messageHeader)
        {
            if (messageHeader == null)
            {
                return;
            }

            stringBuilder
                .AppendFormat("StartLine: {0}", messageHeader.StartLine)
                .AppendLine();

            var headers = messageHeader.HeadersCollection.Raw.ToList();

            if (headers.Count == 0)
            {
                return;
            }

            stringBuilder.AppendLine("Headers:");

            foreach (var header in headers)
            {
                stringBuilder
                    .AppendFormat("    {0}", header)
                    .AppendLine();
            }
        }

        public static String GetHttpTraceMessage(HttpMessageHeaders header)
        {
            if (header == null)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();

            WriteHttpTraceMessage(sb, header);

            return sb.ToString();
        }
    }
}