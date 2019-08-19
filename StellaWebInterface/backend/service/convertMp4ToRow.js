const fs = require("fs");
const beamcoder = require('beamcoder');

class convertMp4ToRow {
  constructor(data) {
    this.animation = {
      class: 'Bitmap',
      startIndex: 0,
      stripLength: 3200, //todo make dynamic
      frameWaitMs: 5, //todo make dynamic
      relativeStart: 0,
      brightness: 0,
      rgbValues: [0.0, 0],
      imageName: '',
      wraps: true
    };
    this.videoFile = data;
    this.config = this.getMappingConfiguration();
    // if (fs.existsSync(`./savedData/${data.FileName}`)) {
    //   this.videoFile = fs.readFileSync(`./savedData/${data.FileName}`);
    // }
  }

  GetRGBPixel(frame, x, y) {
    // Y component
    // noinspection JSRedeclarationOfBlockScope
    let y = frame.data[0][frame.linesize[0] * y + x];

    // U, V components
    x /= 2;
    y /= 2;
    const u = frame.data[1][frame.linesize[1] * y + x];
    const v = frame.data[2][frame.linesize[2] * y + x];

    // RGB conversion
    const r = y + 1.402 * (v - 128);
    const g = y - 0.344 * (u - 128) - 0.714 * (v - 128);
    const b = y + 1.772 * (u - 128);

    return [r, g, b];
  };

  /**
   * Reads the configuration and parses it
   * @returns {Buffer|Boolean}
   */
  getMappingConfiguration() {
    if (fs.existsSync('./savedData/savedLedMapping.json')) {
      return JSON.parse(fs.readFileSync('./savedData/savedLedMapping.json'));
    } else throw {
      name: 'Configuration not found',
      message: 'Configuration is not found in \'./savedData/savedLedMapping.json\'.'
    };
  }

  saveAnimation() {


  }
  //   try {
  //     const process = new ffmpeg(`./savedData/example-video.mp4`);
  //     process.then((video) => {
  //       video.fnExtractFrameToJPG('./savedData/frames', {
  //         frame_rate : 1,
  //         // number : 5,
  //         file_name : 'my_frame_%t_%s'
  //       }, function (error, files) {
  //         if (!error)
  //           console.log('Frames: ' + files);
  //       });
  //
  //       console.log('The video is ready to be processed');
  //     }, (err) => {
  //       console.log('Error: ' + err);
  //     });
  //   } catch (e) {
  //     console.log(e.code);
  //     console.log(e.msg);
  //   }
  // }
}

module.exports = convertMp4ToRow;