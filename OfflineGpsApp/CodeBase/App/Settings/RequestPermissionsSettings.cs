using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.App.Settings
{
    public static class RequestPermissionsSettings
    {
        /// <summary>
        /// Request permission to access the device's storage.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestStoragePermission()
        {
            try
            {
                Microsoft.Maui.ApplicationModel.PermissionStatus permissionstatusEnum = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (permissionstatusEnum != PermissionStatus.Granted)
                {
                    permissionstatusEnum = await Permissions.RequestAsync<Permissions.StorageRead>();
                }
                return permissionstatusEnum == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Console.WriteLine($"Error requesting storage permission: {ex.Message}");
                return false;
            }
        }
    }
}
