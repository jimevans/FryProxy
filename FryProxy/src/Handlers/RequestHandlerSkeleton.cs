using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using FryProxy.Utils;
using log4net;

namespace FryProxy.Handlers
{
    internal abstract class RequestHandlerSkeleton
    {
        private ILog _logger;

        protected ILog Logger
        {
            get { return LazyInitializer.EnsureInitialized(ref _logger, () => LogManager.GetLogger(GetType())); }
        }

        protected Boolean HandleSocketException(Exception ex)
        {
            if (ex.IsSocketException(SocketError.OperationAborted, SocketError.ConnectionReset))
            {
                throw new RequestAbortedException("Connection Closed", ex);
            }

            if (ex is EndOfStreamException)
            {
                throw new RequestAbortedException("Read Failed", ex);
            }

            if (ex.IsSocketException(SocketError.TimedOut))
            {
                throw new RequestAbortedException("Socket Timeout", ex);
            }

            return false;
        }
    }
}