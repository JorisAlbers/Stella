import React from 'react';
import InputAdornment from "@material-ui/core/InputAdornment";
import TextField from "@material-ui/core/TextField";
import Divider from "@material-ui/core/Divider";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import {socketConnect} from 'socket.io-react';
import Rnd from "react-rnd-rotate";

import '../styles/configurationPage.css'

const none = {
  display: 'none'
};
const handleRotateStyles = {
  position: 'absolute',
  width: '10px',
  height: '10px',
  left: '-5px',
  marginLeft: '50%',
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
      dataIsAlreadySet: false,
      data: {
        ledstrips: {amount: 10, items: []},
        room: {y: 200, x: 200}
      },
      parentRoomDiv: null,
      rndRoomDiv: null
    };

    if (!this.state.dataIsAlreadySet) {
      for (let i = 0; i < this.state.data.ledstrips.amount; i++) {
        this.state.data.ledstrips.items.push({
          id: i,
          position: {
            x: i * 20,
            y: 0,
            degree: 0
          },
          size: {x: 100, y: 100}
        })
      }
    }

    this.props.socket.emit('getSavedLedMapping');
    // TODO make it so that that stupid buggy degree thing also works
    this.props.socket.on('returnSavedLedMapping', (savedLedMapping) => {
      return this.setState({
        data: savedLedMapping,
        dataIsAlreadySet: true
      }, this.forceUpdate);
    });
  }

  componentDidMount() {
    this.setState({
      parentRoomDiv: this.parentRoom,
    });
  }

  componentDidUpdate(prevProps, prevState, snapshot) {
    // Update the div where the draggable content is on.
    if (prevState.rndRoomDiv !== this.rnd) {
      this.setState({rndRoomDiv: this.rnd});
    }

    // Update the amount of ledstrips
    if (prevState.data.ledstrips.amount !== this.state.data.ledstrips.amount) {
      let data = {...this.state.data};
      // If the new amount is more
      if (prevState.data.ledstrips.amount < this.state.data.ledstrips.amount) {
        for (let i = 0; i < this.state.data.ledstrips.amount; i++) {
          if (data.ledstrips.items[i]) continue;
          data.ledstrips.items.push({
            id: i,
            position: {
              x: i * 20,
              y: 0,
              degree: 0,
            },
            size: {x: 100, y: 100}
          })
        }
      }
      // If the new amount is less
      if (prevState.data.ledstrips.amount > this.state.data.ledstrips.amount) {
        data.ledstrips.items = data.ledstrips.items.splice(0, this.state.data.ledstrips.amount)
      }
      this.setState(data);
    }

    // Update the room that defines the sizes of everything on the initial load.
    if (prevState.parentRoomDiv !== this.parentRoom ||
      prevState.data.ledstrips.amount !== this.state.data.ledstrips.amount) {
      this.setState({parentRoomDiv: this.parentRoom});
      let data = {...this.state.data};
      for (let i = 0; i < this.state.data.ledstrips.amount; i++) {
        data.ledstrips.items[i].size.x = (this.parentRoom.childNodes[0].clientWidth) / this.state.data.room.x * 10;
        data.ledstrips.items[i].size.y = (this.parentRoom.childNodes[0].clientHeight) / this.state.data.room.y * 200
      }
      this.setState(data);
    }

    // Update the sizes of everything when the room ratio/size changes.
    if (prevState.data.room.x !== this.state.data.room.x ||
      prevState.data.room.y !== this.state.data.room.y ||
      prevState.data.ledstrips.amount !== this.state.data.ledstrips.amount) {
      let data = {...this.state.data};
      for (let i = 0; i < this.state.data.ledstrips.amount; i++) {
        data.ledstrips.items[i].size.x = (this.parentRoom.childNodes[0].clientWidth) / this.state.data.room.x * 10;
        data.ledstrips.items[i].size.y = (this.parentRoom.childNodes[0].clientHeight) / this.state.data.room.y * 200;
      }
      this.setState(data);
    }
  }

  drawRoom(data) {
    const totalAvailableWidth = this.state.parentRoomDiv.clientWidth - 20;
    const max = Math.max(data.room.y, data.room.x);
    const items = data.ledstrips.items;

    return <div
      style={{
        border: "1px solid red",
        position: 'relative',
        width: max === data.room.x ? '100%' : totalAvailableWidth / (data.room.y / data.room.x),
        height: max === data.room.y ? '100%' : totalAvailableWidth / (data.room.x / data.room.y),

        backgroundColor: "transparent",
        backgroundImage: 'linear-gradient(0deg, transparent 9%, rgba(255, 255, 255, .2) 10%, rgba(255, 255, 255, .2) 12%, transparent 13%, transparent 29%, rgba(255, 255, 255, .1) 30%, rgba(255, 255, 255, .1) 31%, transparent 32%, transparent 49%,rgba(255, 255, 255, .1) 50%, rgba(255, 255, 255, .1) 51%, transparent 52%, transparent 69%, rgba(255, 255, 255, .1) 70%, rgba(255, 255, 255, .1) 71%, transparent 72%, transparent 89%,rgba(255, 255, 255, .1) 90%, rgba(255, 255, 255, .1) 91%, transparent 92%, transparent),linear-gradient(90deg, transparent 9%,rgba(255, 255, 255, .2) 10%, rgba(255, 255, 255, .2) 12%, transparent 13%, transparent 29%,rgba(255, 255, 255, .1) 30%, rgba(255, 255, 255, .1) 31%, transparent 32%, transparent 49%,rgba(255, 255, 255, .1) 50%, rgba(255, 255, 255, .1) 51%, transparent 52%, transparent 69%,rgba(255, 255, 255, .1) 70%, rgba(255, 255, 255, .1) 71%, transparent 72%, transparent 89%,rgba(255, 255, 255, .1) 90%, rgba(255, 255, 255, .1) 91%, transparent 92%, transparent)',
        backgroundSize: '50px 50px',
        backgroundPositionX: '-8px',
        backgroundPositionY: '6px',
      }}
    >
      {items.map((item, i) => {
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
            rotate: handleRotateStyles
          }}
          key={i}
          bounds={'parent'}
          position={{
            x: data.ledstrips.items[i].position.x / data.room.x * this.state.parentRoomDiv.clientWidth,
            y: data.ledstrips.items[i].position.y / data.room.y * this.state.parentRoomDiv.clientHeight
          }}
          size={{
            width: data.ledstrips.items[i].size.x,
            height: data.ledstrips.items[i].size.y,
            degree: data.ledstrips.items[i].position.degree
          }}
          onDragStop={(e, d) => {
            data.ledstrips.items[i].position.x = d.x / this.state.parentRoomDiv.clientWidth * data.room.x;
            data.ledstrips.items[i].position.y = d.y / this.state.parentRoomDiv.clientHeight * data.room.y;
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
  }

  render() {
    return <div>
      <Grid container spacing={0} direction={'column'}>
        <Grid xs item>
          <Button variant={'outlined'} color="inherit" align="left"
                  onClick={() => this.setState({currentTab: 'general'})}>General</Button>
          <Button variant={'outlined'} color="inherit" align="left"
                  onClick={() => this.setState({currentTab: 'pixelMapping'})}>Map the LED
            strips</Button>
        </Grid>
        <Grid container direction={'row'}>
          {this.state.currentTab === 'general' &&
          <React.Fragment>

          </React.Fragment>
          }
          {this.state.currentTab === 'pixelMapping' &&
          <React.Fragment>
            <Grid xs item>
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                type={'Number'}
                min={1}
                max={100}
                label="Amount of LED-strips"
                value={this.state.data.ledstrips.amount}
                onChange={(e) => {
                  this.setState({
                    data: {
                      ...this.state.data,
                      ledstrips: {...this.state.data.ledstrips, amount: parseInt(e.target.value)}
                    }
                  });
                }}
                InputProps={{endAdornment: <InputAdornment position="end">units</InputAdornment>}}
              />
              <Divider variant="middle"/>
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                type={'Number'}
                min={2}
                max={100}
                label="Depth of the room"
                value={this.state.data.room.y}
                onChange={(e) => {
                  this.setState({
                    data: {
                      ...this.state.data,
                      room: {...this.state.data.room, y: parseInt(e.target.value)}
                    }
                  });
                }}
                InputProps={{endAdornment: <InputAdornment position="end">centimeter</InputAdornment>}}
              />
              <TextField
                margin={'dense'}
                id="outlined-simple-start-adornment"
                variant="outlined"
                type={'Number'}
                min={1}
                max={100}
                label="Width of the room"
                value={this.state.data.room.x}
                onChange={(e) => {
                  this.setState({
                    data: {
                      ...this.state.data,
                      room: {...this.state.data.room, x: parseInt(e.target.value)}
                    }
                  });
                }}
                InputProps={{endAdornment: <InputAdornment position="end">centimeter</InputAdornment>,}}
              />
              <Divider variant="middle"/>
              <Button variant={'outlined'} color="inherit" align="left" onClick={() => {
                this.props.socket.emit('setSavedLedMapping', this.state.data)
              }}>Save</Button>
            </Grid>
            <Grid xs={10} item
                  ref={(parentRoom) => this.parentRoom = parentRoom}
                  style={{
                    height: this.state.parentRoomDiv && this.state.parentRoomDiv.clientWidth || 0,
                    backgroundColor: '#434343',
                    backgroundImage: 'linear-gradient(#5480D3, #3256A7)',
                  }}>
              {this.state.parentRoomDiv !== null && this.drawRoom(this.state.data)}
            </Grid>
          </React.Fragment>
          }
        </Grid>
      </Grid>
    </div>;
  }
}

export default socketConnect(ConfigurationPage);
