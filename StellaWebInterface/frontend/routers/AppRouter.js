import React from 'react';
import {BrowserRouter, Route, Switch} from 'react-router-dom';
import PublicRoute from './PublicRouter';

//Public
import HomePage from '../components/HomePage';
import NotFoundPage from '../components/NotFoundPage';
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
