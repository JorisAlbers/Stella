using System;

namespace StellaLib.Network.Protocol
{
    // Original source: https://blog.stephencleary.com/2009/04/sample-code-length-prefix-message.html
    /// <summary>
    /// Maintains the necessary buffers for applying a length-prefix message framing protocol over a stream.
    /// </summary>
    /// <remarks>
    /// <para>Create one instance of this class for each incoming stream, and assign a handler to <see cref="MessageArrived"/>. As bytes arrive at the stream, pass them to <see cref="DataReceived"/>, which will invoke <see cref="MessageArrived"/> as necessary.</para>
    /// <para>If <see cref="DataReceived"/> raises <see cref="System.Net.ProtocolViolationException"/>, then the stream data should be considered invalid. After that point, no methods should be called on that <see cref="PacketProtocol"/> instance.</para>
    /// <para>This class uses a 4-byte signed integer length prefix, which allows for message sizes up to 2 GB. Keepalive messages are supported as messages with a length prefix of 0 and no message data.</para>
    /// <para>This is EXAMPLE CODE! It is not particularly efficient; in particular, if this class is rewritten so that a particular interface is used (e.g., Socket's IAsyncResult methods), some buffer copies become unnecessary and may be removed.</para>
    /// </remarks>
    public class PacketProtocol<TMessageType> where TMessageType : System.Enum
    {
        public const int HEADER_SIZE = sizeof(int) + sizeof(int); //  - length , - messageType

        /// <summary>
        /// Wraps a message. The wrapped message is ready to send to a stream.
        /// </summary>
        /// <remarks>
        /// <para>Generates a length prefix for the message and returns the combined length prefix and message.</para>
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="message">The message to send.</param>
        public static byte[] WrapMessage(TMessageType type, byte[] message)
        {
            // Get the message type prefix
            byte[] messageTypePrefix = BitConverter.GetBytes((int)(object)type);

            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(messageTypePrefix.Length + message.Length);

            // Concatenate the length prefix, the message prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + messageTypePrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            messageTypePrefix.CopyTo(ret, lengthPrefix.Length);
            message.CopyTo(ret, lengthPrefix.Length + messageTypePrefix.Length);

            return ret;
        }

        /// <summary>
        /// Wraps a keepalive (0-length) message. The wrapped message is ready to send to a stream.
        /// </summary>
        public static byte[] WrapKeepaliveMessage()
        {
            return BitConverter.GetBytes(0);
        }

        /// <summary>
        /// Initializes a new <see cref="PacketProtocol"/>, limiting message sizes to the BUFFER_SIZE.
        /// </summary>
        public PacketProtocol(int bufferSize)
        {
            _bufferSize = bufferSize;
            _maxMessageSize = bufferSize - HEADER_SIZE;


            // We allocate the buffer for receiving message lengths immediately
            this.lengthBuffer = new byte[sizeof(int)];
        }

        private readonly int _bufferSize;
        private readonly int _maxMessageSize;
        

        /// <summary>
        /// The buffer for the length prefix; this is always 4 bytes long.
        /// </summary>
        private byte[] lengthBuffer;

        /// <summary>
        /// The buffer for the message type; this is always 4 bytes long.
        /// </summary>
        private byte[] messageTypeBuffer;

        /// <summary>
        /// The buffer for the data; this is null if we are receiving the length prefix buffer.
        /// </summary>
        private byte[] dataBuffer;

        /// <summary>
        /// The number of bytes already read into the buffer (the length buffer if <see cref="dataBuffer"/> is null, otherwise the data buffer).
        /// </summary>
        private int bytesReceived;


        /// <summary>
        /// Indicates the completion of a message read from the stream.
        /// </summary>
        /// <remarks>
        /// <para>This event is invoked from within a call to <see cref="DataReceived"/>. Handlers for this event should not call <see cref="DataReceived"/>.</para>
        /// </remarks>
        public Action<TMessageType, byte[]> MessageArrived { get; set; }

        /// <summary>
        /// Indicates the retrieval of a KeepAlive message
        /// </summary>
        /// <remarks>
        /// </remarks>
        public Action KeepAliveArrived { get; set; }

        /// <summary>
        /// Notifies the <see cref="PacketProtocol"/> instance that incoming data has been received from the stream. This method will invoke <see cref="MessageArrived"/> as necessary.
        /// </summary>
        /// <remarks>
        /// <para>This method may invoke <see cref="MessageArrived"/> zero or more times.</para>
        /// <para>Zero-length receives are ignored. Many streams use a 0-length read to indicate the end of a stream, but <see cref="PacketProtocol"/> takes no action in this case.</para>
        /// </remarks>
        /// <param name="data">The data received from the stream. Cannot be null.</param>
        /// <exception cref="System.Net.ProtocolViolationException">If the data received is not a properly-formed message.</exception>
        public void DataReceived(byte[] data, int length)
        {
            // Process the incoming data in chunks, as the ReadCompleted requests it

            // Logically, we are satisfying read requests with the received data, instead of processing the
            //  incoming buffer looking for messages.

            int i = 0;
            while (i != length)
            {
                // Determine how many bytes we want to transfer to the buffer and transfer them
                int bytesAvailable = data.Length - i;
                if (this.dataBuffer != null)
                {
                    // We're reading into the data buffer
                    int bytesRequested = this.dataBuffer.Length - this.bytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, this.dataBuffer, this.bytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    // Notify "read completion"
                    this.ReadCompleted(bytesTransferred);
                }
                else if (this.messageTypeBuffer != null)
                {
                    // We're reading into the messageType buffer
                    int bytesRequested = this.messageTypeBuffer.Length - this.bytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, this.messageTypeBuffer, this.bytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    // Notify "read completion"
                    this.ReadCompleted(bytesTransferred);
                }
                else
                {
                    // We're reading into the length prefix buffer
                    int bytesRequested = this.lengthBuffer.Length - this.bytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, this.lengthBuffer, this.bytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    // Notify "read completion"
                    this.ReadCompleted(bytesTransferred);
                }
            }
        }

        /// <summary>
        /// Called when a read completes. Parses the received data and calls <see cref="MessageArrived"/> if necessary.
        /// </summary>
        /// <param name="count">The number of bytes read.</param>
        /// <exception cref="System.Net.ProtocolViolationException">If the data received is not a properly-formed message.</exception>
        private void ReadCompleted(int count)
        {
            // Get the number of bytes read into the buffer
            this.bytesReceived += count;

            if (this.messageTypeBuffer == null)
            {
                // We're currently receiving the length buffer
                if (this.bytesReceived != sizeof(int))
                {
                    // We haven't gotten all the length buffer yet: just wait for more data to arrive
                }
                else
                {
                    // We've gotten the length buffer
                    int length = BitConverter.ToInt32(this.lengthBuffer, 0);

                    // Sanity check for length < 0
                    if (length < 0)
                        throw new System.Net.ProtocolViolationException("Message length is less than zero");

                    // Another sanity check is needed here for very large packets, to prevent denial-of-service attacks
                    if (_bufferSize > 0 && length > _maxMessageSize)
                        throw new System.Net.ProtocolViolationException("Message length " + length.ToString(System.Globalization.CultureInfo.InvariantCulture) + " is larger than maximum message size " + _bufferSize.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    // Zero-length packets are allowed as keepalives
                    if (length == 0)
                    {
                        this.bytesReceived = 0;
                        if (this.KeepAliveArrived != null)
                            this.KeepAliveArrived();
                    }
                    else
                    {
                        // Create the message type buffer and start reading into it
                        this.messageTypeBuffer = new byte[sizeof(int)];
                        this.bytesReceived = 0;
                    }
                }
            }
            else if (this.dataBuffer == null)
            {
                // We're currently receiving the messageType buffer

                if (this.bytesReceived != sizeof(int))
                {
                    // We haven't gotten all the messageType buffer yet: just wait for more data to arrive
                }
                else
                {
                    // We've gotten the messageType buffer
                    int messageType = BitConverter.ToInt32(this.messageTypeBuffer, 0);

                    // Check if the message type exists
                    if (!Enum.IsDefined(typeof(TMessageType), messageType))
                    {
                        throw new System.Net.ProtocolViolationException($"Message type {messageType} is invalid ");
                    }

                    // Create the data buffer and start reading into it
                    int dataBufferLength = BitConverter.ToInt32(this.lengthBuffer, 0) - this.messageTypeBuffer.Length;
                    this.dataBuffer = new byte[dataBufferLength];
                    this.bytesReceived = 0;
                }
            }
            else
            {
                // We're receiving the data buffer
                if (this.bytesReceived != this.dataBuffer.Length)
                {
                    // We haven't gotten all the data buffer yet: just wait for more data to arrive
                }
                else
                {
                    // We've gotten an entire packet
                    if (this.MessageArrived != null)
                        this.MessageArrived((TMessageType)(object)BitConverter.ToInt32(this.messageTypeBuffer, 0), dataBuffer);

                    // Start reading the length buffer again
                    this.dataBuffer = null;
                    this.messageTypeBuffer = null;
                    this.bytesReceived = 0;
                }
            }
        }
    }
}