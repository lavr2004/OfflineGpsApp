using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.App.Adapters.MapsuiServiceAdapter
{
    public interface IMapsuiServiceAdapter: IEveryMapUsedInApplication_DIP_interface
    {
        /// <summary>
        /// Method that initializes map
        /// </summary>
        public void InitializeMap();

        /// <summary>
        /// Method that refreshes map
        /// </summary>
        public void RefreshMap();
    }
}
