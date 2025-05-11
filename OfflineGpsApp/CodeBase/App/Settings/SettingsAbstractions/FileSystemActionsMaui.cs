using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.App.Settings.SettingsAbstractions
{
    public class FileSystemActionsMaui: IFilesSystemActions
    {
        /// <summary>
        /// Testable abstraction created for testing purposes.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<Stream?> OpenAppPackageFileAsync(string filePath)
        {
            return await FileSystem.OpenAppPackageFileAsync(filePath);
        }
    }
}
