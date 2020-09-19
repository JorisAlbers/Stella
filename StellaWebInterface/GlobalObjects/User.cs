using DotNetify;

namespace StellaWebInterface.GlobalObjects
{
    public class User
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string IpAddress { get; set; }

        public User(IConnectionContext connectionContext, string correlationId)
        {
            Id = connectionContext.ConnectionId;
            CorrelationId = correlationId;
            IpAddress = connectionContext.HttpConnection.RemoteIpAddress.ToString();
        }
    }
}
