using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace FryProxy.Handlers
{
    internal class SslEndpointConnector : EndpointConnector
    {
        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        public SslEndpointConnector(Int32 defaultPort, TimeSpan socketWriteTimeout, TimeSpan socketReadTimeout, RemoteCertificateValidationCallback certificateValidationCallback) : base(defaultPort, socketWriteTimeout, socketReadTimeout)
        {
            _certificateValidationCallback = certificateValidationCallback;
        }

        protected override Stream CreateStream(Socket socket, DnsEndPoint endPoint)
        {
            var sslStream = new SslStream(base.CreateStream(socket, endPoint), false, _certificateValidationCallback);

            sslStream.AuthenticateAsClient(endPoint.Host);

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("SSL Connection Established: {0}:{1}", endPoint.Host, endPoint.Port);
            }

            return sslStream;
        }
    }
}