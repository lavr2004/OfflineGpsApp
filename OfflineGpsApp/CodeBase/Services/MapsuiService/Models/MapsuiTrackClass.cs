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
    public class MapsuiTrackClass
    {
        List<MapsuiPointClass> oMapsuiPointClassList;

        public List<MapsuiPointClass> OMapsuiPointClassList
        {
            get { return oMapsuiPointClassList; }
        }

        public MapsuiTrackClass(List<MapsuiPointClass> oMapsuiPointClassList = null)
        {
            if (oMapsuiPointClassList == null)
            {
                oMapsuiPointClassList = new List<MapsuiPointClass>();
            }
            else
            {
                this.oMapsuiPointClassList = oMapsuiPointClassList;
            }
        }

        public void AddPointToTrack(MapsuiPointClass oMapsuiPointClass)
        {
            oMapsuiPointClassList.Add(oMapsuiPointClass);
        }

    }
}
