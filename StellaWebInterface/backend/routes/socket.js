const net = require("net");

const config = require('../../backend/config/config');
const PackageProtocol = require("../service/packageProtocol");

function Socket(server) {
  const serverSocket = net.createConnection({
    host: config.stellaServerConsole.ip,
    port: config.stellaServerConsole.port
  }, () => {
    console.log('Connection local  address : ' + serverSocket.localAddress + ":" + serverSocket.localPort);
    console.log('Connection remote address : ' + serverSocket.remoteAddress + ":" + serverSocket.remotePort);
  });
  serverSocket.connected = false;
  serverSocket.setTimeout(1000);
  const clientSocket = require('socket.io')(server);
  clientSocket.connectedClients = [];
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
  serverSocket.write(PackageProtocol.wrapGetAvailableStoryboardsMessage());
};

Socket.prototype._setServerListeners = (clientSocket, serverSocket, packageProtocol) => {
  serverSocket.on('close', (hadError) => {
    serverSocket.connected = false;
    clientSocket.emit('status', {
      clientConnectedToBackend: true,
      backendConnectedToServer: serverSocket.connected,
      connectedClients: clientSocket.connectedClients.length,
    });
    console.log("Server connection close - hadError: ", hadError);
    setTimeout(() => {
      console.log("Server connection reconnecting");
      serverSocket.connect({host: config.stellaServerConsole.ip, port: config.stellaServerConsole.port})
    }, 1000);
  });

  serverSocket.on('connect', () => {
    serverSocket.connected = true;
    clientSocket.emit('status', {
      clientConnectedToBackend: true,
      backendConnectedToServer: serverSocket.connected,
      connectedClients: clientSocket.connectedClients.length,
    });
    console.log("Server connection connected");
  });

  serverSocket.on('data', (data) => {
    packageProtocol.dataReceived(data);
    // console.log("Server connection data - data: ", data);
  });

  serverSocket.on('drain', () => {
    console.log("Server connection drain");
  });

  serverSocket.on('end', () => {
    serverSocket.connected = false;
    clientSocket.emit('status', {
      clientConnectedToBackend: true,
      backendConnectedToServer: serverSocket.connected,
      connectedClients: clientSocket.connectedClients.length,
    });
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
  clientSocket.on('connection', (socket) => {
    clientSocket.connectedClients.push(socket.id);
    console.log("Client connection connected");

    socket.on('availableStoryboards', () => {
      socket.emit('availableStoryboards', {})
    });

    socket.on('status', () => {
      socket.emit('status', {
        clientConnectedToBackend: true,
        backendConnectedToServer: serverSocket.connected,
        connectedClients: clientSocket.connectedClients.length,
      })
    });

    socket.on('disconnect', () => {
      clientSocket.connectedClients.splice(clientSocket.connectedClients.indexOf(socket.id), 1);
      console.log("Client connection disconnect");
    });
  });
};

// exports
module.exports = Socket;
