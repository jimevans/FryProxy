using System;
using System.IO;
using System.Net.Sockets;
using FryProxy.Headers;

namespace FryProxy.Handlers
{
    public interface IRemoteEndpointConnector
    {
        Tuple<Socket, Stream> EstablishConnection(HttpRequestHeaders requestHeader);
    }
}