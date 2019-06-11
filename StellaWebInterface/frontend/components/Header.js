import React from 'react';
import AppBar from '@material-ui/core/AppBar/index';

import Drawer from '@material-ui/core/Drawer/index';
import Toolbar from '@material-ui/core/Toolbar/index';
import Button from '@material-ui/core/Button/index';
import IconButton from '@material-ui/core/IconButton/index';
import MenuIcon from '@material-ui/icons/Menu';

import {Link} from 'react-router-dom';
import PublicNavList from '../navs/publicNav';
import ExpandNavList from '../navs/expandNavs'

class Header extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      value: 1,
      open: false,
      componentsmenuopen: false
    };
  }

  onLeftIconButtonClick = (event, index, value) => {
    this.setState({open: !this.state.open});
  };

  toggleDrawer = (open) => () => {
    this.setState({
      open: open,
    });
  };

  render() {
    return (
      <div>
        <Drawer open={this.state.open} onClose={this.toggleDrawer(false)}>
          <div tabIndex={0} role="button">
            <div className="sidelistwrapper">
              <React.Fragment>
                <PublicNavList/>
                <ExpandNavList/>
              </React.Fragment>
            </div>
          </div>
        </Drawer>
        <div className="appbarwrapper">
          <AppBar position="static">
            <Toolbar>
              <IconButton className="iconbuttonsyle" color="inherit" aria-label="Menu"
                          onClick={this.onLeftIconButtonClick}>
                <MenuIcon/>
              </IconButton>
              <Button color="inherit" align="right"><Link to="/">Home</Link></Button>
              <Button color="inherit" align="right"><Link to="/editor">Editor</Link></Button>
              <Button color="inherit" align="right"><Link to="/configurations">Configurations</Link></Button>
            </Toolbar>
          </AppBar>
        </div>
      </div>
    );
  };
}

export default Header;
