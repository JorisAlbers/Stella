class PackageProtocol {
  constructor() {
    this.lengthBuffer = new Buffer.alloc(4);
    this.messageTypeBuffer = null;
    this.dataBuffer = null;
    this.messageArrived = null;
    this.bytesReceived = 0;

    this.BUFFER_SIZE = 1024;
    this.HEADERSIZE = 8;
    this.MAX_MESSAGE_SIZE = this.BUFFER_SIZE - this.HEADERSIZE;
  }

  wrapGetAvailableStoryboardsMessage() {
    return new Buffer.from([5, 0, 0, 0, 1, 0, 0, 0, 1])
  };

  wrapMessage(messageType, data) {
    const buffer = new Buffer.alloc(data.length + this.HEADERSIZE);
    buffer.writeUInt32LE(data.length + 4, 0);
    buffer.writeUInt32LE(messageType, 4);
    data.copy(buffer, this.HEADERSIZE, 0, data.length);
    if (buffer.length > this.MAX_MESSAGE_SIZE) throw "buffer.length > this.MAX_MESSAGE_SIZE";
    return buffer
  }

  dataReceived(data) {
    // Process the incoming data in chunks, as the ReadCompleted requests it

    // Logically, we are satisfying read requests with the received data, instead of processing the
    // incoming buffer looking for messages.
    let i = 0;
    while (i !== data.length) {
      // Determine how many bytes we want to transfer to the buffer and transfer them
      const bytesAvailable = data.length - i;
      if (this.dataBuffer != null) {
        // We're reading into the data buffer
        let bytesRequested = this.dataBuffer.length - this.bytesReceived;

        // Copy the incoming bytes into the buffer
        let bytesTransferred = Math.min(bytesRequested, bytesAvailable);
        data.copy(this.dataBuffer, this.bytesReceived, i, i + bytesTransferred);
        // Array.Copy(data, i, this.dataBuffer, this.bytesReceived, bytesTransferred);
        i += bytesTransferred;

        // Notify "read completion"
        this.ReadCompleted(bytesTransferred);
      } else if (this.messageTypeBuffer != null) {
        // We're reading into the messageType buffer
        let bytesRequested = this.messageTypeBuffer.length - this.bytesReceived;

        // Copy the incoming bytes into the buffer
        let bytesTransferred = Math.min(bytesRequested, bytesAvailable);
        data.copy(this.messageTypeBuffer, this.bytesReceived, i, i + bytesTransferred);
        // Array.Copy(data, i, this.messageTypeBuffer, this.bytesReceived, bytesTransferred);
        i += bytesTransferred;

        // Notify "read completion"
        this.ReadCompleted(bytesTransferred);
      } else {
        // We're reading into the length prefix buffer
        let bytesRequested = this.lengthBuffer.length - this.bytesReceived;
        // Copy the incoming bytes into the buffer
        let bytesTransferred = Math.min(bytesRequested, bytesAvailable);
        data.copy(this.lengthBuffer, this.bytesReceived, i, i + bytesTransferred);
        // Array.Copy(data, i, this.lengthBuffer, this.bytesReceived, bytesTransferred);
        i += bytesTransferred;

        // Notify "read completion"
        this.ReadCompleted(bytesTransferred);
      }
    }
  };

  ReadCompleted(count) {
    // Get the number of bytes read into the buffer
    this.bytesReceived += count;
    if (this.messageTypeBuffer == null) {
      // We're currently receiving the length buffer
      if (this.bytesReceived !== 4) {
        // We haven't gotten all the length buffer yet: just wait for more data to arrive
      } else {
        // We've gotten the length buffer
        let length = this.lengthBuffer.readUInt32LE();

        // Sanity check for length < 0
        if (length < 0) throw new Error("Message length is less than zero");
        // Another sanity check is needed here for very large packets, to prevent denial-of-service attacks
        if (this.BUFFER_SIZE > 0 && length > this.BUFFER_SIZE) throw new Error("Message length " + length + " is larger than maximum message size " + this.BUFFER_SIZE);

        // Zero-length packets are allowed as keepalives
        if (length === 0) {
          this.bytesReceived = 0;
          // console.log("Server connection keep alive received")
        } else {
          // Create the message type buffer and start reading into it
          this.messageTypeBuffer = new Buffer.alloc(4);
          this.bytesReceived = 0;
        }
      }
    } else if (this.dataBuffer == null) {
      // We're currently receiving the messageType buffer
      if (this.bytesReceived !== 4) {
        // We haven't gotten all the messageType buffer yet: just wait for more data to arrive
      } else {
        // We've gotten the messageType buffer
        let messageType = this.messageTypeBuffer.readUInt32LE();

        // TODO validate message type
        // Check if the message type exists
        // if (!Enum.IsDefined(typeof (TMessageType), messageType)) {
        //   throw new System.Net.ProtocolViolationException($
        //   "Message type {messageType} is invalid "
        // );
        // }

        // Create the data buffer and start reading into it
        let dataBufferLength = this.lengthBuffer.readUInt32LE() - this.messageTypeBuffer.length;
        this.dataBuffer = new Buffer.alloc(dataBufferLength);
        this.bytesReceived = 0;
      }
    } else {
      // We're receiving the data buffer
      if (this.bytesReceived !== this.dataBuffer.length) {
        // We haven't gotten all the data buffer yet: just wait for more data to arrive
      } else {
        // We've gotten an entire packet
        if (this.messageArrived != null) this.messageArrived(this.messageTypeBuffer.readUInt32LE(), this.dataBuffer);

        // Start reading the length buffer again
        this.dataBuffer = null;
        this.messageTypeBuffer = null;
        this.bytesReceived = 0;
      }
    }
  };
}

module.exports = PackageProtocol;
