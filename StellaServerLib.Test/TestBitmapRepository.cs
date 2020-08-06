using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
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

        [Test]
        public void ListBitmaps_DirectoryWithFiles_ListOnlyBitmaps()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\bitmapFolder\bitmap.png", new MockFileData("some js") },
                { @"c:\bitmapFolder\not_a_bitmap.txt", new MockFileData(new byte[] { 255 }) },
                { @"c:\bitmapFolder\folder\not_a_bitmap.txt", new MockFileData(new byte[] { 255 }) },
                { @"c:\bitmapFolder\folder\bitmap.png", new MockFileData(new byte[] { 255 }) },
            });

            BitmapRepository repository = new BitmapRepository(fileSystem, "c:\\BitmapFolder");
            var bitmaps = repository.ListAllBitmaps();
            CollectionAssert.AreEquivalent(new string[]{"bitmap", "folder\\bitmap"}, bitmaps);
        }
    }
}
