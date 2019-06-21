import React from 'react';
import {BrowserRouter, Switch} from 'react-router-dom';
import PublicRoute from './PublicRouter';
//Public
import HomePage from '../components/HomePage';
import EditorPage from "../components/EditorPage";
import ConfigurationPage from '../components/ConfigurationPage';

const AppRouter = () => (
  <BrowserRouter>
    <div>
      <Switch>
        <PublicRoute path="/" component={HomePage} exact={true}/>
        <PublicRoute path="/editor" component={EditorPage}/>
        <PublicRoute path="/configurations" component={ConfigurationPage}/>
        {/*<PublicRoute path="/register" component={Register}/>*/}

        {/*<Route component={NotFoundPage}/>*/}
      </Switch>

    </div>
  </BrowserRouter>
);

export default AppRouter;
