using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using HttpRequestHeader = FryProxy.Headers.HttpRequestHeader;

namespace FryProxy.Handlers
{
    internal class DefaultSocketConnector : HandlerSkeleton, IRemoteSocketConnector
    {
        private static readonly Regex HostAndPortRegex = new Regex(@"(?<host>\w+):(?<port>\d+)");

        private readonly Int32 _defaultPort;

        private readonly TimeSpan _socketReadTimeout;
        private readonly TimeSpan _socketWriteTimeout;

        public DefaultSocketConnector(Int32 defaultPort, TimeSpan socketWriteTimeout, TimeSpan socketReadTimeout)
        {
            Contract.Requires<ArgumentOutOfRangeException>(defaultPort > IPEndPoint.MinPort&& defaultPort < IPEndPoint.MaxPort, "defaultPort");
            Contract.Requires<ArgumentNullException>(socketWriteTimeout != null, "socketWriteTimeout");
            Contract.Requires<ArgumentNullException>(socketReadTimeout != null, "socketReadTimeout");

            _defaultPort = defaultPort;
            _socketWriteTimeout = socketWriteTimeout;
            _socketReadTimeout = socketReadTimeout;
        }

        public Tuple<Socket, Stream> EstablishConnection(HttpRequestHeader requestHeader)
        {
            Contract.Requires<ArgumentNullException>(requestHeader != null, "requestHeader");

            DnsEndPoint endPoint;

            if (!TryBuildFromHost(requestHeader.Host, out endPoint) 
                && TryBuildFromUri(requestHeader.RequestURI, out endPoint))
            {
                throw new RequestAbortedException("Cannot determine remote endpoint", requestHeader);
            }

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = (Int32) _socketReadTimeout.TotalMilliseconds,
                SendTimeout = (Int32) _socketWriteTimeout.TotalMilliseconds
            };

            socket.Connect(endPoint.Host, endPoint.Port);

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Connection Established: {0}:{1}", endPoint.Host, endPoint.Port);
            }

            return Tuple.Create(socket, CreateStream(socket, endPoint));
        }

        protected virtual Stream CreateStream(Socket socket, DnsEndPoint endPoint)
        {
            return new NetworkStream(socket, true);
        }

        private static Boolean TryBuildFromUri(String uri, out DnsEndPoint endPoint)
        {
            Uri parsedUri;

            endPoint = Uri.TryCreate(uri, UriKind.Absolute, out parsedUri) 
                ? new DnsEndPoint(parsedUri.Host, parsedUri.Port, AddressFamily.InterNetwork) 
                : null;

            return endPoint != null;
        }

        private Boolean TryBuildFromHost(String hostHeader, out DnsEndPoint endPoint)
        {
            endPoint = null;

            if (String.IsNullOrWhiteSpace(hostHeader))
            {
                return false;
            }

            Match hostAndPortMatch = HostAndPortRegex.Match(hostHeader);

            if (hostAndPortMatch.Success)
            {
                endPoint = new DnsEndPoint(
                    hostAndPortMatch.Groups["host"].Value,
                    Int32.Parse(hostAndPortMatch.Groups["port"].Value),
                    AddressFamily.InterNetwork
                    );
            }
            else
            {
                endPoint = new DnsEndPoint(hostHeader, _defaultPort, AddressFamily.InterNetwork);
            }

            return true;
        }
    }
}