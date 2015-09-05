namespace FryProxy.Messages
{
    public class ConnectionEstablishedMessage : EmptyResponseMessage
    {
        public ConnectionEstablishedMessage() : base(200, "Connection Established")
        {
        }
        
    }
}