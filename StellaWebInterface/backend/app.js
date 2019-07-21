/** Module imports **/
const createError = require('http-errors');
const express = require('express');
const path = require('path');
const logger = require('morgan');
const http = require('http');
const debug = require('debug')('contento:server');
const cookieParser = require('cookie-parser')();
const expressSession = require('express-session');
const bodyParser = require('body-parser');
const siofu  = require("socketio-file-upload");

/** Relative imports **/
const config = require('./config/config');
const SocketClass = require('./routes/socket');
const routes = require('./routes/routes');

const port = normalizePort('3001');

const init = () => {
  const app = express();
  app.use(siofu.router);
  app.use(logger('dev'));
  app.use(express.json());
  app.use(express.urlencoded({extended: false}));
  routes.setRoutes(app);
  app.use(express.static(path.join(__dirname, 'public')));
  app.set('port', port);
  app.use(cookieParser);
  app.use(bodyParser.urlencoded({extended: true}));
  app.use(expressSession({secret: config.express.sessionSecret, resave: true, saveUninitialized: true}));

  // catch 404 and forward to error handler
  app.use(function (req, res, next) {
    next(createError(404));
  });

  // error handler
  app.use(function (err, req, res, next) {
    // set locals, only providing error in development
    res.locals.message = err.message;
    res.locals.error = req.app.get('env') === 'development' ? err : {};

    // send the error page
    res.status(err.status || 500);
    res.send({status: 'error'});
    throw err;
  });
  const server = http.createServer(app);
  const socket = new SocketClass(server);
  routes.setIO(socket);

  server.listen(port);
  console.log(`Listening on port: ${port}`);
  server.on('error', onError);
  server.on('listening', () => {
    onListening(server)
  });
};

function normalizePort(val) {
  const port = parseInt(val, 10);
  if (isNaN(port)) {
    return val;
  }

  if (port >= 0) {
    return port;
  }

  return false;
}

function onError(error) {
  if (error.syscall !== 'listen') {
    throw error;
  }

  const bind = typeof port === 'string'
    ? 'Pipe ' + port
    : 'Port ' + port;

  // handle specific listen errors with friendly messages
  switch (error.code) {
    case 'EACCES':
      console.error(bind + ' requires elevated privileges');
      process.exit(1);
      break;
    case 'EADDRINUSE':
      console.error(bind + ' is already in use');
      process.exit(1);
      break;
    default:
      throw error;
  }
}

function onListening(server) {
  const addr = server.address();
  const bind = typeof addr === 'string'
    ? 'pipe ' + addr
    : 'port ' + addr.port;
  debug('Listening on ' + bind);
}

init();