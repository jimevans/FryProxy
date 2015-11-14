using System;

namespace FryProxy.Api
{
    public interface ITunnelingProxy : IHttpProxy
    {
        event EventHandler SslTunnelRequested;

        event EventHandler SslTunnelCreated;
    }
}