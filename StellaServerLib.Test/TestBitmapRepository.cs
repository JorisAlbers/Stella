using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using Moq;
using NUnit.Framework;

namespace StellaServerLib.Test
{
    public class TestBitmapRepository
    {
        [Test]
        public void Ctor_DirectoryDoesNotExist_ThrowsArgumentException()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            var directoryMock = new Mock<IDirectory>();
            directoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(x => x.Directory).Returns(directoryMock.Object);

            Assert.Throws<ArgumentException>(() => new BitmapRepository(fileSystemMock.Object, "some path"));
        }
    }
}
