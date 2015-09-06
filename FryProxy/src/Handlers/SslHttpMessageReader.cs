using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using FryProxy.Headers;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    internal class SslHttpMessageReader : DefaultHttpMessageReader
    {
        private readonly X509Certificate _certificate;

        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        private readonly HttpResponseMessage _connectionEstablished = new ConnectionEstablishedMessage();

        public SslHttpMessageReader(X509Certificate certificate,
            RemoteCertificateValidationCallback certificateValidationCallback)
        {
            _certificate = certificate;
            _certificateValidationCallback = certificateValidationCallback;
        }

        public override void ReadHttpRequest(HttpRequestMessage message, Stream stream)
        {
            base.ReadHttpRequest(message, stream);

            if (message.RequestMethod != RequestMethods.CONNECT)
            {
                return;
            }

            var sslStream = new SslStream(stream, false, _certificateValidationCallback);

            try
            {
                _connectionEstablished.Write(stream);
                sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, false);
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex))
                {
                    throw new RequestAbortedException("Failed to establish SSL tunnel with client", ex);
                }
            }

            base.ReadHttpRequest(message, sslStream);
        }
    }
}