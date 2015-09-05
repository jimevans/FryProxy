namespace FryProxy.Messages
{
    internal class RequestTimeoutMessage : EmptyResponseMessage
    {
        private RequestTimeoutMessage() : base(408, "Request Timeout")
        {
        }
        
    }
}