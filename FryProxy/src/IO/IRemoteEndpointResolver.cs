using System.Net;
using System.Net.Http;

namespace FryProxy.IO
{
    public interface IRemoteEndpointResolver
    {
        EndPoint ResolveFromHost(string host);

        EndPoint ResolveFromUri(string uri);

        EndPoint Resolve(HttpRequestMessage request);
    }
}