using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models
{
    /// <summary>
    /// Class that represents a track on the map
    /// </summary>
    public class MapsuiServiceTrackModel
    {
        List<MapsuiServicePointModel> oMapsuiPointClassList;

        public List<MapsuiServicePointModel> OMapsuiPointClassList
        {
            get { return oMapsuiPointClassList; }
        }

        public MapsuiServiceTrackModel(List<MapsuiServicePointModel> oMapsuiPointClassList = null)
        {
            if (oMapsuiPointClassList == null)
            {
                oMapsuiPointClassList = new List<MapsuiServicePointModel>();
            }
            else
            {
                this.oMapsuiPointClassList = oMapsuiPointClassList;
            }
        }

        public void AddPointToTrack(MapsuiServicePointModel oMapsuiPointClass)
        {
            oMapsuiPointClassList.Add(oMapsuiPointClass);
        }

    }
}
