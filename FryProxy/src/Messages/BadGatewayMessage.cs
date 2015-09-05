namespace FryProxy.Messages
{
    public class BadGatewayMessage : EmptyResponseMessage
    {
        private BadGatewayMessage() : base(502, "Bad Gateway")
        {
        }
        
    }
}