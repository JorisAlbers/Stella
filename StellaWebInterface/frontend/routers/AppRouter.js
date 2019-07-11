import React from 'react';
import {BrowserRouter, Switch} from 'react-router-dom';
import PublicRoute from './PublicRouter';
//Public
import HomePage from '../components/HomePage';
import EditorPage from "../components/EditorPage";
import StoryboardPage from "../components/StoryboardEditorPage";
import ConfigurationPage from '../components/ConfigurationPage';

const AppRouter = () => (
  <BrowserRouter>
    <div>
      <Switch>
        <PublicRoute path="/" component={HomePage} exact={true}/>
        <PublicRoute path="/storyboard-editor" component={StoryboardPage}/>
        <PublicRoute path="/animation-editor" component={EditorPage}/>
        <PublicRoute path="/configurations" component={ConfigurationPage}/>

        {/*<Route component={NotFoundPage}/>*/}
      </Switch>

    </div>
  </BrowserRouter>
);

export default AppRouter;
