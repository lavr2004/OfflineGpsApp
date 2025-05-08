using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OfflineGpsApp.CodeBase.Service.LayersService
{
    public class GpxTrackLayerFactory
    {
        private readonly string _gpxFilePath;

        public GpxTrackLayerFactory(string gpxFilePath)
        {
            _gpxFilePath = gpxFilePath ?? throw new ArgumentNullException(nameof(gpxFilePath));
        }

        public async Task<ILayer> CreateTrackLayerAsync(string GpxTrackLayerTitle = "GpxTrackLayer")
        {
            // Load and parse GPX file
            var points = await LoadGpxPointsAsync();

            // Convert to Mapsui geometry
            var lineString = new LineString(points.Select(p => new Coordinate(p.Longitude, p.Latitude)).ToArray());
            var feature = new GeometryFeature(lineString);

            // Create MemoryLayer
            var layer = new MemoryLayer(GpxTrackLayerTitle)
            {
                Features = new[] { feature },
                Style = new VectorStyle
                {
                    Line = new Pen { Color = Mapsui.Styles.Color.Red, Width = 3 }
                }
            };

            return layer;
        }

        private async Task<List<(double Latitude, double Longitude)>> LoadGpxPointsAsync()
        {
            if (!File.Exists(_gpxFilePath))
            {
                throw new FileNotFoundException("ER - GPX file not found", _gpxFilePath);
            }

            // Read the GPX file content
            string gpxContent = await File.ReadAllTextAsync(_gpxFilePath);
            if (string.IsNullOrWhiteSpace(gpxContent))
            {
                throw new InvalidOperationException("ER - GPX file is empty or invalid");
            }

            // Parse GPX content to extract track points
            CodeBase.Service.GpxParserService.GpxParserService gpxParser = new CodeBase.Service.GpxParserService.GpxParserService();
            List<(double Latitude, double Longitude)> LatLonTupleList = gpxParser.process_parseTrackPointsGpxToTupleList_fc(gpxContent);

            // Simulate GPX parsing (replace with HammerParserLibrary)
            // Example: Assume GPX contains track points
            //var points = new List<(double Latitude, double Longitude)>();
            //using (var stream = File.OpenRead(_gpxFilePath))
            //{
            //    // TODO: Use HammerParserLibrary to parse GPX
            //    // For now, return dummy points
            //    points.Add((52.31, 20.30));
            //    points.Add((52.32, 20.31));
            //    points.Add((52.33, 20.32));
            //}

            return LatLonTupleList;
        }
    }
}
