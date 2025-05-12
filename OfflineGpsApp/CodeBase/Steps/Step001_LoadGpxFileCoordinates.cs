//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OfflineGpsApp.CodeBase.Steps
//{
//    /// <summary>
//    /// Class for loading GPX file coordinates as MapsuiServicePointModel objects
//    /// </summary>
//    public class Step001_LoadGpxFileCoordinates
//    {
//        public async Task<List<OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServicePointModel>> process_fc()
//        {
//            //reading file data from GPX file
//            List<OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServicePointModel> oMapsuiServicePointModelList= new();
//            string ? gpxContent = await LoadGpxFileContent();
//            if (string.IsNullOrEmpty(gpxContent))
//            {
//                return oMapsuiServicePointModelList;
//            }

//            //parsing data from GPX file into collection of MapsuiServicePointModel objects
//            List<List<string>> AllLatLonListList = ParseGpxContent(gpxContent);
//            if (AllLatLonListList != null && AllLatLonListList.Count > 0)
//            {
//                foreach (List<string> oLatLonList in AllLatLonListList)
//                {
//                    if (oLatLonList != null && oLatLonList.Count == 2)
//                    {
//                        double latitude = double.Parse(oLatLonList[0]);
//                        double longitude = double.Parse(oLatLonList[1]);
//                        OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServicePointModel oMapsuiServicePointModel = new OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServicePointModel(latitude, longitude);
//                        oMapsuiServicePointModelList.Add(oMapsuiServicePointModel);
//                    }
//                }
//            }
//            return oMapsuiServicePointModelList;
//        }

//        private async Task<string?> LoadGpxFileContent()
//        {
//            //read GPX file content from Maui asset
//            string? gpxContent = await OfflineGpsApp.CodeBase.App.Settings.FileSystemSettings.GetReadStringFromMauiAsset("20250421_HIKING_WARSZAWA-KAMPINOS.gpx");
//            return gpxContent;
//        }

//        private List<List<string>> ParseGpxContent(string gpxContent)
//        {
//            //parsing data from GPX file into collection of MapsuiServicePointModel objects
//            OfflineGpsApp.CodeBase.Services.GpxParserService.GpxParserService oGpxParserService = new();
//            System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.process_parse_trackpoints_from_gpx(gpxContent);
//            if (AllLatLonListList == null || AllLatLonListList.Count == 0)
//            {
//                return new List<List<string>>();
//            }
//            return AllLatLonListList;
//        }
//    }
//}
