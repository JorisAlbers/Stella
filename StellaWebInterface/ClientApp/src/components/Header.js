import React from 'react';
import AppBar from '@material-ui/core/AppBar/index';

import Toolbar from '@material-ui/core/Toolbar/index';
import Button from '@material-ui/core/Button/index';

import {Link} from 'react-router-dom';

class Header extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      value: 1,
      open: false,
      componentsmenuopen: false
    };
  }
  
  toggleDrawer = (open) => () => {
    this.setState({
      open: open,
    });
  };

  render() {
    return (
      <div>
        <div className="appbarwrapper">
          <AppBar position="static">
            <Toolbar>
              <Button color="inherit" align="right"><Link to="/">Controls</Link></Button>
              <Button color="inherit" align="right"><Link to="/storyboard-editor">Storyboard editor</Link></Button>
              <Button color="inherit" align="right"><Link to="/animation-editor">Animation editor</Link></Button>
              <Button color="inherit" align="right"><Link to="/configurations">Configurations</Link></Button>
            </Toolbar>
          </AppBar>
        </div>
      </div>
    );
  };
}

export default Header;
