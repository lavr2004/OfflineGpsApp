using Mapsui.Layers;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using OfflineGpsApp.CodeBase.Service.LayersService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.Tests.CodeBase.Service.LayersService
{
    public class GpxTrackLayerFactoryTests
    {
        private readonly string _tempDir;

        public GpxTrackLayerFactoryTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        [Fact]
        public async Task CreateTrackLayerAsync_ValidGpxFile_ReturnsMemoryLayerWithLineString()
        {
            // Arrange
            var gpxFilePath = Path.Combine(_tempDir, "test.gpx");
            File.WriteAllText(gpxFilePath, "<gpx></gpx>"); // Dummy GPX file
            var factory = new GpxTrackLayerFactory(gpxFilePath);

            // Act
            var layer = await factory.CreateTrackLayerAsync(GpxTrackLayerTitle: "GpxTrackLayer");

            // Assert
            Assert.NotNull(layer);
            Assert.IsType<MemoryLayer>(layer);
            Assert.Equal("GpxTrackLayer", layer.Name);
            var memoryLayer = (MemoryLayer)layer;
            Assert.Single(memoryLayer.Features);
            var feature = memoryLayer.Features.First();
            Assert.IsType<GeometryFeature>(feature);
            Assert.IsType<LineString>(((GeometryFeature)feature).Geometry);
        }

        [Fact]
        public async Task CreateTrackLayerAsync_FileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var gpxFilePath = Path.Combine(_tempDir, "nonexistent.gpx");
            var factory = new GpxTrackLayerFactory(gpxFilePath);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => factory.CreateTrackLayerAsync());
        }

        [Fact]
        public void Constructor_NullFilePath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GpxTrackLayerFactory(null));
        }
    }
}
