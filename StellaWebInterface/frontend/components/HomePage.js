import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Slider from "@material-ui/core/Slider";
import {socketConnect} from 'socket.io-react';
import Typography from "@material-ui/core/Typography";
import PlayIcon from "@material-ui/icons/PlayArrow";
import Stop from "@material-ui/icons/Stop";

class HomePage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.state = {
      status: null,
      storyboards: [],
      currentSelectedStoryboard: null,
      currentPlayingStoryboard: null
    };

    this.props.socket.on('returnStatus', (status) => this.setState({status}));
    this.props.socket.on('returnAvailableStoryboards', (data) => {
      console.log(data);
      this.setState({storyboards: data.storyboards})
    });
  }

  componentDidMount() {
    this.props.socket.emit('getStatus');
    this.props.socket.emit('getAvailableStoryboards');
  }

  render() {
    console.log(this.state);

    return <div>
      <Grid container direction={'row'}>
        <Grid xs item>
          <Typography variant={'h6'}>Available storyboards</Typography>
          {this.state.storyboards && this.state.storyboards.map((item, i) => (
            <div key={i} style={{cursor: 'pointer'}}>
              <Typography variant={'subtitle2'}>
                {this.state.currentPlayingStoryboard && item.Name === this.state.currentPlayingStoryboard.Name ?
                  <PlayIcon onClick={() => this.setState({currentPlayingStoryboard: null})}/> :
                  <Stop onClick={() => this.setState({currentPlayingStoryboard: item})}/>
                }
                <p style={{display: 'inline'}}
                   onClick={() => this.setState({currentSelectedStoryboard: item})}>{item.Name}</p>
              </Typography>
            </div>
          ))}
        </Grid>
        <Grid xs item>
          <Typography variant={'h6'}>Content of selected Storyboard</Typography>
          {this.state.currentSelectedStoryboard &&
          <Typography variant={'subtitle2'}>Storyboard name: {this.state.currentSelectedStoryboard.Name}</Typography>
          }
          {this.state.currentSelectedStoryboard && this.state.currentSelectedStoryboard.Animations.map((animation, i) => (
            <div key={i}>
              <Typography>Type: {animation.class}</Typography>
            </div>
          ))}
          {this.state.currentSelectedStoryboard && this.state.currentSelectedStoryboard &&
          <Stop style={{cursor: 'pointer'}}
                onClick={() => this.setState({currentPlayingStoryboard: null})}/>
          }
          {this.state.currentSelectedStoryboard && this.state.currentSelectedStoryboard &&
          <PlayIcon style={{cursor: 'pointer'}}
                    onClick={() => this.setState({currentPlayingStoryboard: this.state.currentSelectedStoryboard})}/>
          }
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
            {this.state.currentPlayingStoryboard && (
              this.state.currentPlayingStoryboard.Animations.map((storyboard, i) =>
                <div key={i} style={{display: 'inline-block', width: '500px'}}>
                  <Typography
                    id={'slider-label-name'}>{storyboard.class} [{storyboard.startIndex}, {storyboard.startIndex + storyboard.stripLength}]</Typography>
                  <div aria-labelledby="slider-label-name" style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      defaultValue={storyboard.frameWaitMs}
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
                      defaultValue={storyboard.rgbValues[0]}
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
                      defaultValue={storyboard.rgbValues[1]}
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
                      defaultValue={storyboard.rgbValues[2]}
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
