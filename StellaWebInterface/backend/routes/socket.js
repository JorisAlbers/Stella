
// constructor
function Socket(server) {
  const socketIO = require('socket.io')(server);
  this._setListeners(socketIO);
}

Socket.prototype._setListeners = (socketIO) => {
  socketIO.on('connection', (socket) => {
    console.log("New connection has been made");

    socket.on('example', (data) => {
      console.log('data', data)
    });

    socket.on('disconnect', () => {
      console.log("Connection has been disconnected");
    });
  });
};

// exports
module.exports = Socket;
