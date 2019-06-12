import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Button from "@material-ui/core/Button";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import InputLabel from "@material-ui/core/InputLabel";
import FormControl from "@material-ui/core/FormControl";
import OutlinedInput from "@material-ui/core/OutlinedInput";
import {socketConnect} from 'socket.io-react';
import AceEditor from 'react-ace';
// Import tools
import "brace/ext/language_tools";
// Import the theme
import 'brace/theme/monokai';
// Import some languages so the user can choose some
import 'brace/mode/javascript';
import 'brace/mode/python';

class EditorPage extends React.Component {
  constructor(props) {
    super(props);
    this.props = props;
    this.state = {
      error: null,
      finalObject: null,
      modus: 'javascript'
    };
    this.code = {
      javascript: `// Javascript sample code \nreturn {a: "foo", b: {c:"bar", d:"buzz"}};`,
      python: `# Python sample code \nreturn {a: "foo", b: {c:"bar", d:"buzz"}};`
    };
  }

  runCode() {
    /**
     * Yes, this function is horrible I know.
     *
     * To the person who might want to hire me and looks at this horrible code.. I know this -'eval()'- 'new Function()' function is
     * horrible and that you generally don't want it anywhere near an application. I know that I should push whatever is
     * in `this.aceEditorValue` to a sandboxed Iframe and that I shouldn't eval unknown user inputted  code in my own
     * application.
     *
     * This code is never to be planned to see the internet but should only be accessible in a local LAN network with a
     * few devices and a local server that only serves to devices inside the network. This way it could never cause any
     * harm to the client/server and it couldn't do a XSS attack.
     */
    switch (this.state.modus) {
      case "javascript":
        try {
          this.setState({error: null, finalObject: new Function(this.code[this.state.modus])()});
        } catch (e) {
          this.setState({error: e.message, finalObject: null})
        }
        break;
      case "python":
        this.setState({error: null, finalObject: "Python doesn't have a compiler yet"});
        break;
      case "draw":
        this.setState({error: null, finalObject: "Draw hasn't been fully implemented yet"});
        break;
    }
  }

  // TODO @Martijn check if key exist, otherwise print alternative.
  // TODO @Robert style the outcome
  recursiveFunctionToRenderObjectsAndArrays(object, key = '') {
    switch (typeof object) {
      case "undefined":
        return <p>{key}: {'undefined'}</p>;
      case "bigint":
      case "number":
      case "string":
      case "symbol":
        return <p>{key}: {object}</p>;
      case "boolean":
        return object ? <p>{key}: true</p> : <p>{key}: false</p>;
      case "function":
        return <p>{key}: {'A Function has been returned this is not allowed'}</p>;
      case "object":
        let items = [];
        for (let subObject in object) {
          if (object.hasOwnProperty(subObject)) {
            items.push(this.recursiveFunctionToRenderObjectsAndArrays(object[subObject], subObject));
          }
        }
        return <React.Fragment><span>{key}: </span>
          <div>{items}</div>
        </React.Fragment>;
    }
  }

  render() {
    const {error, finalObject, modus} = this.state;

    console.log(modus);
    return <div>
      <Grid container spacing={0} direction={'column'}>
        <Grid xs={1} item>
          <FormControl variant="outlined">
            <InputLabel htmlFor="outlined-age-simple">
              Modus:
            </InputLabel>
            <Select
              value={modus}
              onChange={(e) => this.setState({modus: e.target.value})}
              input={<OutlinedInput labelWidth={120} name="age" id="outlined-age-simple"/>}
            >
              <MenuItem value={"javascript"}>Javascript</MenuItem>
              <MenuItem value={"python"}>Python</MenuItem>
              <MenuItem value={"draw"}>Draw</MenuItem>
            </Select>
          </FormControl>
        </Grid>
        <Grid xs={11} item>
          {(modus === "javascript" || modus === 'python') &&
          <Grid container spacing={0} direction={'row'}>
            <Grid xs={11} item>
              <AceEditor
                placeholder="Placeholder Text"
                mode={modus}
                theme="monokai"
                name="ace-code-editor-stella"
                onChange={(e) => this.code[this.state.modus] = e}
                fontSize={14}
                style={{width: '100%', padding: '1px'}}
                showPrintMargin={true}
                showGutter={true}
                highlightActiveLine={true}
                value={this.code[this.state.modus]}
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
                  <Button disabled color="inherit" align="left" onClick={() => {
                  }}>Test</Button>
                  <Button color="inherit" align="left" onClick={() => this.runCode()}>Run</Button>
                  {/*<Button disabled color="inherit" align="left" onClick={() => {}}>Upload</Button>*/}
                </Grid>
              </Grid>
            </Grid>
          </Grid>
          }
        </Grid>
        <Grid xs={2} item>
          {error &&
          <React.Fragment>
            <h2>Error: </h2>
            <p>{error}</p>
          </React.Fragment>
          }
          {finalObject &&
          <React.Fragment>
            <h2>finalObject: </h2>
            {this.recursiveFunctionToRenderObjectsAndArrays(finalObject)}
          </React.Fragment>
          }
        </Grid>
      </Grid>
    </div>;
  }
}

export default socketConnect(EditorPage);
