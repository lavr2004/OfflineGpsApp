using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Settings
{
    public static class MapsuiFileSystemSettings
    {
        public static string GetDataFromFile(string filePath)
        {
            string data = string.Empty;

            try
            {
                using (var reader = new System.IO.StreamReader(filePath))
                {
                    data = reader.ReadToEnd();
                }
                System.Diagnostics.Debug.WriteLine($"OK - successfully read content from file ({data.Length} length): {filePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ER - Error reading file: {ex.Message}");
            }

            return data;
        }

        /// <summary>
        /// Gets the path to the local folder for storing files depending on the platform.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalFolderPath()
        {
            var oDevicePlatform = Microsoft.Maui.Devices.DeviceInfo.Platform;

            if (oDevicePlatform == Microsoft.Maui.Devices.DevicePlatform.WinUI)
            {
                //C:\Users\{^USERNAME^}\Documents
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                return FileSystem.AppDataDirectory;
            }
        }

        public static string GetLocalFilePath(string fileName)
        {
            return Path.Combine(GetLocalFolderPath(), fileName);
        }
    }
}
