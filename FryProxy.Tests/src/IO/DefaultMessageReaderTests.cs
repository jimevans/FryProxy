using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FryProxy.IO;
using FryProxy.Tests.Extensions;
using NUnit.Framework;

namespace FryProxy.Tests.IO
{
    public class DefaultMessageReaderTests
    {

        [Test]
        public void ShouldReadHttpRequestMessage()
        {
            var request = new DefaultMessageReader().ReadRequestMessage(
                new StringBuilder()
                    .AppendLine("GET / HTTP/1.1")
                    .AppendLine("Host: google.com")
                    .AppendLine("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                    .AppendLine("User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36")
                    .AppendLine()
                    .AppendLine()
                    .ToString()
                    .ToStream()
            );

            Assert.That(request, Is.Not.Null);

            Assert.That(request.RequestUri, Is.Not.Null);
            Assert.That(request.RequestUri.Host, Is.EqualTo("google.com"));
            Assert.That(request.RequestUri.PathAndQuery, Is.EqualTo("/"));

            Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(request.Version, Is.EqualTo(new Version(1, 1)));

            Assert.That(request.Headers, Is.Not.Null);
            Assert.That(request.Headers.Host, Is.EqualTo("google.com"));
            Assert.That(request.Headers.Accept, Is.Not.Null);
            Assert.That(request.Headers.UserAgent, Is.Not.Null);
        }

    }
}
