import React, { Component } from 'react';
import {MuiThemeProvider} from '@material-ui/core/styles/index';
// import {SocketProvider} from 'socket.io-react';
// import io from 'socket.io-client';

import './styles/styles.css';
import AppRouter from './routers/AppRouter';
import {theme} from './theme/theme';

// const socket = io.connect('http://192.168.2.15:3001');

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      // <SocketProvider socket={socket}>
      <MuiThemeProvider theme={theme}>
        <AppRouter/>
      </MuiThemeProvider>
      // </SocketProvider>
    )
  };
}
