import React from 'react';
import InputAdornment from "@material-ui/core/InputAdornment";
import TextField from "@material-ui/core/TextField";
import Divider from "@material-ui/core/Divider";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import {socketConnect} from 'socket.io-react';
import Rnd from "react-rnd-rotate";

import '../styles/configurationPage.css'

const ledstripsStyle = {
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  border: 'solid 1px #ddd',
  background: '#f0f0f0',
  zIndex: 10
};
const none = {
  display: 'none'
};
const handleRotateStyles = {
  position: 'absolute',
  width: '10px',
  height: '10px',
  left: 0,
  marginLeft: 0,
  backgroundImage: 'none',
  marginTop: '-10px',
  top: '-10px',
};

class ConfigurationPage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.parentRoom = null;
    this.rnd = null;
    this.state = {
      currentTab: 'pixelMapping',
      data: {
        ledstrips: {
          amount: 10,
          items: []
        },
        room: {
          x: 10,
          y: 10
        }
      },
      parentRoomDiv: null
    };
  }

  componentDidMount() {
    let data = {...this.state.data};
    for (let i = 0; i < this.state.data.ledstrips.amount; i++) {
      data.ledstrips.items.push({
        id: i,
        position: {
          x: i * 10,
          y: i * 10,
          degree: 100,
        },
        size: {
          x: 15,
          y: 100
        }
      })
    }

    this.setState({parentRoomDiv: this.parentRoom, data});
  }

  componentDidUpdate(prevProps, prevState, snapshot) {
    if (prevState.parentRoomDiv !== this.parentRoom) {
      this.setState({parentRoomDiv: this.parentRoom});
    }
  }

  drawRoom() {
    const totalAvailableWidth = this.state.parentRoomDiv.clientWidth - 20;
    const max = Math.max(this.state.data.room.y, this.state.data.room.x);

    return <div
      style={{
        border: "1px solid red",
        position: 'relative',
        width: max === this.state.data.room.x ? '100%' : totalAvailableWidth / (this.state.data.room.y / this.state.data.room.x),
        height: max === this.state.data.room.y ? '100%' : totalAvailableWidth / (this.state.data.room.x / this.state.data.room.y),

        backgroundColor: "transparent",
        backgroundImage: 'linear-gradient(0deg, transparent 9%, rgba(255, 255, 255, .2) 10%, rgba(255, 255, 255, .2) 12%, transparent 13%, transparent 29%, rgba(255, 255, 255, .1) 30%, rgba(255, 255, 255, .1) 31%, transparent 32%, transparent 49%,rgba(255, 255, 255, .1) 50%, rgba(255, 255, 255, .1) 51%, transparent 52%, transparent 69%, rgba(255, 255, 255, .1) 70%, rgba(255, 255, 255, .1) 71%, transparent 72%, transparent 89%,rgba(255, 255, 255, .1) 90%, rgba(255, 255, 255, .1) 91%, transparent 92%, transparent),linear-gradient(90deg, transparent 9%,rgba(255, 255, 255, .2) 10%, rgba(255, 255, 255, .2) 12%, transparent 13%, transparent 29%,rgba(255, 255, 255, .1) 30%, rgba(255, 255, 255, .1) 31%, transparent 32%, transparent 49%,rgba(255, 255, 255, .1) 50%, rgba(255, 255, 255, .1) 51%, transparent 52%, transparent 69%,rgba(255, 255, 255, .1) 70%, rgba(255, 255, 255, .1) 71%, transparent 72%, transparent 89%,rgba(255, 255, 255, .1) 90%, rgba(255, 255, 255, .1) 91%, transparent 92%, transparent)',
        backgroundSize: '50px 50px',
        backgroundPositionX: '-8px',
        backgroundPositionY: '6px',
      }}
    >
      {this.state.data.ledstrips.items.map((item, i) => {
        return <Rnd
          ref={(c) => this.rnd = c}
          style={ledstripsStyle}
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
            rotate: handleRotateStyles
          }}
          key={i}
          bounds={'parent'}
          position={{
            x: this.state.data.ledstrips.items[i].position.x,
            y: this.state.data.ledstrips.items[i].position.y,
            degree: this.state.data.ledstrips.items[i].position.degree
          }}
          size={{
            width: this.state.data.ledstrips.items[i].size.x,
            height: this.state.data.ledstrips.items[i].size.y
          }}

          onDragStop={(e, d) => {
            let data = this.state.data;
            data.ledstrips.items[i].position.x = d.x;
            data.ledstrips.items[i].position.y = d.y;
            this.setState({data})
          }}
          onResizeStop={(e, direction, resizable, delta, position) => {
            let data = this.state.data;
            data.ledstrips.items[i].position.degree = delta.degree;
            this.setState({data});
          }}
        >
          {i + 1}
        </Rnd>
      })}
    </div>

  }

  render() {
    const {currentTab} = this.state;

    return <div>
      <Grid container spacing={0} direction={'column'}>
        <Grid xs item>
          <Button color="inherit" align="left" onClick={() => this.setState({currentTab: 'general'})}>General</Button>
          <Button color="inherit" align="left" onClick={() => this.setState({currentTab: 'pixelMapping'})}>Map the LED
            strips</Button>
        </Grid>
        <Grid container direction={'row'}>
          {currentTab === 'general' &&
          <React.Fragment>

          </React.Fragment>
          }
          {currentTab === 'pixelMapping' &&
          <React.Fragment>
            <Grid xs item>
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                label="Amount of LED-strips"
                defaultValue={this.state.data.ledstrips.amount}
                onChange={(e) => {
                  let data = {...this.state.data};
                  data.ledstrips.amount = e.target.value;
                  return this.setState({data});
                }}
                InputProps={{endAdornment: <InputAdornment position="end">units</InputAdornment>}}
              />
              <Divider variant="middle"/>
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                label="Depth of the room"
                defaultValue={this.state.data.room.y}
                onChange={(e) => {
                  let data = {...this.state.data};
                  data.room.y = e.target.value;
                  return this.setState({data});
                }}
                InputProps={{endAdornment: <InputAdornment position="end">meter</InputAdornment>}}
              />
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                label="Width of the room"
                defaultValue={this.state.data.room.x}
                onChange={(e) => {
                  let data = {...this.state.data};
                  data.room.x = e.target.value;
                  return this.setState({data});
                }}
                InputProps={{endAdornment: <InputAdornment position="end">meter</InputAdornment>,}}
              />
            </Grid>
            <Grid xs={10} item
                  ref={(parentRoom) => this.parentRoom = parentRoom}
                  style={{
                    height: this.state.parentRoomDiv && this.state.parentRoomDiv.clientWidth || 0,
                    backgroundColor: '#434343',
                    backgroundImage: 'linear-gradient(#5480D3, #3256A7)',
                  }}>
              {this.state.parentRoomDiv !== null && this.drawRoom()}
            </Grid>
          </React.Fragment>
          }
        </Grid>
      </Grid>
    </div>;
  }
}

export default socketConnect(ConfigurationPage);
