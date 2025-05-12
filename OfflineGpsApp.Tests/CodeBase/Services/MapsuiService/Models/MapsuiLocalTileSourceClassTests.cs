using BruTile;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

namespace OfflineGpsApp.Tests.CodeBase.Services.MapsuiService.Models
{
    public class MapsuiLocalTileSourceClassTests
    {
        private readonly string _tempDir;

        public MapsuiLocalTileSourceClassTests()
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
            var tileSource = new MapsuiServiceLocalTileSourceModel(_tempDir); //CHANGED: pass tempDir to constructor

            // Mock HttpClient
            typeof(MapsuiServiceLocalTileSourceModel)
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
            var tileSource = new MapsuiServiceLocalTileSourceModel(_tempDir); //CHANGED: pass tempDir to constructor

            // Mock HttpClient
            typeof(MapsuiServiceLocalTileSourceModel)
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
            var tileSource = new MapsuiServiceLocalTileSourceModel(_tempDir); //CHANGED: pass tempDir to constructor

            // Mock HttpClient
            typeof(MapsuiServiceLocalTileSourceModel)
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
