using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Microsoft.Maui.Controls;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models
{
    /// <summary>
    /// Class that represents a point on the map
    /// </summary>
    public class MapsuiServicePointModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? IconPath { get; set; }
        public string? IconName { get; set; }
        public string? IconDescription { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.MinValue;

        public bool IsOk { get; set; } = true;

        public MapsuiServicePointModel(double latitude, double longitude, double elevation = Double.MinValue)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public MapsuiServicePointModel(string latitude, string longitude, string elevation = "")
        {
            if (double.TryParse(latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
             double.TryParse(longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
            {
                Latitude = lat;
                Longitude = lon;
                if (!String.IsNullOrEmpty(elevation))
                {
                    double.TryParse(elevation, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double Elevation);
                }
            }
            else
            {
                throw new ArgumentException("Invalid latitude or longitude format");
            }
        }

        /// <summary>
        /// Method to convert the point to a Mapsui Feature from Mapsui.Layers.PointFeature
        /// </summary>
        /// <returns></returns>
        public Mapsui.Layers.PointFeature ToPointFeature()
        {
            //.ToMPoint() - is Mapsui.Extensions method
            //Mapsui.IFeature feature = new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
            //feature["name"] = c.Name;
            return new PointFeature(SphericalMercator.FromLonLat(this.Longitude, this.Latitude).ToMPoint());
        }

        /// <summary>
        /// Method to convert the point to a Mapsui.MPoint
        /// </summary>
        /// <returns></returns>
        public Mapsui.MPoint ToMPoint()
        {
            return SphericalMercator.FromLonLat(this.Longitude, this.Latitude).ToMPoint();
        }

        /// <summary>
        /// Provide conversion of current point to a Mapsui.Layers.PointFeature with styles for displaying on the map.
        /// </summary>
        /// <returns></returns>
        public Mapsui.Layers.PointFeature ToStartPointOnMap()
        {
            Mapsui.Layers.PointFeature onMapPointFeature = ToPointFeature();
            onMapPointFeature.Styles = new System.Collections.Generic.List<Mapsui.Styles.IStyle>
            {
                CreateRedFlagStyle(size: 0.8) //ADDED: to use realistic red flag with pole

                //new Mapsui.Styles.SymbolStyle
                //{
                //    SymbolType = Mapsui.Styles.SymbolType.Triangle,
                //    SymbolScale = 0.4,
                //    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                //    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Black, 1.0)
                //}
            };
            return onMapPointFeature;
        }

        /// <summary>
        /// Provide conversion of current point to a TRACKPOINT on route
        /// </summary>
        /// <returns></returns>
        public Mapsui.Layers.PointFeature ToMiddleTrackPointOnMap()
        {
            Mapsui.Layers.PointFeature onMapPointFeature = ToPointFeature();
            onMapPointFeature.Styles = new System.Collections.Generic.List<Mapsui.Styles.IStyle>
            {
                new Mapsui.Styles.SymbolStyle
                {
                    SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                    SymbolScale = 0.2,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Black, 1.0)
                }
            };

            return onMapPointFeature;
        }

        public Mapsui.Layers.PointFeature ToFinishPointOnMap()
        {
            Mapsui.Layers.PointFeature onMapPointFeature = ToPointFeature();
            onMapPointFeature.Styles = new System.Collections.Generic.List<Mapsui.Styles.IStyle>
            {
                //Mapsui.Styles.IStyle type
                CreateCheckeredFlagStyle(size: 0.8)
            };
            return onMapPointFeature;
        }

        /// <summary>
        /// ADDED: to create realistic red flag with pole START POINT
        /// </summary>
        /// <returns></returns>
        private Mapsui.Styles.IStyle CreateRedFlagStyle(double size = 0.4)
        {
            // Create a 64x64 image for red flag with pole
            using SkiaSharp.SKBitmap bitmap = new SkiaSharp.SKBitmap(64, 64);
            using SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(bitmap);
            canvas.Clear(SkiaSharp.SKColors.Transparent);

            // Draw shorter pole
            using SkiaSharp.SKPaint polePaint = new SkiaSharp.SKPaint
            {
                Color = SkiaSharp.SKColors.Black,
                StrokeWidth = 3,
                Style = SkiaSharp.SKPaintStyle.Stroke
            };
            canvas.DrawLine(32, 34, 32, 64, polePaint); // ADDED: shorter pole (22 pixels long)

            // Draw wavy red flag
            using SkiaSharp.SKPaint flagPaint = new SkiaSharp.SKPaint
            {
                Color = SkiaSharp.SKColors.Red,
                Style = SkiaSharp.SKPaintStyle.Fill
            };
            SkiaSharp.SKPath flagPath = new SkiaSharp.SKPath();
            flagPath.MoveTo(32, 34); // Top-left at pole +18
            // Top edge with two waves using cubic Bezier
            flagPath.CubicTo(36, 32, 40, 36, 44, 34);
            flagPath.CubicTo(48, 32, 52, 36, 56, 34);
            // Right edge
            flagPath.LineTo(56, 50);
            // Bottom edge with two waves
            flagPath.CubicTo(52, 52, 48, 48, 44, 50);
            flagPath.CubicTo(40, 52, 36, 48, 32, 50);
            flagPath.Close();
            canvas.DrawPath(flagPath, flagPaint);

            // Add black outline to flag
            using SkiaSharp.SKPaint outlinePaint = new SkiaSharp.SKPaint
            {
                Color = SkiaSharp.SKColors.Black,
                StrokeWidth = 1,
                Style = SkiaSharp.SKPaintStyle.Stroke
            };
            canvas.DrawPath(flagPath, outlinePaint);

            // Convert SkiaSharp bitmap to byte array
            using SkiaSharp.SKImage image = SkiaSharp.SKImage.FromBitmap(bitmap);
            using SkiaSharp.SKData data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
            byte[] imageBytes = data.ToArray();

            // Register bitmap with Mapsui
            int bitmapId = Mapsui.Styles.BitmapRegistry.Instance.Register(imageBytes);

            return new Mapsui.Styles.SymbolStyle
            {
                BitmapId = bitmapId,
                SymbolScale = size,
                SymbolOffset = new Mapsui.Styles.Offset(0, 19) // ADDED: adjusted for shorter pole
            }; //CHANGED
        }

        /// <summary>
        /// for creating checkered flag bitmap style
        /// </summary>
        /// <returns></returns>
        private Mapsui.Styles.IStyle CreateCheckeredFlagStyle(double size = 0.4)
        {
            // Create a 64x64 image for checkered flag with pole
            using SkiaSharp.SKBitmap bitmap = new SkiaSharp.SKBitmap(64, 64);
            using SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(bitmap);
            canvas.Clear(SkiaSharp.SKColors.Transparent);

            // Draw shorter pole
            using SkiaSharp.SKPaint polePaint = new SkiaSharp.SKPaint
            {
                Color = SkiaSharp.SKColors.Black,
                StrokeWidth = 3,
                Style = SkiaSharp.SKPaintStyle.Stroke
            };
            canvas.DrawLine(32, 34, 32, 64, polePaint); // ADDED: shorter pole (22 pixels long)

            // Create wavy flag path
            SkiaSharp.SKPath flagPath = new SkiaSharp.SKPath();
            flagPath.MoveTo(32, 34); // Top-left at pole +18
            // Top edge with two waves using cubic Bezier
            flagPath.CubicTo(36, 32, 40, 36, 44, 34);
            flagPath.CubicTo(48, 32, 52, 36, 56, 34);
            // Right edge
            flagPath.LineTo(56, 50);
            // Bottom edge with two waves
            flagPath.CubicTo(52, 52, 48, 48, 44, 50);
            flagPath.CubicTo(40, 52, 36, 48, 32, 50);
            flagPath.Close();

            // Draw checkered pattern within flag path
            using SkiaSharp.SKPaint blackPaint = new SkiaSharp.SKPaint { Color = SkiaSharp.SKColors.Black };
            using SkiaSharp.SKPaint whitePaint = new SkiaSharp.SKPaint { Color = SkiaSharp.SKColors.White };
            int squareSize = 4;
            canvas.Save();
            canvas.ClipPath(flagPath); // Clip to wavy flag shape
            for (int x = 32; x < 56; x += squareSize)
            {
                for (int y = 34; y < 50; y += squareSize)
                {
                    bool isBlack = (x / squareSize + y / squareSize) % 2 == 0;
                    canvas.DrawRect(x, y, squareSize, squareSize, isBlack ? blackPaint : whitePaint);
                }
            }
            canvas.Restore();

            // Add black outline to flag
            using SkiaSharp.SKPaint outlinePaint = new SkiaSharp.SKPaint
            {
                Color = SkiaSharp.SKColors.Black,
                StrokeWidth = 1,
                Style = SkiaSharp.SKPaintStyle.Stroke
            };
            canvas.DrawPath(flagPath, outlinePaint);

            // Convert SkiaSharp bitmap to byte array
            using SkiaSharp.SKImage image = SkiaSharp.SKImage.FromBitmap(bitmap);
            using SkiaSharp.SKData data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
            byte[] imageBytes = data.ToArray();

            // Register bitmap with Mapsui
            int bitmapId = Mapsui.Styles.BitmapRegistry.Instance.Register(imageBytes);

            return new Mapsui.Styles.SymbolStyle
            {
                BitmapId = bitmapId,
                SymbolScale = size,
                SymbolOffset = new Mapsui.Styles.Offset(0, 19) // ADDED: adjusted for shorter pole
            }; //CHANGED
        }
    }
}