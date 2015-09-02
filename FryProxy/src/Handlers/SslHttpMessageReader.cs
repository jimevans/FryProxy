using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using FryProxy.Headers;
using FryProxy.Writers;

namespace FryProxy.Handlers
{
    internal class SslHttpMessageReader : DefaultHttpMessageReader
    {
        private readonly X509Certificate _certificate;

        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        public SslHttpMessageReader(X509Certificate certificate,
            RemoteCertificateValidationCallback certificateValidationCallback)
        {
            _certificate = certificate;
            _certificateValidationCallback = certificateValidationCallback;
        }

        public override Tuple<HttpRequestHeader, Stream> ReadHttpRequest(Stream stream)
        {
            Tuple<HttpRequestHeader, Stream> plainHttpRequest = base.ReadHttpRequest(stream);

            if (plainHttpRequest == null)
            {
                throw new RequestAbortedException("Read Failed");
            }

            if (plainHttpRequest.Item1.MethodType != RequestMethodTypes.CONNECT)
            {
                return plainHttpRequest;
            }

            var sslStream = new SslStream(stream, false, _certificateValidationCallback);

            try
            {
                new HttpResponseWriter(stream).WriteConnectionEstablished();
                sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, false);
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex))
                {
                    throw new RequestAbortedException("Failed to establish SSL tunnel with client", ex);
                }
            }

            return base.ReadHttpRequest(sslStream);
        }
    }
}