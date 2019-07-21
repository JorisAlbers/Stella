const fs = require("fs");
const PImage = require('pureimage');

class convertMp4ToRow {
  constructor(pathToVideo = './savedData/example-video.mp4') {
    this.videoFile = null;
    if (fs.existsSync(pathToVideo)) {
      this.videoFile = fs.readFileSync(pathToVideo);
    }


  }

  // saveAnimation() {
  //   const video = new PImage;
  // }
}

module.exports = convertMp4ToRow;