import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import {socketConnect} from 'socket.io-react';
import AceEditor from 'react-ace';
import Button from "@material-ui/core/Button";
// Import some tools
import "brace/ext/language_tools";
// Import the theme
import 'brace/theme/monokai';
// Import some languages so the user can choose some
// import 'brace/mode/java';
import 'brace/mode/javascript';

class EditorPage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.state = {
      // status: null,
      // storyboards: null
    };
    this.aceEditorValue = `alert("hello world");`;

    // this.props.socket.on('status', (status) => this.setState({status}));
    // this.props.socket.on('availableStoryboards', (storyboards) => this.setState({storyboards}));
  }

  componentDidMount() {
    // this.props.socket.emit('status');
    // this.props.socket.emit('availableStoryboards');
  }

  runCode() {
    /**
     * Yes, this function is horrible I know.
     *
     * To the person who might want to hire me and looks at this horrible code.. I know this 'eval()' function is
     * horrible and that you generally don't want it anywhere near an application. I know that I should push whatever is
     * in `this.aceEditorValue` to a sandboxed Iframe and that I shouldn't eval unknown user inputted  code in my own
     * application.
     *
     * This code is never to be planned to see the internet but should only be accessible in a local LAN network with a
     * few devices and a local server that only serves to devices inside the network. This way it could never cause any
     * harm to the client/server and it couldn't do a XSS attack.
     */
    eval(this.aceEditorValue);
  }

  onChange(e) {
    this.aceEditorValue = e;
  }

  render() {
    // const {status, storyboards} = this.state;

    return <div>
      <Grid container spacing={0} direction={'row'}>
        <Grid xs={11} item>
          <AceEditor
            placeholder="Placeholder Text"
            mode="javascript"
            theme="monokai"
            name="ace-code-editor-stella"
            onChange={(e) => this.onChange(e)}
            fontSize={14}
            style={{width: '100%', padding: '1px'}}
            showPrintMargin={true}
            showGutter={true}
            highlightActiveLine={true}
            value={this.aceEditorValue}
            setOptions={{
              enableBasicAutocompletion: true,
              enableLiveAutocompletion: true,
              enableSnippets: false,
              showLineNumbers: true,
              tabSize: 2,
            }}/>


        </Grid>
        <Grid xs item>
          <Grid container spacing={3} direction={'column'}>
            <Grid xs item>
              <Button disabled color="inherit" align="left" onClick={() => {}}>Test</Button>
              <Button color="inherit" align="left" onClick={() => this.runCode()}>Run</Button>
              {/*<Button disabled color="inherit" align="left" onClick={() => {}}>Upload</Button>*/}
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </div>;
  }
}

export default socketConnect(EditorPage);
