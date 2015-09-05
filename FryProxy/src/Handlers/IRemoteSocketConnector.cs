using System;
using System.IO;
using System.Net.Sockets;
using FryProxy.Headers;

namespace FryProxy.Handlers
{
    public interface IRemoteSocketConnector
    {
        Tuple<Socket, Stream> EstablishConnection(HttpRequestHeader requestHeader);
    }
}