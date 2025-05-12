//using HammerParserLibrary;
//using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;
//using System.Globalization;
//using nsFileSystemSettings = OfflineGpsApp.CodeBase.Services.MapsuiService.Settings.MapsuiFileSystemSettings;

//namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders
//{
//    /// <summary>
//    /// Class that builds GPX tracks that will be used on the map by LayerBuilder
//    /// </summary>
//    public class GPXTrackBuilder
//    {
//        MapsuiServiceTrackModel oMapsuiTrackClass = new MapsuiServiceTrackModel();

//        public GPXTrackBuilder(string gpxFilePath)
//        {
//            List<List<string>> LatLonListList = ParseDataFromGpxFile(gpxFilePath);
//            // Create a new track
//            CreateTrackFromLatLonList(LatLonListList);
//        }

//        private void CreateTrackFromLatLonList(List<List<string>> LatLonListList)
//        {
//            // Create a new track
//            if (LatLonListList.Count == 0)
//            {
//                System.Diagnostics.Debug.WriteLine("ER - No points found in GPX file.");
//                return;
//            }
//            for (int i = 0; i < LatLonListList.Count; i++)
//            {
//                if (LatLonListList[i].Count == 2)
//                {
//                    string lat = LatLonListList[i][0];
//                    string lon = LatLonListList[i][1];

//                    if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lon))
//                    {
//                        System.Diagnostics.Debug.WriteLine($"ER - No points found in GPX file: {lat} {lon}");
//                        return;
//                    }

//                    //normalize lat/lon values for different cultures
//                    lat = lat.Replace(',', '.').Trim();
//                    lon = lon.Replace(',', '.').Trim();

//                    //convert lat/lon values to double
//                    if (Double.TryParse(lat, NumberStyles.Any, CultureInfo.InvariantCulture, out System.Double lattitude) && Double.TryParse(lon, NumberStyles.Any, CultureInfo.InvariantCulture, out System.Double longitude))
//                    {
//                        MapsuiServicePointModel mapsuiPoint = new MapsuiServicePointModel(lattitude, longitude);
//                        mapsuiPoint.Title = $"Point {i + 1}";
//                        oMapsuiTrackClass.AddPointToTrack(mapsuiPoint);
//                    }
//                }
//            }
//        }

//        private List<List<string>> ParseDataFromGpxFile(string gpxFilePath)
//        {
//            HammerParser oHammerParser = new HammerParser();
//            HammerParserConfig snippetsParsingConfig = new HammerParserConfig();
//            HammerParserConfig valuesParsingConfig = new HammerParserConfig();
//            valuesParsingConfig.is_into_one_line_text = true;
//            valuesParsingConfig.is_include_start_end_tags = false;

//            //first snippets loop parsing setup
//            //oHammerParser.config_parse_snippets_fc("wpt ", "wpt>", snippetsParsingConfig);
//            //oHammerParser.config_parse_value_fc("lat=\"", "\"", valuesParsingConfig);//lattitude
//            //oHammerParser.config_parse_value_fc("lon=\"", "\"", valuesParsingConfig);//longitude

//            //second snippets loop parsing setup
//            oHammerParser.config_parse_snippets_fc("trkpt ", "trkpt>", snippetsParsingConfig);
//            oHammerParser.config_parse_value_fc("lat=\"", "\"", valuesParsingConfig);//lattitude
//            oHammerParser.config_parse_value_fc("lon=\"", "\"", valuesParsingConfig);//longitude

//            //parsing process
//            List<List<string>> LatLonListList = oHammerParser.process_fc(nsFileSystemSettings.GetDataFromFile(gpxFilePath));
//            return LatLonListList;
//        }
//    }
//}
