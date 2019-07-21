const fs = require("fs");
const PImage = require('pureimage');

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
    this.videoFile = null;
    this.config = this.getMappingConfiguration();
    if (fs.existsSync(`./savedData/${data.FileName}`)) {
      this.videoFile = fs.readFileSync(`./savedData/${data.FileName}`);
    }
  }

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
}

module.exports = convertMp4ToRow;