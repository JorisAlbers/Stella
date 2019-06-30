import React from 'react';
import Grid from '@material-ui/core/Grid/index';
import Button from "@material-ui/core/Button";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import InputLabel from "@material-ui/core/InputLabel";
import FormControl from "@material-ui/core/FormControl";
import OutlinedInput from "@material-ui/core/OutlinedInput";
import CloudUploadIcon from '@material-ui/icons/CloudUpload';
import {socketConnect} from 'socket.io-react';
import AceEditor from 'react-ace';
// import * as rapydscript from 'rapydscript-ng';
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
      currentMode: 'image',
      modeOptions: [{
        name: 'Javascript',
        machineName: 'javascript',
        enabled: true
      }, {
        name: 'Python',
        machineName: 'python',
        enabled: true
      }, {
        name: 'Draw',
        machineName: 'draw',
        enabled: false
      }, {
        name: 'Youtube',
        machineName: 'youtube',
        enabled: true
      }, {
        name: 'Vimeo',
        machineName: 'vimeo',
        enabled: true
      }, {
        name: 'Image',
        machineName: 'image',
        enabled: true
      }],
      imageFile: null
    };
    this.code = {
      javascript: `// Javascript sample code \nreturn {a: "foo", b: {c:"bar", d:"brazzers"}};`,
      python: `# Python sample code \nprint('hello world')`,
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
    switch (this.state.currentMode) {
      case "javascript":
        try {
          this.setState({error: null, finalObject: new Function(this.code[this.state.currentMode])()});
        } catch (e) {
          this.setState({error: e.message, finalObject: null})
        }
        break;
      case "python":
        // try {
        // const compiler = rapydscript.create_compiler();
        // this.setState({
        //   error: null,
        //   finalObject: eval(compiler.compile(this.code[this.state.currentMode], {'omit_baselib': false}))
        // });
        // } catch (e) {
        //   this.setState({error: e.message, finalObject: null})
        // }
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
    const {error, finalObject, currentMode, modeOptions} = this.state;

    return <div>
      <Grid container spacing={0} direction={'column'}>
        <Grid xs item>
          <FormControl variant="outlined">
            <InputLabel htmlFor="outlined-age-simple">
              currentMode:
            </InputLabel>
            <Select
              value={currentMode}
              onChange={(e) => this.setState({currentMode: e.target.value})}
              input={<OutlinedInput labelWidth={100} name="age" id="outlined-age-simple"/>}
            >
              {modeOptions.map((option) => {
                return <MenuItem disabled={!option.enabled} key={option.machineName}
                                 value={option.machineName}>{option.name}</MenuItem>
              })}
            </Select>
          </FormControl>
        </Grid>
        <Grid container direction={'row'}>
          {(currentMode === "image") &&
          <Grid xs={11} item>
            <input
              accept="image/*"
              style={{display: 'none'}}
              id="contained-button-file"
              type="file"
              onChange={(e) => {
                const fileReaderBase64 = new FileReader();
                fileReaderBase64.readAsDataURL(e.target.files[0]);
                fileReaderBase64.onload = (result) => {
                  this.setState({imageFileBase64: result});
                  const c = document.getElementById("canvas-class-name");
                  const ctx = c.getContext("2d");
                  const myImage = new Image();
                  myImage.onload = () => {
                    c.width = myImage.width;
                    c.height = myImage.height;
                    ctx.drawImage(myImage, 0, 0, myImage.width, myImage.height, 0, 0, c.width, c.height);
                    const data = ctx.getImageData(0, 0, c.width, c.height);

                    result = new Uint8ClampedArray(data.data.length * 0.75);
                    for (let newIndex = 0, oldIndex = 0; oldIndex < data.data.length; oldIndex++) {
                      if (oldIndex % 4 === 3) continue;
                      result[newIndex++] = data.data[oldIndex]
                    }

                    this.setState({imageFile: {data: result, width: data.width, height: data.height}})
                  };
                  myImage.src = result.target.result;
                };
              }}
            />
            <label htmlFor="contained-button-file">
              <Button variant="contained" component="span">Select file
                <CloudUploadIcon/>
              </Button>
            </label>
            <Button color="inherit" align="left" disabled={!this.state.imageFile}
                    onClick={() => {
                      this.props.socket.emit('sendSingleFrame', this.state.imageFile);
                    }}>Upload</Button>
            <canvas id={'canvas-class-name'} style={{width: '100%'}}/>
          </Grid>
          }
          {(currentMode === "javascript" || currentMode === 'python') &&
          <Grid xs={11} className={'ide-editor'} item>
            <AceEditor
              placeholder="Placeholder Text"
              mode={currentMode}
              theme="monokai"
              name="ace-code-editor-stella"
              onChange={(e) => this.code[currentMode] = e}
              fontSize={14}
              style={{width: '100%', padding: '1px'}}
              showPrintMargin={true}
              showGutter={true}
              highlightActiveLine={true}
              value={this.code[currentMode]}
              setOptions={{
                enableBasicAutocompletion: true,
                enableLiveAutocompletion: true,
                enableSnippets: false,
                showLineNumbers: true,
                tabSize: 2,
              }}/>
          </Grid>
          }
          <Grid xs item>
            <Button disabled color="inherit" align="left">Test</Button>
            <Button color="inherit" align="left" onClick={() => this.runCode()}>Run</Button>
            {/*<Button disabled color="inherit" align="left" onClick={() => {}}>Upload</Button>*/}
          </Grid>
        </Grid>
        <Grid xs item>
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
