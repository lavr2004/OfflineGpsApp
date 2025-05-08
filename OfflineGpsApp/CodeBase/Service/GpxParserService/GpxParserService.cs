using HammerParserLibrary;
using System.Globalization;

namespace OfflineGpsApp.CodeBase.Service.GpxParserService
{
    public class GpxParserService
    {
        public System.Collections.Generic.List<List<string>> AllLatLonListList;
        HammerParser oHammerParser = new HammerParser();
        public GpxParserService()
        {
        }

        public System.Collections.Generic.List<List<string>> process_fc(string input_str)
        {
            HammerParserConfig snippetsParsingConfig = new HammerParserConfig();
            HammerParserConfig valuesParsingConfig = new HammerParserConfig();
            valuesParsingConfig.is_into_one_line_text = true;
            valuesParsingConfig.is_include_start_end_tags = false;

            //first snippets loop parsing setup
            //oHammerParser.config_parse_snippets_fc("wpt ", "wpt>", snippetsParsingConfig);
            //oHammerParser.config_parse_value_fc("lat=\"", "\"", valuesParsingConfig);//lattitude
            //oHammerParser.config_parse_value_fc("lon=\"", "\"", valuesParsingConfig);//longitude

            //second snippets loop parsing setup
            oHammerParser.config_parse_snippets_fc("trkpt ", "trkpt>", snippetsParsingConfig);
            oHammerParser.config_parse_value_fc("lat=\"", "\"", valuesParsingConfig);//lattitude
            oHammerParser.config_parse_value_fc("lon=\"", "\"", valuesParsingConfig);//longitude

            //parsing process
            AllLatLonListList = oHammerParser.process_fc(input_str);
            return AllLatLonListList;
        }

        /// <summary>
        /// Provide safe parsing GPX data from input string to List of tuples (Lattitude, Longitude)
        /// </summary>
        /// <param name="input_str"></param>
        /// <returns>List of tuples with latitude and longitude doubles</returns>
        public List<(System.Double Lattitude, System.Double Longitude)> process_parseTrackPointsGpxToTupleList_fc(string input_str)
        {
            process_fc(input_str);

            List<(System.Double Lattitude, System.Double Longitude)> LanLonTupleList = new List<(System.Double Lattitude, System.Double Longitude)>();
            if (AllLatLonListList.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("ER - No points found in GPX file.");
                return LanLonTupleList;
            }

            string lat, lon;
            foreach (List<string> latlonlist in AllLatLonListList)
            {
                if (latlonlist.Count == 2)
                {
                    lat = latlonlist[0];
                    lon = latlonlist[1];

                    if (string.IsNullOrWhiteSpace(lat) || string.IsNullOrWhiteSpace(lon))
                    {
                        System.Diagnostics.Debug.WriteLine($"ER - Invalid lat/lon pair - values problem - {latlonlist}");
                        continue;
                    }

                    lat = lat.Replace(',', '.').Trim();
                    lon = lon.Replace(',', '.').Trim();

                    if (Double.TryParse(lat, NumberStyles.Any, CultureInfo.InvariantCulture, out System.Double lattitude) && Double.TryParse(lon, NumberStyles.Any, CultureInfo.InvariantCulture, out System.Double longitude))
                    {
                        LanLonTupleList.Add((lattitude, longitude));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ER - Invalid lat/lon pair - count problem - {latlonlist}");
                }
            }

            return LanLonTupleList;
        }
    }
}
