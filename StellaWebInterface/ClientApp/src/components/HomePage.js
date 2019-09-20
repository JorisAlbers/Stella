import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Slider from "@material-ui/core/Slider";
// import {socketConnect} from 'socket.io-react';
import Typography from "@material-ui/core/Typography";
import PlayIcon from "@material-ui/icons/PlayArrow";
import Stop from "@material-ui/icons/Stop";
import Status from "./Blocks/Status";

class HomePage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.state = {
      storyboards: [],
      currentSelectedStoryboard: null,
      currentPlayingStoryboard: null,
      masterControl: {frameWaitMs: 10, brightness: 0, rgbValues: [0, 0, 0],},
    };

    // this.props.socket.on('returnAvailableStoryboards', (data) => {
    //   this.setState({storyboards: data.storyboards})
    // });
    // this.props.socket.on('returnCurrentPlayingStoryboard', currentPlayingStoryboard => {
    //   this.setState({currentPlayingStoryboard})
    // });
    // this.props.socket.on('returnMasterControl', masterControl => {
    //   this.setState({masterControl})
    // })
  }

  componentDidMount() {
    // this.props.socket.emit('getAvailableStoryboards');
    // this.props.socket.emit('getCurrentPlayingStoryboard');
    // this.props.socket.emit('getMasterControl');
  }

  render() {
    return <div>
      <Grid container direction={'row'}>
        <Grid xs item style={{overflowX: 'auto', maxHeight: '500px'}}>
          <Typography variant={'h6'}>Available storyboards</Typography>
          {this.state.storyboards && this.state.storyboards.map((item, i) => (
            <div key={i} style={{cursor: 'pointer'}}>
              <Typography variant={'subtitle2'}>
                {this.state.currentPlayingStoryboard && item.Name === this.state.currentPlayingStoryboard.Name ?
                  <Stop onClick={() => {
                    this.setState({currentPlayingStoryboard: null});
                  }}/>
                  :
                  <PlayIcon onClick={() => {
                    // this.props.socket.emit('setCurrentPlayingStoryboard', item.Name);
                    this.setState({currentPlayingStoryboard: item});
                  }}/>
                }
                <p style={{display: 'inline'}}
                   onClick={() => this.setState({currentSelectedStoryboard: item})}>{item.Name}</p>
              </Typography>
            </div>
          ))}
        </Grid>
        <Grid xs item style={{overflowX: 'auto', maxHeight: '-webkit-fill-available'}}>
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
                onClick={() => {
                  this.setState({currentPlayingStoryboard: null});
                }}/>
          }
          {this.state.currentSelectedStoryboard && this.state.currentSelectedStoryboard &&
          <PlayIcon style={{cursor: 'pointer'}} onClick={() => {
            // this.props.socket.emit('setCurrentPlayingStoryboard', this.state.currentSelectedStoryboard.Name);
            this.setState({currentPlayingStoryboard: this.state.currentSelectedStoryboard});
          }}/>
          }
        </Grid>

        <Grid xs item style={{padding: 0}}>
          <Grid container direction={'column'}>
            <Status/>
            <Grid xs item>
              <Typography variant={'h6'}>Master control</Typography>
              <div>
                <div aria-labelledby="slider-label-name" style={{float: 'left', display: 'inline', width: '100px'}}>
                  <Slider
                    style={{height: '100px', marginTop: '10px'}}
                    orientation="vertical"
                    value={this.state.masterControl.frameWaitMs}
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
                    onChange={(e, value) => {
                      const masterControl = {...this.state.masterControl};
                      masterControl.frameWaitMs = value;
                      // this.props.socket.emit('setFrameWaitMs', {index: -1, value: masterControl.frameWaitMs});
                      this.setState({masterControl})
                    }}
                  />
                  <Typography id="slider-label-speed" gutterBottom>
                    Speed
                  </Typography>
                </div>

                <div style={{float: 'left', display: 'inline', width: '100px'}}>
                  <Slider
                    style={{height: '100px', marginTop: '10px'}}
                    orientation="vertical"
                    value={this.state.masterControl.brightness}
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
                    onChange={(e, value) => {
                      const masterControl = {...this.state.masterControl};
                      masterControl.brightness = value;
                      // this.props.socket.emit('setBrightnessCorrection', {index: -1, value: masterControl.brightness});
                      this.setState({masterControl})
                    }}
                  />
                  <Typography id="slider-label-brightness" gutterBottom>
                    Brightness
                  </Typography>
                </div>

                <div style={{float: 'left', display: 'inline', width: '100px'}}>
                  <Slider
                    style={{height: '100px', marginTop: '10px'}}
                    orientation="vertical"
                    value={this.state.masterControl.rgbValues[0]}
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
                    onChange={(e, value) => {
                      const masterControl = {...this.state.masterControl};
                      masterControl.rgbValues[0] = value;
                      // this.props.socket.emit('setRgbFade', {index: -1, value: masterControl.rgbValues});
                      this.setState({masterControl})
                    }}
                  />
                  <Typography id="slider-label-red" gutterBottom>
                    Red
                  </Typography>
                </div>

                <div style={{float: 'left', display: 'inline', width: '100px'}}>
                  <Slider
                    style={{height: '100px', marginTop: '10px'}}
                    orientation="vertical"
                    value={this.state.masterControl.rgbValues[1]}
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
                    onChange={(e, value) => {
                      const masterControl = {...this.state.masterControl};
                      masterControl.rgbValues[1] = value;
                      // this.props.socket.emit('setRgbFade', {index: -1, value: masterControl.rgbValues});
                      this.setState({masterControl})
                    }}
                  />
                  <Typography id="slider-label-green" gutterBottom>
                    Green
                  </Typography>
                </div>

                <div style={{float: 'left', display: 'inline', width: '100px'}}>
                  <Slider
                    style={{height: '100px', marginTop: '10px'}}
                    orientation="vertical"
                    value={this.state.masterControl.rgbValues[2]}
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
                    onChange={(e, value) => {
                      const masterControl = {...this.state.masterControl};
                      masterControl.rgbValues[2] = value;
                      // this.props.socket.emit('setRgbFade', {index: -1, value: masterControl.rgbValues});
                      this.setState({masterControl})
                    }}
                  />
                  <Typography id="slider-label-blue" gutterBottom>
                    Blue
                  </Typography>
                </div>
              </div>
            </Grid>
          </Grid>
        </Grid>
      </Grid>

      <Grid container direction={'row'}>
        <Grid item xs>
          <Typography variant={'h6'}>Control panel</Typography>
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
                      value={storyboard.frameWaitMs}
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
                      onChange={(e, value) => {
                        const currentPlayingStoryboard = {...this.state.currentPlayingStoryboard};
                        currentPlayingStoryboard.Animations[i].frameWaitMs = value;
                        // this.props.socket.emit('setFrameWaitMs', {index: i, value: value});
                        this.setState({currentPlayingStoryboard})
                      }}
                    />
                    <Typography id="slider-label-speed" gutterBottom>
                      Speed
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      value={storyboard.brightness}
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
                      onChange={(e, value) => {
                        const currentPlayingStoryboard = {...this.state.currentPlayingStoryboard};
                        currentPlayingStoryboard.Animations[i].brightness = value;
                        // this.props.socket.emit('setBrightnessCorrection', {index: i, value: value});
                        this.setState({currentPlayingStoryboard})
                      }}
                    />
                    <Typography id="slider-label-brightness" gutterBottom>
                      Brightness
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      value={storyboard.rgbValues[0]}
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
                      onChange={(e, value) => {
                        const currentPlayingStoryboard = {...this.state.currentPlayingStoryboard};
                        currentPlayingStoryboard.Animations[i].rgbValues[0] = value;
                        // this.props.socket.emit('setRgbFade', {
                        //   index: i,
                        //   value: currentPlayingStoryboard.Animations[i].rgbValues
                        // });
                        this.setState({currentPlayingStoryboard})
                      }}
                    />
                    <Typography id="slider-label-red" gutterBottom>
                      Red
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      value={storyboard.rgbValues[1]}
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
                      onChange={(e, value) => {
                        const currentPlayingStoryboard = {...this.state.currentPlayingStoryboard};
                        currentPlayingStoryboard.Animations[i].rgbValues[1] = value;
                        // this.props.socket.emit('setRgbFade', {
                        //   index: i,
                        //   value: currentPlayingStoryboard.Animations[i].rgbValues
                        // });
                        this.setState({currentPlayingStoryboard})
                      }}
                    />
                    <Typography id="slider-label-green" gutterBottom>
                      Green
                    </Typography>
                  </div>

                  <div style={{float: 'left', display: 'inline', width: '100px'}}>
                    <Slider
                      style={{height: '100px', marginTop: '10px'}}
                      orientation="vertical"
                      value={storyboard.rgbValues[2]}
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
                      onChange={(e, value) => {
                        const currentPlayingStoryboard = {...this.state.currentPlayingStoryboard};
                        currentPlayingStoryboard.Animations[i].rgbValues[2] = value;
                        // this.props.socket.emit('setRgbFade', {
                        //   index: i,
                        //   value: currentPlayingStoryboard.Animations[i].rgbValues
                        // });
                        this.setState({currentPlayingStoryboard})
                      }}
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

export default HomePage; //socketConnect(HomePage);
