import React, {Component} from "react";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import PropTypes from "prop-types";
import dotnetify from "dotnetify";

export default class Status extends Component {
  static contextTypes = {connect: PropTypes.func};

  constructor(props, context) {
    super(props, context);
    this.state = {
      ConnectedClients: 0,
      ConnectedRaspberries: 0
    };
    this.vm = dotnetify.react.connect('StatusBlock', this);

    this.dispatch = state => this.vm.$dispatch(state);
    this.dispatchState = state => {
      this.setState(state);
      this.vm.$dispatch(state);
    };
  }

  componentWillUnmount(): void {
    this.vm.$destroy()
  }

  render() {
    return (
      <Grid xs item>
        <Typography variant={'h6'}>Application status</Typography>
        <React.Fragment>
          <p>connectedClients: {this.state.ConnectedClients}</p>
          <p onClick={() => {
            this.dispatch({ConnectedRaspberries: this.state.ConnectedRaspberries + 10});
            this.setState({ConnectedRaspberries: this.state.ConnectedRaspberries + 10});
          }}>connectedRaspberries: {this.state.ConnectedRaspberries}</p>
        </React.Fragment>
      </Grid>
    )
  };
}
