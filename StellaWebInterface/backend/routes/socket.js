const net = require("net");

const config = require('../../backend/config/config');
const PackageProtocol = require("../service/packageProtocol");

class Socket {
  constructor(server) {
    this.serverSocket = net.createConnection({
      host: config.stellaServerConsole.ip,
      port: config.stellaServerConsole.port
    }, () => {
      console.log('Connection local  address : ' + this.serverSocket.localAddress + ":" + this.serverSocket.localPort);
      console.log('Connection remote address : ' + this.serverSocket.remoteAddress + ":" + this.serverSocket.remotePort);
    });
    this.serverSocket.connected = false;
    this.serverSocket.setTimeout(1000);
    this.clientSocket = require('socket.io')(server);
    this.clientSocket.connectedClients = [];
    this.packageProtocol = new PackageProtocol();

    this.packageProtocol.messageArrived = (messageType, data) => {
      console.log("message protocol says it has a package - messageType: ", messageType, ", data: ", data.readUInt32LE());
      switch (messageType) {
        case 0:
          console.log('messageType received none');
          return null;
        case 1:
          console.log('messageType received GetAvailableStoryboards');
          break;
      }
    };

    this._setServerListeners();
    this._setClientListeners();
  }

  getAvailableStoryboards () {
    this.serverSocket.write(this.packageProtocol.wrapGetAvailableStoryboardsMessage());
  };

  _setServerListeners () {
    this.serverSocket.on('close', (hadError) => {
      this.serverSocket.connected = false;
      this.clientSocket.emit('status', {
        clientConnectedToBackend: true,
        backendConnectedToServer: this.serverSocket.connected,
        connectedClients: this.clientSocket.connectedClients.length,
      });
      console.log("Server connection close - hadError: ", hadError);
      setTimeout(() => {
        console.log("Server connection reconnecting");
        this.serverSocket.connect({host: config.stellaServerConsole.ip, port: config.stellaServerConsole.port})
      }, 1000);
    });

    this.serverSocket.on('connect', () => {
      this.serverSocket.connected = true;
      this.clientSocket.emit('status', {
        clientConnectedToBackend: true,
        backendConnectedToServer: this.serverSocket.connected,
        connectedClients: this.clientSocket.connectedClients.length,
      });
      console.log("Server connection connected");
    });

    this.serverSocket.on('data', (data) => {
      this.packageProtocol.dataReceived(data);
      // console.log("Server connection data - data: ", data);
    });

    this.serverSocket.on('drain', () => {
      console.log("Server connection drain");
    });

    this.serverSocket.on('end', () => {
      this.serverSocket.connected = false;
      this.clientSocket.emit('status', {
        clientConnectedToBackend: true,
        backendConnectedToServer: this.serverSocket.connected,
        connectedClients: this.clientSocket.connectedClients.length,
      });
      console.log("Server connection end");
    });

    this.serverSocket.on('error', (error) => {
      console.log("Server connection error - error: ", error);
    });

    this.serverSocket.on('lookup', (err, address, family, host) => {
      console.log("Server connection lookup - lookup: ", err, " - address: ", address, " - family: ", family, " - host: ", host);
    });

    this.serverSocket.on('ready', () => {
      console.log("Server connection ready");
    });

    this.serverSocket.on('timeout', () => {
      console.log("Server connection timeout");
    });
  };

  _setClientListeners() {
    this.clientSocket.on('connection', (socket) => {
      this.clientSocket.connectedClients.push(socket.id);
      this.getAvailableStoryboards();

      console.log("Client connection connected");

      socket.on('availableStoryboards', () => {
        // socket.getAvailableStoryboards();
        socket.emit('availableStoryboards', {})
      });

      socket.on('status', () => {
        socket.emit('status', {
          clientConnectedToBackend: true,
          backendConnectedToServer: this.serverSocket.connected,
          connectedClients: this.clientSocket.connectedClients.length,
        })
      });

      socket.on('disconnect', () => {
        this.clientSocket.connectedClients.splice(this.clientSocket.connectedClients.indexOf(socket.id), 1);
        console.log("Client connection disconnect");
      });
    });
  };
}

// exports
module.exports = Socket;
