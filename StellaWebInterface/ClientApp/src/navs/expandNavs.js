import React from 'react';
import List from '@material-ui/core/List/index';
import ListItem from '@material-ui/core/ListItem/index';
import ListItemIcon from '@material-ui/core/ListItemIcon/index';
import ListItemText from '@material-ui/core/ListItemText/index';
import Collapse from '@material-ui/core/Collapse/index';
import ExpandLess from '@material-ui/icons/ExpandLess';
import ExpandMore from '@material-ui/icons/ExpandMore';
import StarBorder from '@material-ui/icons/StarBorder';
import LabelIcon from '@material-ui/icons/Label';
import {NavLink} from 'react-router-dom';

class ExpandNav extends React.Component {
  state = {
    componentsMenuOpen: false
  };

  handleClick = () => {
    this.setState({componentsMenuOpen: !this.state.componentsMenuOpen});
  };

  render() {
    return (<List component="nav">
      <ListItem button onClick={this.handleClick}>
        <ListItemIcon className="innernavitem">
          <LabelIcon/>
        </ListItemIcon>
        <ListItemText inset primary="Components"/>
        {this.state.componentsMenuOpen ? <ExpandLess/> : <ExpandMore/>}
      </ListItem>
      <Collapse in={this.state.componentsMenuOpen} timeout="auto" unmountOnExit>
        <List component="div" disablePadding>
          <NavLink to="/forms" className="NavLinkItem">
            <ListItem button>
              <ListItemIcon className="innernavitem">
                <StarBorder/>
              </ListItemIcon>
              <ListItemText inset primary="Forms"/>
            </ListItem>
          </NavLink>
        </List>
      </Collapse>
    </List>);
  }
}

export default ExpandNav;
