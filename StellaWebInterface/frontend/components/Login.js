import React from 'react';
import Typography from '@material-ui/core/Typography/index';
import Paper from '@material-ui/core/Paper/index';
import TextField from '@material-ui/core/TextField/index';
import AccountCircle from '@material-ui/icons/AccountCircle';
import Key from '@material-ui/icons/VpnKey';
import Button from '@material-ui/core/Button/index';

export class Login extends React.Component {
  constructor(props) {
    super(props);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.state = {
      loginFail: false
    }
  }

  handleSubmit = (e) => {
    const formData = new FormData(e.target);
    e.preventDefault();

    const user = {};
    for (let entry of formData.entries()) {
      user[entry[0]] = entry[1]
    }

    const userData = startLogin(user);
    this.props.startLogin(user.uid);
    if (userData) this.props.history.push('/dashboard');
    else this.setState({loginFail: true})
  };

  render() {
    const {loginFail} = this.state;
    return (
      <div className="login-page-class">
        <Paper className="loginPaper">
          <div className="loginheaderpart">
            <Typography variant="display3" gutterBottom className="loginpageheader">
              Login
            </Typography>
          </div>
          <Typography variant="headline" component="h3">
            Login to your account
          </Typography>
          {loginFail &&
          <Typography variant="headline" color={'error'} component="h3">
            Sorry, your login failed
          </Typography>
          }
          <form onSubmit={this.handleSubmit}>
            <div className="loginformgroup">
              <AccountCircle/>
              <TextField id="input-username" label="Username" name={'name'}/>
            </div>
            <div className="loginformgroup">
              <Key/>
              <TextField type="password" id="input-password" label="Password" name={'password'}/>
            </div>
            <Button type={'submit'} variant="raised" color="primary">
              <Typography variant="button" gutterBottom className="logintypography">
                Login
              </Typography>
            </Button>
          </form>
        </Paper>
      </div>
    );
  }
}


export default Login;
