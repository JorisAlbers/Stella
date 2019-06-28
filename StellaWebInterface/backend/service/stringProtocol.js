class StringProtocol {
  constructor() {
    this.PACKAGE_HEADER_BYTES = 4;
    this._stringBuilder = '';
    this._numberOfPackages = 0;
    this._packagesReceived = 0;
    this.message = null;
  }

  // Convert the string to an array of byte arrays
  serialize(message, maxPackageSize) {
    const data = new Buffer(message, 'ascii');

    const bytesAvailablePerPackage = maxPackageSize - this.PACKAGE_HEADER_BYTES;
    const packagesNeeded = (data.length + bytesAvailablePerPackage - 1) / bytesAvailablePerPackage;

    const returnData = [];
    // Create first package
    returnData.push(this.createPackage(data, 0, maxPackageSize, packagesNeeded));
    for (let i = 1; i < packagesNeeded; i++) {
      returnData.push(this.createPackage(data, i * bytesAvailablePerPackage, maxPackageSize, i));
    }
    return returnData;
  }

  /// <summary>
  ///  Create a subsequent package with a normal package header
  /// </summary>
  createPackage(data, startIndex, maxPackageSize, headerCounter) {
    const buffer = new Buffer([Math.min(data.length - startIndex + this.PACKAGE_HEADER_BYTES, maxPackageSize + this.PACKAGE_HEADER_BYTES)]);

    buffer.writeInt32LE(headerCounter, 0);

    data.copy(buffer, 4, 0, buffer.length);
    return buffer;
  }

  /// <summary>
  /// Returns true when the string has been fully deserialized
  /// </summary>
  /// <param name="package"></param>
  /// <returns></returns>
  deserialize(data) {
    if (this._packagesReceived === 0) {
      this._numberOfPackages = data.readUInt32LE();
    }

    this._stringBuilder += data.toString('ascii');
    this._packagesReceived++;

    if (this._packagesReceived === this._numberOfPackages) {
      this.message = this._stringBuilder;
    }
  }
}

module.exports = StringProtocol;