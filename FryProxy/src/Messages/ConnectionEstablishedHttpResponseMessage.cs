namespace FryProxy.Messages
{
    public class ConnectionEstablishedHttpResponseMessage : EmptyHttpResponseMessage
    {
        private static ConnectionEstablishedHttpResponseMessage _instance;

        public ConnectionEstablishedHttpResponseMessage() : base(200, "Connection Established")
        {
        }

        public static ConnectionEstablishedHttpResponseMessage Instance
        {
            get { return _instance ?? (_instance = new ConnectionEstablishedHttpResponseMessage()); }
        }
    }
}