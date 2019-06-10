import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from '@material-ui/core/styles/index';
import Stepper from '@material-ui/core/Stepper/index';
import Step from '@material-ui/core/Step/index';
import StepLabel from '@material-ui/core/StepLabel/index';
import Button from '@material-ui/core/Button/index';
import Typography from '@material-ui/core/Typography/index';
import Grid from '@material-ui/core/Grid/index';
import Paper from '@material-ui/core/Paper/index';
import TextField from '@material-ui/core/TextField/index';

const styles = theme => ({
  root: {
    width: '100%',
  },
  button: {
    marginRight: theme.spacing.unit,
  },
  instructions: {
    marginTop: theme.spacing.unit,
    marginBottom: theme.spacing.unit,
  },
  textField: {
    marginLeft: theme.spacing.unit,
    marginRight: theme.spacing.unit,
    width: 200,
  }
});

function getSteps() {
  return [
    'Name and Email',
    'Personal Details',
    // 'Connect with your aid provider'
  ];
}

class Register extends React.Component {
  static propTypes = {
    classes: PropTypes.object,
  };

  state = {
    activeStep: 0,
    skipped: new Set(),
    country: '',
    state: ''
  };

  isStepOptional = step => {
    return step === 1 || step === 2;
  };

  isStepSkipped(step) {
    return this.state.skipped.has(step);
  }

  isStepFailed = step => {
    return step === 1;
  };

  handleCountryChange = event => {
    this.setState({[event.target.name]: event.target.value});
  };

  handleStateChange = event => {
    this.setState({[event.target.name]: event.target.value})
  };


  getStepContent = (step) => {
    switch (step) {
      case 0:
        return (
          <div style={{padding: 20}}>
            <Typography variant="display2" gutterBottom align="center">Just few details</Typography>
            <Typography variant="body1" gutterBottom align="center">
              Start by filling out the form below to register
            </Typography>
            <Grid container spacing={24}>
              <Grid item xs={6} style={{marginTop: 10}} align="center"><TextField
                id="firstname"
                label="First Name"
                fullWidth
                placeholder="Enter your First Name"
                margin="normal"
              /> </Grid>
              <Grid item xs={6} style={{marginTop: 10}} align="center"><TextField
                id="lastname"
                label="Last Name"
                fullWidth
                placeholder="Enter your Last Name"
                margin="normal"
              /> </Grid>
              <Grid item xs={12} style={{marginTop: 10}} align="center"><TextField
                id="email"
                label="Email address"
                fullWidth
                placeholder="Enter your Email address"
                margin="normal"
              /> </Grid>
              <Grid item xs={12} style={{marginTop: 10}} align="center"><TextField
                id="username"
                label="Username"
                fullWidth
                placeholder="Enter your desired username"
                margin="normal"
              /> </Grid>
              <Grid item xs={6} style={{marginTop: 10}} align="center"><TextField
                id="password"
                label="Password"
                type="password"
                fullWidth
                placeholder="Enter your desired password"
                margin="normal"
              /> </Grid>
              <Grid item xs={6} style={{marginTop: 10}} align="center"><TextField
                id="passwordRetype"
                label="Retype password"
                type="password"
                fullWidth
                placeholder="Retype your desired password"
                margin="normal"
              /> </Grid>
            </Grid>
          </div>
        );
      case 1:
        return (
          <div style={{padding: 20}}>

            <Typography variant="display2" gutterBottom align="center">Tell us more about yourself</Typography>
            <Typography variant="body1" gutterBottom align="center">
              This part can be skipped for now and update later
            </Typography>
            <Grid container spacing={24}>
              <Grid item xs={12} style={{marginTop: 10}} align="center"><TextField
                id="address"
                label="Address"
                fullWidth
                placeholder="Enter your Address"
                margin="normal"
              /> </Grid>
              <Grid item xs={6} style={{marginTop: 10}} align="center"><TextField
                id="streetname"
                label="Street Name"
                fullWidth
                placeholder="Enter your Street Name"
                margin="normal"
              /> </Grid>
              <Grid item xs={6} style={{marginTop: 10}} align="center"><TextField
                id="city"
                label="City"
                fullWidth
                placeholder="Enter your City Name"
                margin="normal"
              /> </Grid>
              {/*<Grid item xs={6} style={{marginTop: 10}} align="center"><FormControl>*/}
              {/*  <InputLabel htmlFor="age-simple">Country</InputLabel>*/}
              {/*  <Select*/}
              {/*    style={{width: 200}}*/}
              {/*    value={this.state.country}*/}
              {/*    onChange={this.handleCountryChange}*/}
              {/*    inputProps={{*/}
              {/*      name: 'country',*/}
              {/*      id: 'country',*/}
              {/*    }}*/}
              {/*  >*/}
              {/*    <MenuItem value="">*/}
              {/*      <em>None</em>*/}
              {/*    </MenuItem>*/}
              {/*    <MenuItem value="India">India</MenuItem>*/}
              {/*    <MenuItem value="Australia">Australia</MenuItem>*/}
              {/*    <MenuItem value="South Africa">South Africa</MenuItem>*/}
              {/*    <MenuItem value="SriLanka">SriLanka</MenuItem>*/}
              {/*    <MenuItem value="NewZealand">NewZealand</MenuItem>*/}
              {/*    <MenuItem value="West Indies">West Indies</MenuItem>*/}
              {/*    <MenuItem value="England">England</MenuItem>*/}
              {/*  </Select>*/}
              {/*</FormControl> </Grid>*/}
              {/*<Grid item xs={6} style={{marginTop: 10}} align="center"><FormControl>*/}
              {/*  <InputLabel htmlFor="age-simple">State</InputLabel>*/}
              {/*  <Select*/}
              {/*    style={{width: 200}}*/}
              {/*    value={this.state.state}*/}
              {/*    onChange={this.handleStateChange}*/}
              {/*    inputProps={{*/}
              {/*      name: 'state',*/}
              {/*      id: 'state',*/}
              {/*    }}*/}
              {/*  >*/}
              {/*    <MenuItem value="">*/}
              {/*      <em>None</em>*/}
              {/*    </MenuItem>*/}
              {/*    <MenuItem value="Dependent state1">Dependent state1</MenuItem>*/}
              {/*    <MenuItem value="Dependent State 2">Dependent State 2</MenuItem>*/}

              {/*  </Select>*/}
              {/*</FormControl> </Grid>*/}
            </Grid>
          </div>
        );
      case 2:
        return 'This is the bit I really care about!';
      default:
        return 'Unknown step';
    }
  };

  handleNext = () => {
    const {activeStep} = this.state;
    let {skipped} = this.state;

    // Final step
    // if (activeStep === steps.length - 1) {
    //
    // }

    if (this.isStepSkipped(activeStep)) {
      skipped = new Set(skipped.values());
      skipped.delete(activeStep);
    }
    this.setState({
      activeStep: activeStep + 1,
      skipped,
    });
  };

  handleBack = () => {
    const {activeStep} = this.state;
    this.setState({
      activeStep: activeStep - 1,
    });
  };

  handleSkip = () => {
    const {activeStep} = this.state;
    if (!this.isStepOptional(activeStep)) {
      // You probably want to guard against something like this,
      // it should never occur unless someone's actively trying to break something.
      throw new Error("You can't skip a step that isn't optional.");
    }
    const skipped = new Set(this.state.skipped.values());
    skipped.add(activeStep);
    this.setState({
      activeStep: this.state.activeStep + 1,
      skipped,
    });
  };

  render() {
    const {classes} = this.props;
    const steps = getSteps();
    const {activeStep} = this.state;

    return (
      <div className={classes.root}>

        <Stepper activeStep={activeStep}>
          {steps.map((label, index) => {
            const props = {};
            const labelProps = {};
            if (this.isStepOptional(index)) {
              labelProps.optional = (
                <Typography variant="caption" color="error">
                  Alert message
                </Typography>
              );
            }
            if (this.isStepFailed(index)) {
              labelProps.error = true;
            }
            if (this.isStepSkipped(index)) {
              props.completed = false;
            }
            return (
              <Step key={label} {...props}>
                <StepLabel {...labelProps}>{label}</StepLabel>
              </Step>
            );
          })}
        </Stepper>

        <Grid container spacing={24}>
          <Grid item xs={12} style={{marginTop: 5}}>
            <Paper className="getstartedpagepaper">

              {this.getStepContent(activeStep)}
              <Grid item xs={12} style={{marginTop: 10}} align="center">
                <Button
                  disabled={activeStep === 0}
                  onClick={this.handleBack}
                  className={classes.button}
                >
                  Back
                </Button>
                {this.isStepOptional(activeStep) && (
                  <Button
                    variant="raised"
                    color="primary"
                    onClick={this.handleSkip}
                    className={classes.button}
                  >
                    Skip
                  </Button>
                )}
                <Button
                  variant="raised"
                  color="primary"
                  onClick={this.handleNext}
                  className={classes.button}
                >
                  {activeStep === steps.length - 1 ? 'Finish' : 'Next'}
                </Button>
              </Grid>
            </Paper>
          </Grid>
        </Grid>
      </div>
    );
  }
}

export default withStyles(styles)(Register);
