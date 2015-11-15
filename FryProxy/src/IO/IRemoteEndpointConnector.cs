using System.Net;
using System.Net.Sockets;

namespace FryProxy.IO
{
    public interface IRemoteEndpointConnector
    {
        Socket Connect(EndPoint remotEndPoint);
    }
}