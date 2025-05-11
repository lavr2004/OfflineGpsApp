using System.Text;
using Moq;
using OfflineGpsApp.CodeBase.App.Settings;
using OfflineGpsApp.CodeBase.App.Settings.SettingsAbstractions;

namespace OfflineGpsApp.Tests.CodeBase.App.Settings
{
    public class FileSystemSettingsTests
    {
        private readonly Mock<IFilesSystemActions> _fileSystemMock;

        public FileSystemSettingsTests()
        {
            _fileSystemMock = new Mock<IFilesSystemActions>();
        }

        [Fact]
        public async Task GetReadStringFromMauiAsset_ReturnsFileContent_WhenFileExists()
        {
            // Arrange
            string fileName = "AboutAssets.txt";
            string expectedContent = "Test content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContent));
            _fileSystemMock.Setup(fs => fs.OpenAppPackageFileAsync(fileName)).ReturnsAsync(stream);

            // Act
            var result = await FileSystemSettings.GetReadStringFromMauiAsset(fileName, _fileSystemMock.Object);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task GetReadStringFromMauiAsset_ReturnsNull_WhenFileNotFound()
        {
            // Arrange
            string fileName = "NonExistent.txt";
            _fileSystemMock.Setup(fs => fs.OpenAppPackageFileAsync(fileName)).ReturnsAsync((Stream?)null);

            // Act
            var result = await FileSystemSettings.GetReadStringFromMauiAsset(fileName, _fileSystemMock.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetReadStringFromMauiAsset_UsesDefaultFileName_WhenFileNameNotProvided()
        {
            // Arrange
            string defaultFileName = "AboutAssets.txt";
            string expectedContent = "Default content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContent));
            _fileSystemMock.Setup(fs => fs.OpenAppPackageFileAsync(defaultFileName)).ReturnsAsync(stream);

            // Act
            var result = await FileSystemSettings.GetReadStringFromMauiAsset(null, _fileSystemMock.Object);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task GetReadStringFromMauiAsset_ReturnsNull_WhenExceptionOccurs()
        {
            // Arrange
            string fileName = "AboutAssets.txt";
            _fileSystemMock.Setup(fs => fs.OpenAppPackageFileAsync(fileName)).ThrowsAsync(new IOException("IO error"));

            // Act
            var result = await FileSystemSettings.GetReadStringFromMauiAsset(fileName, _fileSystemMock.Object);

            // Assert
            Assert.Null(result);
        }
    }
}
