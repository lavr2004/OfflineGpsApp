using HammerParserLibrary;

namespace OfflineGpsApp.CodeBase.Service.GpxParserService
{
    public class GpxParserService
    {
        public System.Collections.Generic.List<List<string>> AllLatLonListList;
        HammerParser oHammerParser = new HammerParser();
        public GpxParserService(string input_str)
        {
            // Constructor logic if required
            if (string.IsNullOrWhiteSpace(input_str))
            {
                throw new ArgumentException("ER - GPX content cannot be empty");
            }
            process_fc(input_str);
        }

        private void process_fc(string input_str)
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

        }
    }
}
