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
 *    * name = getSavedLedMapping                    - Client asks for available save file of led mapping
 *    * name = setSavedLedMapping                    - Client set for available save file of led mapping
 *    * name = sendSingleFrame (Deprecated)          - Client sends a single frame that needs horizontal scanning
 *    * name = addNewAnimation                       - Client sends new data to generate a new animation
 *    * name = getStatus                             - Client asks for the status data
 *    * name = getCurrentPlayingStoryboard           - Client asks the current playing storyboard
 *    * name = setCurrentPlayingStoryboard           - Client sets the current playing storyboard
 *    * name = setFrameWaitMs,          index, value - Client sets the frame of given index
 *    * name = setRgbFade,              index, value - Client sets the rgbFade of given index
 *    * name = setBrightnessCorrection, index, value - Client sets the brightness of given index
 *    * name = getMasterControl                      - Client asks master control settings
 *
 *    Outgoing messages
 *    * name = returnSavedLedMapping                 - Return available save file of led mapping
 *    * name = returnAvailableStoryboards            - Return available storyboards
 *    * name = returnStatus                          - Return available status data
 *    * name = returnCurrentPlayingStoryboard        - Return the current playing storyboard
 *    * name = returnMasterControl                   - Return the current master control settings
 */

const fs = require("fs");
const net = require("net");
const siofu = require("socketio-file-upload");

const config = require('../../backend/config/config');
const PackageProtocol = require("../service/packageProtocol");
const StringProtocol = require("../service/stringProtocol");
const yamlToJson = require("../service/yamlConverter");
const SimpleImageToAnimationHelper = require("../service/simpleImageToRowHelper");
const convertMp4ToRow = require("../service/convertMp4ToRow");


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

    this.availableStoryboards = null;
    this.currentPlayingStoryboard = null;
    this.masterControl = {frameWaitMs: 10, brightness: 0, rgbValues: [0, 0, 0],};

    this.packageProtocol.messageArrived = (messageType, data) => {
      console.log("message protocol says it has a package - messageType: ", messageType, ", data: ", data.readUInt32LE());
      switch (messageType) {
        case 0:
          console.log('messageType received none');
          return null;
        case 1:
          this.stringProtocol.deserialize(data, this.packageProtocol.MAX_MESSAGE_SIZE);

          if (this.stringProtocol.message !== null) {

            // fs.writeFileSync('./savedData/temp1.yaml', this.stringProtocol.message, {encoding: 'ascii'});
            const yamlObject = yamlToJson(this.stringProtocol.message);
            // fs.writeFileSync('./savedData/temp2.json', JSON.stringify(yamlObject));

            this.availableStoryboards = yamlObject;
            this.clientSocket.emit('returnAvailableStoryboards', yamlObject);
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
      this.getAvailableStoryboards();
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
      const uploader = new siofu();
      uploader.dir = "./savedData";
      uploader.listen(socket);

      this.clientSocket.connectedClients.push(socket.id);

      console.log("Client connection connected");

      uploader.on('saved', event => {
        console.log(event.file);
      });
      uploader.on("error", (event) => {
        console.log("Error from uploader", event);
      });

      socket.on('getSavedLedMapping', () => {
        if (fs.existsSync('./savedData/savedLedMapping.json')) {
          const savedLedMapping = fs.readFileSync('./savedData/savedLedMapping.json', 'utf8');
          socket.emit('returnSavedLedMapping', JSON.parse(savedLedMapping))
        }
      });

      socket.on('setSavedLedMapping', (data) => {
        fs.writeFileSync('./savedData/savedLedMapping.json', JSON.stringify(data));
      });

      socket.on('setCurrentPlayingStoryboard', (dataString) => {
        const packages = this.stringProtocol.serialize(dataString, this.packageProtocol.MAX_MESSAGE_SIZE);
        for (let i = 0; i < packages.length; i++) {
          this.serverSocket.write(this.packageProtocol.wrapMessage(2, packages[i]));
        }

        // get the current playing storyboard from the list of available storyboards.
        this.currentPlayingStoryboard = (() => {
          for (let i = 0; i < this.availableStoryboards.storyboards.length; i++) {
            if (this.availableStoryboards.storyboards[i].Name === dataString) {
              return this.availableStoryboards.storyboards[i];
            }
          }
        })()
      });
      socket.on('getCurrentPlayingStoryboard', () => {
        if (this.currentPlayingStoryboard) {
          socket.emit('returnCurrentPlayingStoryboard', this.currentPlayingStoryboard);
        }
      });

      socket.on('setFrameWaitMs', (data) => {
        const buffer = new Buffer.alloc(8);
        buffer.writeInt32LE(data.index, 0);
        buffer.writeInt32LE(data.value, 4);
        console.log('setFrameWaitMs', data);
        this.serverSocket.write(this.packageProtocol.wrapMessage(6, buffer));

        if (data.index === -1) {
          this.masterControl.frameWaitMs = data.value;
        } else {
          this.currentPlayingStoryboard.Animations[data.index].frameWaitMs = data.value;
        }
      });
      socket.on('setRgbFade', (data) => {
        const buffer = new Buffer.alloc(16);
        buffer.writeInt32LE(data.index, 0);
        buffer.writeFloatLE(data.value[0], 4);
        buffer.writeFloatLE(data.value[1], 8);
        buffer.writeFloatLE(data.value[2], 12);
        console.log('setRgbFade', data);
        this.serverSocket.write(this.packageProtocol.wrapMessage(8, buffer));

        if (data.index === -1) {
          this.masterControl.rgbValues = data.value;
        } else {
          this.currentPlayingStoryboard.Animations[data.index].rgbValues = data.value
        }
      });
      socket.on('setBrightnessCorrection', (data) => {
        const buffer = new Buffer.alloc(8);
        buffer.writeInt32LE(data.index, 0);
        buffer.writeFloatLE(data.value, 4);
        console.log('setBrightnessCorrection', data);
        this.serverSocket.write(this.packageProtocol.wrapMessage(10, buffer));

        if (data.index === -1) {
          this.masterControl.brightness = data.value;
        } else {
          this.currentPlayingStoryboard.Animations[data.index].brightness = data.value;
        }
      });

      socket.on('getAvailableStoryboards', () => {
        if (this.availableStoryboards !== null) {
          socket.emit('returnAvailableStoryboards', this.availableStoryboards)
        } else {
          this.getAvailableStoryboards();
        }
      });

      socket.on('addNewAnimation', (data) => {
        let animation = null;
        switch (data.type) {
          case 'mappedVideoUpload':
            animation = new convertMp4ToRow(data).saveAnimation();
            break;
          default:
            break;
        }
        fs.writeFileSync('./savedData/temp.json', JSON.stringify(animation));
      });
      socket.on('sendSingleFrame', (data) => {
        // todo: Send joris the animation
        // const animation = new SimpleImageToAnimationHelper(data.imageFile, data.numberOfStripsPerRow).getAnimation();
        const animation = new SimpleImageToAnimationHelper(data.imageFile, data.numberOfStripsPerRow).saveAnimation();
        fs.writeFileSync('./savedData/temp.json', JSON.stringify(animation));
      });

      socket.on('getMasterControl', () => {
        socket.emit('returnMasterControl', this.masterControl)
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
