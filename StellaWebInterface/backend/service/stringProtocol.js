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
    const data = new Buffer.from(message, 'ascii');

    const bytesAvailablePerPackage = maxPackageSize - this.PACKAGE_HEADER_BYTES;
    const packagesNeeded = Math.floor((data.length + bytesAvailablePerPackage - 1) / bytesAvailablePerPackage);

    const returnData = [];
    // Create first package
    returnData.push(this.createPackage(data, 0, bytesAvailablePerPackage, packagesNeeded));
    for (let i = 1; i < packagesNeeded; i++) {
      returnData.push(this.createPackage(data, i * bytesAvailablePerPackage, bytesAvailablePerPackage, i));
    }
    return returnData;
  }

  /// <summary>
  ///  Create a subsequent package with a normal package header
  /// </summary>
  createPackage(data, startIndex, maxBodySize, headerCounter) {
    const packageSize = Math.min(maxBodySize + this.PACKAGE_HEADER_BYTES, data.length - startIndex + this.PACKAGE_HEADER_BYTES);
    const buffer = new Buffer.alloc(packageSize);

    buffer.writeInt32LE(headerCounter, 0);
    data.copy(buffer, 4, startIndex, buffer.length - 4);
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

    this._stringBuilder += data.slice(4).toString();

    this._packagesReceived++;

    if (this._packagesReceived === this._numberOfPackages) {
      this.message = this._stringBuilder;
    }
  }
}

module.exports = StringProtocol;