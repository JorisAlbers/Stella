import React from 'react';
import {render} from 'react-dom';
import {MuiThemeProvider} from '@material-ui/core/styles/index';
import {SocketProvider} from 'socket.io-react';
import io from 'socket.io-client';
import dotnetify from 'dotnetify'

import './styles/styles.css';
import AppRouter from './routers/AppRouter';
import {theme} from './theme/theme';

const socket = io.connect('http://192.168.0.100:3001');

const App = () => (
  <MuiThemeProvider theme={theme}>
    <AppRouter/>
  </MuiThemeProvider>
);

render(
  <SocketProvider socket={socket}>
    <App/>
  </SocketProvider>, document.getElementById('app')
);
