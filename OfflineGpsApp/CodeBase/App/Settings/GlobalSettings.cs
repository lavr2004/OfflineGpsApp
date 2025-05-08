//using OfflineGpsApp.CodeBase.MVVM.Model;
//using OfflineGpsApp.CodeBase.Services.RepositoryService.RepositoryConcreteService;


//using OfflineGpsApp.CodeBase.MVVM.Model;
//using OfflineGpsApp.CodeBase.Services.RepositoryService.RepositoryConcreteService;

using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

namespace OfflineGpsApp.CodeBase.App.Settings;

public static class GlobalSettings
{
    /// <summary>
    /// Gets the path to the local folder for storing files depending on the platform.
    /// </summary>
    /// <returns></returns>
    public static string GetLocalFolderPath()
    {
        var oDevicePlatform = DeviceInfo.Platform;

        if (oDevicePlatform == DevicePlatform.WinUI)
        {
            //C:\Users\{^USERNAME^}\Documents
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

    /// <summary>
    /// Method to check if the application is launched for the first time.
    /// </summary>
    /// <returns></returns>
    public static bool IsFirstLaunch()
    {
        bool isFirstLaunch = Preferences.Get("IsFirstLaunch", true);
        if (isFirstLaunch)
        {
            Preferences.Set("IsFirstLaunch", false);
        }
        return isFirstLaunch;
    }

    //public static IRepositoryConcreteService<TaskModel> GetConcreteRepositoryService()
    //{
    //    string databasepath = ToDoListApp.Code.Settings.GlobalSettings.GetLocalFilePath("ToDoListApp.sqlite");
    //    // Выберите нужный репозиторий
    //    if (/* Использовать SQLite */ true)
    //        return new RepositoryConcreteServiceSqlite<TaskModel>(databasepath, ToDoListApp.Code.Settings.GlobalSettings.GetInitialDataModelsList());//variant-1
    //                                                                                                                                                 //return RepositoryConcreteServiceSqlite<TaskModel>.FabricSelfAsync(databasepath, ToDoListApp.Code.Settings.GlobalSettings.GetInitialDataModelsList()).GetAwaiter().GetResult();//variant-2
    //    else
    //        return new RepositoryConcreteServiceRestapi<TaskModel>(new HttpClient(), "https://api.example.com/items");
    //}
}