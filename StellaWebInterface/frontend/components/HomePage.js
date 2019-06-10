import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Typography from '@material-ui/core/Typography/index';

const HomePage = () => (
  <div className="landingPagebodyComponent">

    <br/>
    <Typography variant="display3" gutterBottom align="center">
      Welcome to React Company
    </Typography>

    <Grid container spacing={24}>
      <Grid item xs={12} md={12}>
        <Typography variant="body2" gutterBottom align="center">
          Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here,
          content here', making it look like readable English. Many desktop publishing packages and web page editors now
          use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still
          in their infancy
        </Typography>
      </Grid>
      <Grid item xs={12}>

      </Grid>
    </Grid>

    <Grid container spacing={24}>
      <Grid item xs={12} md={12}>

      </Grid>
    </Grid>

  </div>
);

export default HomePage;
