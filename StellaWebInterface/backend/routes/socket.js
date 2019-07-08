/**
 * Server listeners
 *  Incoming messages:
 *    * name = getAvailableStoryboards  = 1,  datatype = string ( All preloaded storyboards names separated by ';')
 *    * name = getFrameWaitMs           = 5,  datatype = byte[] ( [int1,int2]                  int1 = animationIndex, int2 = frameWaitMs) frameWaitMs min value = 5
 *    * name = getRgbFade               = 7,  datatype = byte[] ( [int,float1,float2,float3]   int  = animationIndex, float1 = r fade correction, float2 = g fade correction, float3 = b fade correction.) fade correction in the range of -1 to 0)
 *    * name = getBrightnessCorrection  = 10, datatype = byte[] ( [int, float]                 int  = animationIndex, float = brightness correction.brigthness Correction in the range of -1 to 1
 *
 *  Outgoing message:
 *    * name = getAvailableStoryboards  = 1
 *    * name = startPreloadedStoryboard = 2,  datatype = string (name of the storyboard)
 *    * name = startStoryboard          = 3,  datatype = string (yaml, storyboard settings)
 *    * name = storeBitmap              = 4,  datatype = byte[] ( [int1, int2, int3, ascii name, flattened rgb array] int1 = name length, int2 = width, int3 = height , ascii name = has length of int1, rgb array = byte for each channel (flattened, so [r,g,b,r,g,b,r,g,b]). The name has to fit in the first package!)
 *    * name = getFrameWaitMs           = 5,  datatype = byte[] ( [int]                                               int  = animationIndex)
 *    * name = setFrameWaitMs           = 6,  datatype = byte[] ( [int1, int2]                                        int1 = animationIndex, int2 = frameWaitMs)
 *    * name = getRgbFade               = 7,  datatype = byte[] ( [int]                                               int  = animationIndex)
 *    * name = setRgbFade               = 8,  datatype = byte[] ( [int,float1,float2,float3]                          int  = animationIndex, float1 = r fade correction, float2 = g fade correction, float3 = b fade correction.)
 *    * name = getBrightnessCorrection  = 9,  datatype = byte[] ( [int]                                               int  = animationIndex
 *    * name = setBrightnessCorrection  = 10, datatype = byte[] ( [int, float])                                       int  = animationIndex, float = brigthnessCorrection
 *
 *  Client listeners
 *    Incoming messages:
 *    * name = getSavedLedMapping          - Client asks for available save file of led mapping
 *    * name = setSavedLedMapping          - Client set for available save file of led mapping
 *    * name = sendSingleFrame             - Client sends a single frame that needs horizontal scanning
 *    * name = getStatus                   - Client asks for the status data
 *
 *    Outgoing messages
 *    * name = returnSavedLedMapping       - Return available save file of led mapping
 *    * name = returnAvailableStoryboards  - Return available storyboards
 *    * name = returnStatus                - Return available status data
 */

const fs = require("fs");
const net = require("net");

const config = require('../../backend/config/config');
const PackageProtocol = require("../service/packageProtocol");
const StringProtocol = require("../service/stringProtocol");
const yamlToString = require("../service/yamlConverter");
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
          if (this.stringProtocol.message !== null) {
          // Todo: Send result back to all clients listening to event 'returnAvailableStoryboards'
            this.clientSocket.emit('returnAvailableStoryboards', this.stringProtocol.message);
            console.log(this.stringProtocol.message);
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
      console.log("Server connection close - hadError: ...");
      // console.log("Server connection close - hadError: ", hadError);
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
          const savedLedMapping = fs.readFileSync('./savedData/savedLedMapping.json', 'utf8');
          socket.emit('returnSavedLedMapping', JSON.parse(savedLedMapping))
        }
      });

      socket.on('setSavedLedMapping', (data) => {
        fs.writeFileSync('./savedData/savedLedMapping.json', JSON.stringify(data));
      });

      socket.on('getAvailableStoryboards', () => {
        const yamlFile = fs.readFileSync('./../../StellaServerConsole/Resources/Storyboards/SmallTest.yaml', 'utf8');
        const yamlObject = yamlToString(yamlFile);
        fs.writeFileSync('./savedData/temp.json', JSON.stringify(yamlObject));

        // this.getAvailableStoryboards();
        // socket.emit('returnAvailableStoryboards', {})
      });

      socket.on('sendSingleFrame', (data) => {
        // todo: Send joris the animation
        const animation = new SimpleImageToAnimationHelper(data.imageFile, data.numberOfStripsPerRow).getAnimation();
        fs.writeFileSync('./savedData/temp.json', JSON.stringify(animation));
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
