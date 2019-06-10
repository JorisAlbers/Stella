// app/routes.js
const indexController = require('../controllers/indexController');

module.exports = {
  _io: null,

  setRoutes(app) {
    app.get('/', indexController);
  },

  setIO(io) {
    this._io = io;
  },
};
