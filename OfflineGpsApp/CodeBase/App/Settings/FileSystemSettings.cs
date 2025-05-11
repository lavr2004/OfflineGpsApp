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

    }
}
