using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FryProxy.IO
{
    public class DefaultRemoteConnector : IRemoteEndpointConnector
    {
        public Socket Connect(EndPoint remotEndPoint)
        {
            var socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket.Connect(remotEndPoint);

            return socket;
        }

        public Stream CreateStream(Socket socket)
        {
            return new NetworkStream(socket);
        }

        public Stream ConnectAndCreateStream(EndPoint remotEndPoint)
        {
            return CreateStream(Connect(remotEndPoint));
        }
    }
}