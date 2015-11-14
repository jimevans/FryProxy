using System;

namespace FryProxy.Api
{
    public interface ITransparentProxy : IHttpProxy
    {
        event EventHandler AuthenticatedAsClient;

        event EventHandler AuthenticatedAsServer;
    }
}