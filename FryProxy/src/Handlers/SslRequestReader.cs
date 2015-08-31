using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using FryProxy.Headers;
using FryProxy.Writers;

namespace FryProxy.Handlers
{
    internal class SslRequestReader : DefaultRequestReader
    {
        private readonly X509Certificate _certificate;

        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        public SslRequestReader(X509Certificate certificate,
            RemoteCertificateValidationCallback certificateValidationCallback)
        {
            _certificate = certificate;
            _certificateValidationCallback = certificateValidationCallback;
        }

        public override Tuple<HttpRequestHeader, Stream> ReadHttpRequest(Stream requestStream)
        {
            Tuple<HttpRequestHeader, Stream> plainHttpRequest = base.ReadHttpRequest(requestStream);

            if (plainHttpRequest == null)
            {
                throw new RequestAbortedException("Read Failed");
            }

            if (plainHttpRequest.Item1.MethodType != RequestMethodTypes.CONNECT)
            {
                return plainHttpRequest;
            }

            var sslStream = new SslStream(requestStream, false, _certificateValidationCallback);

            try
            {
                new HttpResponseWriter(requestStream).WriteConnectionEstablished();
                sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, false);
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex, plainHttpRequest.Item1))
                {
                    throw new RequestAbortedException("Failed to establish SSL tunnel with client", ex);
                }
            }

            return base.ReadHttpRequest(sslStream);
        }
    }
}