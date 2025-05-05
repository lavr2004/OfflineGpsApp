using BruTile;
using Moq;
using Moq.Protected;
using OfflineGpsApp.CodeBase.Service.LayersService;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OfflineGpsApp.Tests
{
    public class OpenStreetMapLocalTileSourceTests
    {
        private readonly string _tempDir;

        public OpenStreetMapLocalTileSourceTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        [Fact]
        public async Task GetTileAsync_LocalTileExists_ReturnsTileData()
        {
            // Arrange
            var tileInfo = new TileInfo { Index = new TileIndex(0, 0, 0) };
            var tilePath = Path.Combine(_tempDir, "tiles", "0", "0", "0.png");
            Directory.CreateDirectory(Path.GetDirectoryName(tilePath));
            byte[] expectedData = new byte[] { 1, 2, 3 };
            await File.WriteAllBytesAsync(tilePath, expectedData);

            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(mockHandler.Object);
            var tileSource = new OpenStreetMapLocalTileSource();

            // Mock FileSystem.AppDataDirectory
            typeof(FileSystem).GetField("AppDataDirectory", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(null, _tempDir);

            // Mock HttpClient
            typeof(OpenStreetMapLocalTileSource)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tileSource, httpClient);

            // Act
            var result = await tileSource.GetTileAsync(tileInfo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData, result);
            mockHandler.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetTileAsync_LocalTileDoesNotExist_DownloadsAndSavesTile()
        {
            // Arrange
            var tileInfo = new TileInfo { Index = new TileIndex(0, 0, 0) };
            var tilePath = Path.Combine(_tempDir, "tiles", "0", "0", "0.png");
            byte[] expectedData = new byte[] { 1, 2, 3 };

            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "https://tile.openstreetmap.org/0/0/0.png"),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(expectedData)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var tileSource = new OpenStreetMapLocalTileSource();

            // Mock FileSystem.AppDataDirectory
            typeof(FileSystem).GetField("AppDataDirectory", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(null, _tempDir);

            // Mock HttpClient
            typeof(OpenStreetMapLocalTileSource)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tileSource, httpClient);

            // Act
            var result = await tileSource.GetTileAsync(tileInfo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData, result);
            Assert.True(File.Exists(tilePath));
            Assert.Equal(expectedData, await File.ReadAllBytesAsync(tilePath));
            mockHandler.VerifyAll();
        }

        [Fact]
        public async Task GetTileAsync_DownloadFails_ReturnsNull()
        {
            // Arrange
            var tileInfo = new TileInfo { Index = new TileIndex(0, 0, 0) };
            var tilePath = Path.Combine(_tempDir, "tiles", "0", "0", "0.png");

            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "https://tile.openstreetmap.org/0/0/0.png"),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Download failed"));

            var httpClient = new HttpClient(mockHandler.Object);
            var tileSource = new OpenStreetMapLocalTileSource();

            // Mock FileSystem.AppDataDirectory
            typeof(FileSystem).GetField("AppDataDirectory", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(null, _tempDir);

            // Mock HttpClient
            typeof(OpenStreetMapLocalTileSource)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tileSource, httpClient);

            // Act
            var result = await tileSource.GetTileAsync(tileInfo);

            // Assert
            Assert.Null(result);
            Assert.False(File.Exists(tilePath));
            mockHandler.VerifyAll();
        }
    }

}