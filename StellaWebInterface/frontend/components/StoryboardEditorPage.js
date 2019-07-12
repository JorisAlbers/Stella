import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Typography from "@material-ui/core/Typography";
import {socketConnect} from 'socket.io-react';
import FileCopy from "@material-ui/icons/fileCopy";
import Add from "@material-ui/icons/add";
import Edit from "@material-ui/icons/edit";
import Rnd from "react-rnd-rotate";
import TextField from "@material-ui/core/TextField";
import InputAdornment from "@material-ui/core/InputAdornment";

const none = {
  display: 'none'
};

class EditorPage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.parentRoom = null;
    this.rnd = null;
    this.state = {
      storyboards: [],
      animations: [],
      newStoryboard: {
        amountTimelines: 2,
        animations: []
      },
      parentRoomDiv: null,
      rndRoomDiv: null
    };

    this.props.socket.on('returnAvailableStoryboards', (data) => {
      const animations = [];
      for (let i = 0; i < data.storyboards.length; i++) {
        for (let j = 0; j < data.storyboards[i].Animations.length; j++) {
          animations.push({
            class: data.storyboards[i].Animations[j].class,
            pattern: data.storyboards[i].Animations[j].pattern,
            wraps: data.storyboards[i].Animations[j].wraps,
            color: data.storyboards[i].Animations[j].color,
            imageName: data.storyboards[i].Animations[j].imageName
          })
        }
      }
      this.setState({storyboards: data.storyboards, animations: animations})
    });
  }

  componentDidMount() {
    this.props.socket.emit('getAvailableStoryboards');
    this.setState({
      parentRoomDiv: this.parentRoom,
    });
  }

  render() {
    return <div>
      <Grid container spacing={0} direction={'row'}>
        <Grid xs item>
          <Typography variant={'h6'}>Available storyboards</Typography>
          <div>
            <Add style={{cursor: 'pointer', margin: '5px'}}/>
            <p style={{display: 'inline'}}>Add new storyboard</p>
          </div>
          {this.state.storyboards && this.state.storyboards.map((item, i) => (
            <div key={i}>
              <FileCopy style={{cursor: 'pointer', margin: '5px'}}/>
              <Edit style={{cursor: 'pointer', margin: '5px'}}/>
              <p style={{display: 'inline'}}>{item.Name}</p>
            </div>
          ))}
        </Grid>
        <Grid xs item>
          <Typography variant={'h6'}>Available animations</Typography>
          {this.state.animations && this.state.animations.map((item, i) => (
            <div key={i}>
              <Add/>
              <p
                style={{display: 'inline'}}>{item.class} [{item.pattern || ''}{item.imageName || ''}{item.color || ''}{item.wraps || ''}]</p>
            </div>
          ))}
        </Grid>
        <Grid xs item>
          <Typography variant={'h6'}>Your new storyboard</Typography>
          {this.state.newStoryboard && (
            <div>
              <TextField
                margin={'dense'}
                // id="outlined-simple-start-adornment"
                variant="outlined"
                type={'Text'}
                label="Name"
                defaultValue={this.state.newStoryboard.name}
                onChange={(e) => {
                }}
              />
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                type={'Number'}
                min={1}
                max={10}
                label="Amount of desired timelines"
                defaultValue={this.state.newStoryboard.amountTimelines}
                onChange={(e) => {
                  this.setState({
                    newStoryboard: {
                      ...this.state.newStoryboard,
                      amountTimelines: parseInt(e.target.value)
                    }
                  });
                }}
                InputProps={{endAdornment: <InputAdornment position="end">timelines</InputAdornment>}}
              />
            </div>
          )}
        </Grid>
        <Grid container direction={'row'}>
          <Grid xs item ref={(parentRoom) => this.parentRoom = parentRoom}
                style={{
                  height: (this.state.newStoryboard.amountTimelines * 50) + 22 + 'px' ,
                  backgroundColor: '#434343',
                  backgroundImage: 'linear-gradient(#5480D3, #3256A7)',
                }}>
            {this.state.parentRoomDiv !== null && (
              <div>
                <div
                  style={{
                    border: "1px solid red",
                    position: 'relative',
                    width: '100%',
                    height: (this.state.newStoryboard.amountTimelines * 50) + 'px',

                    backgroundColor: "transparent",
                    backgroundImage: 'linear-gradient(0deg, transparent 9%, rgba(255, 255, 255, .2) 10%, rgba(255, 255, 255, .2) 12%, transparent 13%, transparent 29%, rgba(255, 255, 255, .1) 30%, rgba(255, 255, 255, .1) 31%, transparent 32%, transparent 49%,rgba(255, 255, 255, .1) 50%, rgba(255, 255, 255, .1) 51%, transparent 52%, transparent 69%, rgba(255, 255, 255, .1) 70%, rgba(255, 255, 255, .1) 71%, transparent 72%, transparent 89%,rgba(255, 255, 255, .1) 90%, rgba(255, 255, 255, .1) 91%, transparent 92%, transparent),linear-gradient(90deg, transparent 9%,rgba(255, 255, 255, .2) 10%, rgba(255, 255, 255, .2) 12%, transparent 13%, transparent 29%,rgba(255, 255, 255, .1) 30%, rgba(255, 255, 255, .1) 31%, transparent 32%, transparent 49%,rgba(255, 255, 255, .1) 50%, rgba(255, 255, 255, .1) 51%, transparent 52%, transparent 69%,rgba(255, 255, 255, .1) 70%, rgba(255, 255, 255, .1) 71%, transparent 72%, transparent 89%,rgba(255, 255, 255, .1) 90%, rgba(255, 255, 255, .1) 91%, transparent 92%, transparent)',
                    backgroundSize: '50px 50px',
                    backgroundPositionX: '-8px',
                    backgroundPositionY: '6px',
                  }}
                >
                  {this.state.newStoryboard && this.state.newStoryboard.animations && this.state.newStoryboard.animations.map((item, i) => {
                    return <Rnd
                      ref={(c) => this.rnd = c}
                      style={{
                        // TODO @martijn: Try to make it so that the square has the correct bounds when rotated
                        // marginTop: `${(data.ledstrips.items[i].size.y / 2) * Math.abs(Math.cos(data.ledstrips.items[i].position.degree  * Math.PI / 180)) - ledstrips.items[i].size.y / 2}px`,
                        // marginLeft: `${(data.ledstrips.items[i].size.y / 2) * Math.sin(data.ledstrips.items[i].position.degree  * Math.PI / 180)}px`
                      }}
                      className={'rnd-resizable1'}
                      resizeHandleClasses={{
                        rotate: 'resize-handle-base-class'
                      }}
                      resizeHandleStyles={{
                        left: none,
                        right: none,
                        top: none,
                        bottom: none,
                        topLeft: none,
                        topRight: none,
                        bottomRight: none,
                        bottomLeft: none,
                        topCenter: none,
                        rightCenter: none,
                        bottomCenter: none,
                        leftCenter: none,
                        rotate: none
                      }}
                      key={i}
                      bounds={'parent'}
                      position={{
                        x: data.ledstrips.items[i].position.x,
                        y: data.ledstrips.items[i].position.y,
                      }}
                      size={{
                        width: data.ledstrips.items[i].size.x,
                        height: data.ledstrips.items[i].size.y,
                        degree: data.ledstrips.items[i].position.degree
                      }}
                      onDragStop={(e, d) => {
                        data.ledstrips.items[i].position.x = d.x;
                        data.ledstrips.items[i].position.y = d.y;
                        this.setState({data})
                      }}
                      onResizeStop={(e, direction, resizable, delta, position) => {
                        data.ledstrips.items[i].position.degree = delta.degree;
                        this.setState({data});
                      }}
                    >
                      {i + 1}
                    </Rnd>
                  })}
                </div>
              </div>
            )}
          </Grid>
        </Grid>
      </Grid>
    </div>;
  }
}

export default socketConnect(EditorPage);
