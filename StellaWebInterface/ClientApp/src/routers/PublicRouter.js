import React from 'react';
import {Route} from 'react-router-dom';
import Header from '../components/Header';
import Footer from '../components/Footer';
import General from "../components/General";

export const PublicRoute = ({component: Component, ...rest}) => (
  <Route {...rest} component={(props) => (
    <React.Fragment>
      <General/>
      <Header/>
      <Component {...props} />
      <Footer/>
    </React.Fragment>
  )}/>
);

export default PublicRoute;
