namespace StellaServer.Network
{
    public class ClientDisconnectedException : System.Exception
    {
        public ClientDisconnectedException(string id) { }
        public ClientDisconnectedException(string id, string message) : base(id) { }
        public ClientDisconnectedException(string id, string message, System.Exception inner) : base(id, inner) { }
        protected ClientDisconnectedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}