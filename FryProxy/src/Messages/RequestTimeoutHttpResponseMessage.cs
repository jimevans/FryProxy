namespace FryProxy.Messages
{
    internal class RequestTimeoutHttpResponseMessage : EmptyHttpResponseMessage
    {
        private static RequestTimeoutHttpResponseMessage _instance;

        private RequestTimeoutHttpResponseMessage() : base(408, "Request Timeout")
        {
        }

        public static RequestTimeoutHttpResponseMessage Instance
        {
            get { return _instance ?? (_instance = new RequestTimeoutHttpResponseMessage()); }
        }
    }
}