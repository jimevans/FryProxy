namespace FryProxy.Messages
{
    public class BadGatewayHttpResponseMessage : EmptyHttpResponseMessage
    {
        private static BadGatewayHttpResponseMessage _instance;

        private BadGatewayHttpResponseMessage() : base(502, "Bad Gateway")
        {
        }

        public static BadGatewayHttpResponseMessage Instance
        {
            get { return _instance ?? (_instance = new BadGatewayHttpResponseMessage()); }
        }
    }
}