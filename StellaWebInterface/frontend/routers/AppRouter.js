import React from 'react';
import {BrowserRouter, Switch} from 'react-router-dom';
import PublicRoute from './PublicRouter';
//Public
import HomePage from '../components/HomePage';
import HowDoesItWorkPage from "../components/HowDoesItWorkPage";
import Login from '../components/Login';
import Register from '../components/Register';

const AppRouter = () => (
  <BrowserRouter>
    <div>
      <Switch>
        <PublicRoute path="/" component={HomePage} exact={true}/>
        <PublicRoute path="/how-does-it-work" component={HowDoesItWorkPage}/>
        <PublicRoute path="/login" component={Login}/>
        <PublicRoute path="/register" component={Register}/>

        {/*<Route component={NotFoundPage}/>*/}
      </Switch>

    </div>
  </BrowserRouter>
);

export default AppRouter;
