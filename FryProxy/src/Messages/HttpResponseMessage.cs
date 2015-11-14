using System;
using System.IO;
using System.Text.RegularExpressions;
using FryProxy.Api;
using FryProxy.Headers;
using FryProxy.IO;

namespace FryProxy.Messages
{
    public class HttpResponseMessage : HttpMessage, IHttpResponseMessage
    {
        private static readonly Regex ResponseLineRegex = new Regex(
            @"HTTP/(?<version>\d\.\d)\s(?<status>\d{3})\s(?<reason>.*)", RegexOptions.Compiled
            );

        public HttpResponseMessage()
        {
        }

        public HttpResponseMessage(HttpResponseHeader messageHeader) : base(messageHeader)
        {
        }

        public HttpResponseMessage(HttpResponseHeader messageHeader, Stream body) : base(messageHeader, body)
        {
        }

        /// <summary>
        ///     First line of HTTP response message
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     If Status-Line is invalid
        /// </exception>
        public override sealed String StartLine
        {
            get { return String.Format("HTTP/{0} {1} {2}", Version, StatusCode, Reason); }

            set
            {
                var match = ResponseLineRegex.Match(value);

                if (!match.Success)
                {
                    throw new ArgumentException("Ivalid Response-Line", "value");
                }

                Reason = match.Groups["reason"].Value;
                Version = match.Groups["version"].Value;
                StatusCode = Int32.Parse(match.Groups["status"].Value);

                base.StartLine = value;
            }
        }

        public HttpResponseHeader ResponseHeader
        {
            get { return MessageHeader as HttpResponseHeader; }
        }

        public Boolean IsRedirect
        {
            get
            {
                return ResponseHeader != null
                       && (ResponseHeader.StatusCode >= 300 || ResponseHeader.StatusCode < 400)
                       && !String.IsNullOrEmpty(ResponseHeader.Location);
            }
        }

        protected override void WriteBody(HttpContentWriter writer)
        {
            if (IsRedirect)
            {
                return;
            }

            base.WriteBody(writer);
        }

        protected override HttpMessageHeaders ReadHeader(Stream stream)
        {
            return new HttpResponseHeader(base.ReadHeader(stream));
        }
    }
}