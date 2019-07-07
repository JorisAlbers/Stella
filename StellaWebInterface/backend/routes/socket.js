/**
 *
 * Server listeners
 *  Incoming messages:
 *    * name = getAvailableStoryboards  = 1,  datatype = string (separated by ';')
 *
 *  Outgoing message:
 *    * name = getAvailableStoryboards  = 1
 *    * name = startStoryboard          = 2,  datatype = string (name of the storyboard)
 *    * name = startAnimation           = 3,  datatype = string (name of the animation)
 *    * name = startTempStoryboard      = 4,  datatype = string (jsonnated data see: './StellaServerConsole/Resources/Storyboards/StoryboardExample.yaml')
 *
 *  Client listeners
 *    Incoming messages
 *
 *    Outgoing messages
 *
 */

const fs = require("fs");
const net = require("net");

const config = require('../../backend/config/config');
const PackageProtocol = require("../service/packageProtocol");
const StringProtocol = require("../service/stringProtocol");
const SimpleImageToAnimationHelper = require("../service/simpleImageToRowHelper");


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
    this.stringProtocol = new StringProtocol();

    this.packageProtocol.messageArrived = (messageType, data) => {
      console.log("message protocol says it has a package - messageType: ", messageType, ", data: ", data.readUInt32LE());
      switch (messageType) {
        case 0:
          console.log('messageType received none');
          return null;
        case 1:
          this.stringProtocol.deserialize(data, this.packageProtocol.MAX_MESSAGE_SIZE);

          // Todo: Send result back to all clients listening to event 'returnAvailableStoryboards'
          if (this.stringProtocol.message !== null) {
            this.stringProtocol = new StringProtocol();
          }
          console.log('messageType received GetAvailableStoryboards');
          break;
      }
    };

    this._setServerListeners();
    this._setClientListeners();
  }

  getAvailableStoryboards() {
    this.serverSocket.write(this.packageProtocol.wrapGetAvailableStoryboardsMessage());
  };

  _setServerListeners() {
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
      // console.log("Server connection timeout");
    });
  };

  _setClientListeners() {
    this.clientSocket.on('connection', (socket) => {
      this.clientSocket.connectedClients.push(socket.id);

      console.log("Client connection connected");

      socket.on('getSavedLedMapping', () => {
        if (fs.existsSync('./savedData/savedLedMapping.json')) {
          const savedLedMapping = fs.readFileSync('./savedData/savedLedMapping.json');
          socket.emit('returnSavedLedMapping', JSON.parse(savedLedMapping))
        }
      });

      socket.on('setSavedLedMapping', (data) => {
        fs.writeFileSync('./savedData/savedLedMapping.json', JSON.stringify(data));
      });

      socket.on('getAvailableStoryboards', () => {
        this.getAvailableStoryboards();
        socket.emit('returnAvailableStoryboards', {})
      });

      socket.on('sendSingleFrame', (data) => {
        // todo: Send joris the animation
        const animation = new SimpleImageToAnimationHelper(data.imageFile, data.numberOfStripsPerRow).getAnimation();
        // fs.writeFileSync('./savedData/temp.json', JSON.stringify(animation));
      });

      socket.on('getStatus', () => {
        socket.emit('returnStatus', {
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
