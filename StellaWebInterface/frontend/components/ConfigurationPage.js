import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Paper from '@material-ui/core/Paper/index';
import Typography from '@material-ui/core/Typography/index';

const ConfigurationPage = () => (
  <React.Fragment>
    <Grid container spacing={24}>
      <Grid item xs={12} style={{marginTop: 10}}>
        <Paper className="homepagepaper">
          <Typography variant="body1" gutterBottom align="center">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the
            industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and
            scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into
            electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of
            Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like
            Aldus PageMaker including versions of Lorem Ipsum.
          </Typography>
        </Paper>
      </Grid>
    </Grid>
  </React.Fragment>
);

export default ConfigurationPage;
