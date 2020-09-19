import React, {Component} from 'react';
import dotnetify from "dotnetify";

export default class App extends Component {
  constructor(props) {
    super(props);

    this.state = { Users: []};
    
    this.correlationId = `${Math.random()}`;
    this.vm = dotnetify.react.connect('GeneralVM', this, { vmArg: { AddUser: this.correlationId } });
    this.dispatchState = state => this.vm.$dispatch(state);
  }

  componentWillUnmount() {
    this.dispatchState({RemoveUser: null});
    this.vm.$destroy();
  }

  render() {
    return (
      <div>
      </div>
    )
  };
}
