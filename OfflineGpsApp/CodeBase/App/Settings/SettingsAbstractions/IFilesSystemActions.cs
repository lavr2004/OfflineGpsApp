using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.App.Settings.SettingsAbstractions
{
    public interface IFilesSystemActions
    {
        Task<Stream?> OpenAppPackageFileAsync(string filePath);
    }
}
