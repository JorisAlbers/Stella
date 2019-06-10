import React from 'react';
import List from '@material-ui/core/List/index';
import ListItem from '@material-ui/core/ListItem/index';
import ListItemIcon from '@material-ui/core/ListItemIcon/index';
import ListItemText from '@material-ui/core/ListItemText/index';
import PropTypes from "prop-types";
import HomeIcon from '@material-ui/icons/Home';
import QuestionAnswer from '@material-ui/icons/QuestionAnswer';

/* import your desired icon from material-ui icons library */
import {NavLink} from 'react-router-dom';

export const publicNavs = [
  {
    url: '/',
    name: 'Home',
    icon: <HomeIcon/>
  }, {
    url: '/how-does-it-work',
    name: 'How does this app work',
    icon: <QuestionAnswer/>
  },
  // add new Nav links here as a json object, in this file the public navigations
];

export default () => (
  publicNavs.map((navItem) => {
    return <NavLink to={navItem.url} className="NavLinkItem" key={navItem.url} activeClassName="NavLinkItem-selected">
      <List component="nav">
        <ListItem button>
          <ListItemIcon className="innernavitem">
            {navItem.icon}
          </ListItemIcon>
          <ListItemText primary={navItem.name} className="innernavitem" color="black"/>
        </ListItem>
      </List>
    </NavLink>
  })
);
