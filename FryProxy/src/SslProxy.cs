using System;
using System.Diagnostics.Contracts;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using FryProxy.Handlers;

namespace FryProxy
{
    /// <summary>
    ///     HTTP proxy capable to intercept HTTPS requests.
    ///     Authenticates to client and server using provided <see cref="X509Certificate" />
    /// </summary>
    public class SslProxy : HttpProxy
    {
        private const Int32 DefaultSecureHttpPort = 443;

        private static readonly RemoteCertificateValidationCallback DefaultValidationCallback =
            (sender, certificate, chain, errors) => true;

        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        private readonly IHttpMessageReader _httpMessageReader;

        /// <summary>
        ///     Creates new instance of <see cref="HttpProxy" /> using provided default port and internal buffer size.
        /// </summary>
        /// <param name="defaultPort">
        ///     Port number on destination server which will be used if not specified in request
        /// </param>
        /// <param name="certificate">
        ///     Certificate used for server authentication
        /// </param>
        /// <param name="rcValidationCallback">
        ///     Used to validate destination server certificate. By default it accepts anything provided by server
        /// </param>
        public SslProxy(X509Certificate certificate, Int32 defaultPort,
            RemoteCertificateValidationCallback rcValidationCallback = null) : base(defaultPort)
        {
            Contract.Requires<ArgumentNullException>(certificate != null, "certificate");

            _certificateValidationCallback = rcValidationCallback ?? DefaultValidationCallback;

            _httpMessageReader = new SslHttpMessageReader(certificate, _certificateValidationCallback);
        }

        /// <summary>
        ///     Creates new instance of <see cref="HttpProxy" /> using default HTTP port (443).
        /// </summary>
        /// <param name="certificate">
        ///     Certificate used for server authentication
        /// </param>
        /// <param name="certificateValidationCallback">
        ///     Used to validate destination server certificate. By default it accepts anything provided by server
        /// </param>
        public SslProxy(X509Certificate certificate,
            RemoteCertificateValidationCallback certificateValidationCallback = null)
            : this(certificate, DefaultSecureHttpPort, certificateValidationCallback)
        {
        }

        protected override IHttpMessageReader HttpMessageReader
        {
            get { return _httpMessageReader; }
        }

        protected override IRemoteEndpointConnector RemoteEndpointConnector
        {
            get
            {
                return new SslEndpointConnector(
                    DefaultPort, 
                    ServerWriteTimeout, 
                    ServerReadTimeout,
                    _certificateValidationCallback);
            }
        }
    }
}