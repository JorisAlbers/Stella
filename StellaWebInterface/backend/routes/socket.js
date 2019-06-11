const net = require("net");

const PackageProtocol = require("../service/packageProtocol");

function Socket(server) {
  const serverSocket = net.createConnection({host: '192.168.2.6', port: 20060}, () => {
    console.log('Connection local address : ' + serverSocket.localAddress + ":" + serverSocket.localPort);
    console.log('Connection remote address : ' + serverSocket.remoteAddress + ":" + serverSocket.remotePort);
  });
  serverSocket.setTimeout(1000);
  const clientSocket = require('socket.io')(server);
  const packageProtocol = new PackageProtocol();

  packageProtocol.messageArrived = (messageType, data) => {
    console.log("message protol says it has a package ", messageType);
    switch (messageType) {
      case 0:
        console.log('messageType received none');
        return null;
        break;
      case 1:
        console.log('messageType received GetAvailableStoryboards');
        break;
    }
  };

  this._setServerListeners(serverSocket, packageProtocol);
  this._setClientListeners(clientSocket);
}

Socket.prototype.getAvailableStoryboards = (serverSocket, packageProtocol) => {
  serverSocket.write(packageProtocol.wrapGetAvailableStoryboardsMessage());
};

Socket.prototype._setServerListeners = (serverSocket, packageProtocol) => {
  serverSocket.on('close', (hadError) => {
    console.log("Server connection close - hadError: ", hadError)
  });

  serverSocket.on('connect', () => {
    console.log("Server connection connected")
  });

  serverSocket.on('data', (data) => {
    console.log("Server connection data - data: ", data);
  });

  serverSocket.on('drain', () => {
    console.log("Server connection drain")
  });

  serverSocket.on('end', () => {
    console.log("Server connection end")
  });

  serverSocket.on('error', (error) => {
    console.log("Server connection error - error: ", error)
  });

  serverSocket.on('lookup', (err, address, family, host) => {
    console.log("Server connection lookup - lookup: ", err, " - address: ", address, " - family: ", family, " - host: ", host)
  });

  serverSocket.on('ready', () => {
    console.log("Server connection ready");
  });

  serverSocket.on('timeout', (data) => {
    console.log("Server connection timeout")
  });
};

Socket.prototype._setClientListeners = (clientSocket) => {
  clientSocket.on('connection', (socket) => {
    console.log("Client connection connected");

    socket.on('disconnect', () => {
      console.log("Client connection disconnect");
    });
  });
};

// exports
module.exports = Socket;
