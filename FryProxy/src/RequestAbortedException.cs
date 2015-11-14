using System;
using System.Text;
using FryProxy.Headers;
using FryProxy.Utils;

namespace FryProxy
{
    public class RequestAbortedException : Exception
    {
        public RequestAbortedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RequestAbortedException(
            String reason,
            Exception innerException,
            HttpRequestHeaders requestHeader = null,
            HttpResponseHeader responseHeader = null)
            : base(BuildErrorMessage(reason, requestHeader, responseHeader), innerException)
        {
        }

        public RequestAbortedException(
            String reason = null,
            HttpRequestHeaders requestHeader = null,
            HttpResponseHeader responseHeader = null)
            : base(BuildErrorMessage(reason, requestHeader, responseHeader))
        {
        }

        private static String BuildErrorMessage(
            String reason = null,
            HttpRequestHeaders requestHeader = null,
            HttpResponseHeader responseHeader = null)
        {
            var messageBuilder = new StringBuilder();

            if (String.IsNullOrWhiteSpace(reason))
            {
                messageBuilder.Append("Request Aborted.");
            }
            else
            {
                messageBuilder.AppendFormat("Request aborted due to [{0}].", reason);
            }

            messageBuilder.AppendLine();

            if (requestHeader != null)
            {
                messageBuilder.AppendLine("Request:");
                messageBuilder.WriteHttpTraceMessage(requestHeader);
            }

            if (responseHeader != null)
            {
                messageBuilder.AppendLine("Response:");
                messageBuilder.WriteHttpTraceMessage(responseHeader);
            }

            return messageBuilder.ToString();
        }
    }
}