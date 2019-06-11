import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import {socketConnect} from 'socket.io-react';

class HomePage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.state = {
      status: null,
      storyboards: null
    };

    this.props.socket.on('getStatus', (status) => this.setState({status}));
    this.props.socket.on('storyboards', (storyboards) => this.setState({storyboards}));
  }

  componentDidMount() {
    this.props.socket.emit('getStatus');
    this.props.socket.emit('getAvailableStoryboards');
  }

  render() {
    const {status, storyboards} = this.state;

    return <div >
      <Grid container spacing={3} direction={'row'} >
        <Grid xs={9} item >
          <h2>Available storyboards</h2>
          {storyboards &&
          <React.Fragment>

          </React.Fragment>}
        </Grid>
        <Grid xs={3} item >
          <h2>Application status</h2>
          {status &&
          <React.Fragment>
            <p>status.clientConnectedToBackend: {status.clientConnectedToBackend ? 'true' : 'false'}</p>
            <p>status.backendConnectedToServer: {status.backendConnectedToServer ? 'true' : 'false'}</p>
            <p>status.connectedClients: {status.connectedClients}</p>
          </React.Fragment>
          }
        </Grid>
      </Grid>

      <Grid container>
        <Grid item xs>

        </Grid>
      </Grid>

    </div>;
  }
}

export default socketConnect(HomePage);
