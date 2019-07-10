import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Slider from "@material-ui/core/Slider";
import {socketConnect} from 'socket.io-react';
import Typography from "@material-ui/core/Typography";

class HomePage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.state = {
      status: null,
      storyboards: []
    };

    this.props.socket.on('returnStatus', (status) => this.setState({status}));
    this.props.socket.on('returnAvailableStoryboards', (stringedStoryboards) => {
      console.log(stringedStoryboards);

      /*
      if (typeof stringedStoryboards === 'string') {
        const storyboards = stringedStoryboards.split(';');
        const result = [];
        for (let i = 0; i < storyboards.length; i++) {
          result.push({
            id: i,
            name: storyboards[i],
            speed: 10,
            brightness: 0,
            r: 0,
            g: 0,
            b: 0,
          })
        }
        return this.setState({storyboards: result});
      }*/
    });
  }

  componentDidMount() {
    this.props.socket.emit('getStatus');
    this.props.socket.emit('getAvailableStoryboards');
  }

  render() {
    return <div>
      <Grid container direction={'row'}>
        <Grid xs={9} item>
          <h2>Available storyboards</h2>
          {this.state.storyboards &&
          <React.Fragment>

          </React.Fragment>}
        </Grid>
        <Grid xs={3} item>
          <h2>Application status</h2>
          {this.state.status &&
          <React.Fragment>
            <p>clientConnectedToBackend: {this.state.status.clientConnectedToBackend ? 'true' : 'false'}</p>
            <p>backendConnectedToServer: {this.state.status.backendConnectedToServer ? 'true' : 'false'}</p>
            <p>connectedClients: {this.state.status.connectedClients}</p>
          </React.Fragment>
          }
        </Grid>
      </Grid>

      <Grid container direction={'row'}>
        <Grid item xs>
          <h2>Control panel</h2>
          <div style={{whiteSpace: 'nowrap', overflowX: 'scroll'}}>
            {this.state.storyboards[0] && (
              this.state.storyboards.map((storyboard) =>
                <div key={storyboard.id} style={{display: 'inline-block', width: '500px'}}>
                  <Typography id={'slider-label-name'}>{storyboard.name}</Typography>
                  <div aria-labelledby="slider-label-name" style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      defaultValue={storyboard.speed}
                      aria-labelledby="slider-label-speed"
                      min={5}
                      max={250}
                      marks={[{
                        value: 5,
                        label: '5',
                      }, {
                        value: 250,
                        label: '250',
                      }]}
                    />
                    <Typography id="slider-label-speed" gutterBottom>
                      Speed
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      defaultValue={storyboard.brightness}
                      aria-labelledby="slider-label-brightness"
                      step={0.1}
                      min={-1}
                      max={1}
                      marks={[{
                        value: -1,
                        label: '-1',
                      }, {
                        value: 0,
                        label: '0',
                      }, {
                        value: 1,
                        label: '+1',
                      }]}
                    />
                    <Typography id="slider-label-brightness" gutterBottom>
                      Brightness
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      defaultValue={storyboard.r}
                      aria-labelledby="slider-label-red"
                      step={0.1}
                      min={-1}
                      max={0}
                      marks={[{
                        value: -1,
                        label: '-1',
                      }, {
                        value: 0,
                        label: '0',
                      }]}
                    />
                    <Typography id="slider-label-red" gutterBottom>
                      Red
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      defaultValue={storyboard.g}
                      aria-labelledby="slider-label-green"
                      step={0.1}
                      min={-1}
                      max={0}
                      marks={[{
                        value: -1,
                        label: '-1',
                      }, {
                        value: 0,
                        label: '0',
                      }]}
                    />
                    <Typography id="slider-label-green" gutterBottom>
                      Green
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      defaultValue={storyboard.b}
                      aria-labelledby="slider-label-blue"
                      step={0.1}
                      min={-1}
                      max={0}
                      marks={[{
                        value: -1,
                        label: '-1',
                      }, {
                        value: 0,
                        label: '0',
                      }]}
                    />
                    <Typography id="slider-label-blue" gutterBottom>
                      Blue
                    </Typography>
                  </div>
                </div>)
            )}
          </div>
        </Grid>
      </Grid>

    </div>;
  }
}

export default socketConnect(HomePage);
