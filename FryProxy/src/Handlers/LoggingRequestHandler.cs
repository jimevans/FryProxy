using System.Threading;
using log4net;

namespace FryProxy.Handlers
{
    internal abstract class LoggingRequestHandler
    {
        private ILog _logger;

        protected ILog Logger
        {
            get { return LazyInitializer.EnsureInitialized(ref _logger, () => LogManager.GetLogger(GetType())); }
        }
    }
}