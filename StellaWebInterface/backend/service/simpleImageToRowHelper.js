const fs = require("fs");

class simpleImageToAnimationHelper {
  constructor(imageData, numberOfStripsPerRow) {
    this.numberOfLedPerStrip = 120;

    this.imageData = imageData;
    this.numberOfStripsPerRow = numberOfStripsPerRow;

    this.config = this.getMappingConfiguration();
  }

  /**
   * Returns one single pixel parameters.
   * @param x
   * @param y
   * @returns {[Number, Number, Number]}
   */
  getPx(x, y) {
    return [
      this.imageData.data[(y * this.imageData.width + x) * 3],
      this.imageData.data[(y * this.imageData.width + x) * 3 + 1],
      this.imageData.data[(y * this.imageData.width + x) * 3 + 2],
    ];
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

  /**
   * Generates the animation
   * @returns {Array}
   */
  getAnimation() {
    const numberOfStrips = this.config.ledstrips.amount / this.numberOfStripsPerRow;
    let result = [];

    // Make the calculation for each frame
    for (let frame = 0; frame < this.imageData.height; frame++) {
      let rowResult = [];

      // Make the calculation for each row
      for (let row = 0; row < numberOfStrips; row++) {
        let tempRow = row + frame;
        if ((tempRow) > this.imageData.height) {
          tempRow = tempRow - this.imageData.height;
        }

        // Make the calculation for each pixel in the strip
        for (let pixel = 0; pixel < this.numberOfLedPerStrip * this.numberOfStripsPerRow; pixel++) {
          rowResult.push(this.getPx(Math.round(pixel / this.imageData.width * pixel), tempRow))
        }
      }

      // Push the rows to the result.
      result.push(rowResult);
    }
    return result
  }
}

module.exports = simpleImageToAnimationHelper;