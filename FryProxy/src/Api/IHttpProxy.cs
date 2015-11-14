using System.Net;
using System.Net.Sockets;
using FryProxy.Messages;

namespace FryProxy.Api
{
    public delegate void ConnectionAccepted(IPEndPoint endPoint);

    public delegate void RequestReceived(HttpRequestMessage requestMessage);

    public delegate void ResponseReceived(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);

    public delegate void ResponseSent(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);

    public interface IHttpProxy
    {
        event ConnectionAccepted ConnectionAcceped;

        event RequestReceived RequestReceived;

        event ResponseReceived ResponseReceived;

        event ResponseSent ResponseSent;

        void AcceptSocket(Socket socket);
    }
}