using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfflineGpsApp.CodeBase.App.Settings.SettingsAbstractions;

namespace OfflineGpsApp.CodeBase.App.Settings
{
    public static class FileSystemSettings
    {
        public static async Task<string?> GetReadStringFromMauiAsset(string? fileName = null, IFilesSystemActions? fileSystem = null)
        {
            fileSystem ??= new FileSystemActionsMaui();
            fileName ??= "AboutAssets.txt";

            try
            {
                var stream = await fileSystem.OpenAppPackageFileAsync(fileName);
                if (stream == null)
                {
                    return null;
                }

                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {fileName}: {ex.Message}");
                return null;
            }
        }

        ///// <summary>
        ///// Public method to read a string from the Maui Asset file from Resources\Raw folder
        ///// </summary>
        ///// <param name="FileNameFromResourcesRawFolder"></param>
        ///// <returns></returns>
        //public static async Task<string?> GetReadStringFromMauiAsset(string? FileNameFromResourcesRawFolder = null)
        //{
        //    if (string.IsNullOrEmpty(FileNameFromResourcesRawFolder))
        //    {
        //        FileNameFromResourcesRawFolder = "AboutAssets.txt";
        //    }
        //    using var stream = await FileSystem.OpenAppPackageFileAsync(FileNameFromResourcesRawFolder);
        //    if (stream == null)
        //    {
        //        return "";
        //    }

        //    using var reader = new StreamReader(stream);

        //    var contents = reader.ReadToEnd();

        //    return contents;
        //}

        public static async Task<Microsoft.Maui.Storage.FileResult?> PickLocalGpxAndroid(string msg = "Open GPX file")
        {
            try
            {
                //read file
                var settings = new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.Android, new[] { "application/gpx", "application/gpx+xml", "*/*" } } };
                var options = new Microsoft.Maui.Storage.PickOptions
                {
                    PickerTitle = msg,
                    FileTypes = new Microsoft.Maui.Storage.FilePickerFileType(settings)
                };

                Microsoft.Maui.Storage.FileResult? oFileResult = await Microsoft.Maui.Storage.FilePicker.PickAsync(options);

                //validate file extension
                if (oFileResult != null && !oFileResult.FileName.EndsWith(".gpx", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("Selected file is not a GPX file");
                    return null;
                }
                return oFileResult;
            }
            catch (Exception ex)
            {
                // Error may happen if user cancels the action
                System.Diagnostics.Debug.WriteLine($"Error in process of selection file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Request and read GPX file from the file system on Android via FilePicker user dialog
        /// </summary>
        /// <returns>string GPX file content</returns>
        public static async Task<string?> ReadGpxFileContentFromFileSystemAndroid()
        {
            Microsoft.Maui.Storage.FileResult? oFileResult = await PickLocalGpxAndroid();
            if (oFileResult == null)
                return null;

            using System.IO.Stream stream = await oFileResult.OpenReadAsync();
            using System.IO.StreamReader reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

    }

    
}
