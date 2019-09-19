import React, { Component } from 'react';
import {MuiThemeProvider} from '@material-ui/core/styles/index';

import './styles/styles.css';
import AppRouter from './routers/AppRouter';
import {theme} from './theme/theme';

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <MuiThemeProvider theme={theme}>
        <AppRouter/>
      </MuiThemeProvider>
    )
  };
}
