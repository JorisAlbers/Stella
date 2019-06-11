const net = require("net");

const PackageProtocol = require("../service/packageProtocol");

function Socket(server) {
  const serverSocket = net.createConnection({host: '192.168.2.6', port: 20060}, () => {
    console.log('Connection local address : ' + serverSocket.localAddress + ":" + serverSocket.localPort);
    console.log('Connection remote address : ' + serverSocket.remoteAddress + ":" + serverSocket.remotePort);
  });
  serverSocket.connected = false;
  serverSocket.setTimeout(1000);
  const clientSocket = require('socket.io')(server);
  const packageProtocol = new PackageProtocol();

  packageProtocol.messageArrived = (messageType, data) => {
    console.log("message protocol says it has a package ", messageType);
    switch (messageType) {
      case 0:
        console.log('messageType received none');
        return null;
      case 1:
        console.log('messageType received GetAvailableStoryboards');
        break;
    }
  };

  this._setServerListeners(clientSocket, serverSocket, packageProtocol);
  this._setClientListeners(clientSocket, serverSocket, packageProtocol);
}

Socket.prototype.getAvailableStoryboards = (clientSocket, serverSocket, packageProtocol) => {
  serverSocket.write(packageProtocol.wrapGetAvailableStoryboardsMessage());
};

Socket.prototype._setServerListeners = (clientSocket, serverSocket, packageProtocol) => {
  serverSocket.on('close', (hadError) => {
    serverSocket.connected = false;
    console.log("Server connection close - hadError: ", hadError)
  });

  serverSocket.on('connect', () => {
    serverSocket.connected = true;
    console.log("Server connection connected");
  });

  serverSocket.on('data', (data) => {
    console.log("Server connection data - data: ", data);
  });

  serverSocket.on('drain', () => {
    console.log("Server connection drain");
  });

  serverSocket.on('end', () => {
    serverSocket.connected = false;
    console.log("Server connection end");
  });

  serverSocket.on('error', (error) => {
    console.log("Server connection error - error: ", error);
  });

  serverSocket.on('lookup', (err, address, family, host) => {
    console.log("Server connection lookup - lookup: ", err, " - address: ", address, " - family: ", family, " - host: ", host);
  });

  serverSocket.on('ready', () => {
    console.log("Server connection ready");
  });

  serverSocket.on('timeout', () => {
    console.log("Server connection timeout");
  });
};

Socket.prototype._setClientListeners = (clientSocket, serverSocket, packageProtocol) => {
  let clients = [];
  clientSocket.on('connection', (socket) => {
    clients.push(socket.id);
    console.log("Client connection connected");

    socket.on('storyboards', () => {
      socket.emit('storyboards', {})
    });

    socket.on('getStatus', () => {
      socket.emit('getStatus', {
        clientConnectedToBackend: true,
        backendConnectedToServer: serverSocket.connected,
        connectedClients: clients.length,
      })
    });

    socket.on('disconnect', () => {
      clients.splice(clients.indexOf(socket.id), 1);
      console.log("Client connection disconnect");
    });
  });
};

// exports
module.exports = Socket;
