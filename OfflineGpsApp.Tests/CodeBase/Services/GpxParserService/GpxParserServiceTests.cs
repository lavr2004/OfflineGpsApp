using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Moq.Protected;
using Xunit;

using nsGpxParserService = OfflineGpsApp.CodeBase.Services.GpxParserService;

namespace OfflineGpsApp.Tests.CodeBase.Services.GpxParserService
{
    public class GpxParserServiceTests
    {
        private readonly nsGpxParserService.GpxParserService _gpxParserService;
        private string gpxContentStr = @"<?xml version='1.0' encoding='utf-8'?>
<ns0:gpx xmlns:ns0=""http://www.topografix.com/GPX/1/1"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" creator=""Garmin Connect"" version=""1.1"" xsi:schemaLocation=""http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/11.xsd"">
  <ns0:metadata>
    <ns0:name>Hiking_Warszawa-Kampinos_40km_FreeMindsWay</ns0:name>
    <ns0:link href=""connect.garmin.com"">
      <ns0:text>Garmin Connect</ns0:text>
    </ns0:link>
    <ns0:time>2025-04-21T09:55:29.000Z</ns0:time>
  </ns0:metadata>
  <ns0:trk>
    <ns0:name>Hiking_Warszawa-Kampinos_40km_FreeMindsWay</ns0:name>
    <ns0:trkseg>
      <ns0:trkpt lat=""52.31022119522095"" lon=""20.76190710067749"">
        <ns0:ele>86.86</ns0:ele>
        <ns0:time>2025-04-21T09:55:29.000Z</ns0:time>
      </ns0:trkpt>
      <ns0:trkpt lat=""52.34967875294387"" lon=""20.303464019671082"">
        <ns0:ele>79.66</ns0:ele>
        <ns0:time>2025-04-21T20:33:21.080Z</ns0:time>
      </ns0:trkpt>
    </ns0:trkseg>
  </ns0:trk></ns0:gpx>";

        public GpxParserServiceTests()
        {
            _gpxParserService = new nsGpxParserService.GpxParserService();
        }

        [Fact]
        public void Process_fc_ValidInput_ReturnsParsedPoints()
        {
            // Arrange
            //string inputStr = "<gpx><trkpt lat=\"52.31\" lon=\"20.30\"></trkpt><trkpt lat=\"52.32\" lon=\"20.31\"></trkpt></gpx>";

            // Act
            List<(double Lattitude, double Longitude)> LatLonTupleList = _gpxParserService.process_parseTrackPointsGpxToTupleList_fc(gpxContentStr);

            // Assert
            Assert.NotNull(LatLonTupleList);
            Assert.Equal(2, LatLonTupleList.Count);
            Assert.Equal(52.31022119522095, LatLonTupleList[0].Lattitude);
            Assert.Equal(20.76190710067749, LatLonTupleList[0].Longitude);
            Assert.Equal(52.34967875294387, LatLonTupleList[1].Lattitude);
            Assert.Equal(20.303464019671082, LatLonTupleList[1].Longitude);
        }

        [Fact]
        public void Process_fc_InvalidInput_ReturnsEmptyList()
        {
            // Arrange
            string inputStr = "<gpx><trkpt></trkpt></gpx>";

            // Act
            var result = _gpxParserService.process_parseTrackPointsGpxToTupleList_fc(inputStr);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
