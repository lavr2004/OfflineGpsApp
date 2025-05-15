using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.App.Adapters.GPSServiceAdapter
{
    public class GpsServiceAdapterEventArgs : EventArgs
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
