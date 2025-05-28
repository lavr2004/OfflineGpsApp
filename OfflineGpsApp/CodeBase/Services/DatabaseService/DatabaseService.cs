using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using SQLite;

using nsDatabaseService = OfflineGpsApp.CodeBase.Services.DatabaseService;

//ADDED: for working with SQLite database and handling resource file access in .NET MAUI
namespace OfflineGpsApp.CodeBase.Services.DatabaseService
{
    public class DatabaseService
    {
        private readonly SQLite.SQLiteConnection _connection;
        private const string DatabaseResourceName = "NID_mazowieckie.sqlite"; //ADDED: resource name for database file
        private const string DatabaseFileName = "NID_mazowieckie.sqlite"; //ADDED: local file name in AppDataDirectory

        public DatabaseService()
        {
            string dbPath = InitializeDatabaseAsync().GetAwaiter().GetResult(); //ADDED: initialize database synchronously for simplicity
            _connection = new SQLite.SQLiteConnection(dbPath); //CHANGED: initialize SQLite connection with local path
            _connection.CreateTable<nsDatabaseService.Models.DatabaseServiceMonumentModel>(); //ADDED: ensure table exists
            LogTableInfo(); //ADDED: log table information for debugging
        }

        //ADDED: to copy database from Resources\Raw to AppDataDirectory
        private async Task<string> InitializeDatabaseAsync()
        {
            string localDbPath = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, DatabaseFileName); //ADDED: path in AppDataDirectory
            if (!System.IO.File.Exists(localDbPath)) //ADDED: check if database already copied
            {
                using var stream = await Microsoft.Maui.Storage.FileSystem.OpenAppPackageFileAsync(DatabaseResourceName); //ADDED: open resource stream
                if (stream == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error: Failed to open database resource stream"); //ADDED: log resource failure
                    throw new System.InvalidOperationException("Cannot open database resource");
                }
                using var fileStream = System.IO.File.Create(localDbPath); //ADDED: create file in AppDataDirectory
                await stream.CopyToAsync(fileStream); //ADDED: copy resource to local file
                System.Diagnostics.Debug.WriteLine($"Database copied to: {localDbPath}"); //ADDED: log copy operation
            }
            return localDbPath;
        }

        //ADDED: for debugging table structure and contents
        private void LogTableInfo()
        {
            try
            {
                // Check available tables
                var tables = _connection.Query<TableInfo>("SELECT name FROM sqlite_master WHERE type='table'");
                System.Diagnostics.Debug.WriteLine("Available tables in database:");
                foreach (var table in tables)
                {
                    System.Diagnostics.Debug.WriteLine($"  Table: {table.Name}"); //ADDED: log table name
                }

                // Check if table 'monuments' exists and get row count
                var tableCount = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='monuments'");
                System.Diagnostics.Debug.WriteLine($"Table 'monuments' exists: {tableCount > 0}"); //ADDED: log table existence
                if (tableCount > 0)
                {
                    var rowCount = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM monuments");
                    System.Diagnostics.Debug.WriteLine($"Rows in 'monuments': {rowCount}"); //ADDED: log row count

                    // Log column names and types
                    var columns = _connection.Query<TableInfo>("PRAGMA table_info(monuments)");
                    System.Diagnostics.Debug.WriteLine("Columns in 'monuments':");
                    foreach (var column in columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {column.Name}: {column.Type}"); //ADDED: log column name and type
                    }

                    // Log sample data (first row)
                    var sampleRow = _connection.Table<nsDatabaseService.Models.DatabaseServiceMonumentModel>().FirstOrDefault();
                    if (sampleRow != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Sample monument: Id={sampleRow.Id}, Name={sampleRow.Name}, Latitude={sampleRow.Latitude}, Longitude={sampleRow.Longitude}"); //ADDED: log sample data
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No sample data found in 'monuments'"); //ADDED: log empty sample
                    }
                }
            }
            catch (SQLite.SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error accessing table info: {ex.Message}"); //ADDED: log any errors
            }
        }

        public System.Collections.Generic.List<nsDatabaseService.Models.DatabaseServiceMonumentModel> GetMonuments()
        {
            var monuments = _connection.Table<nsDatabaseService.Models.DatabaseServiceMonumentModel>().ToList(); //ADDED: retrieve all monuments
            System.Diagnostics.Debug.WriteLine($"Retrieved {monuments.Count} monuments from database"); //ADDED: log number of retrieved monuments
            return monuments;
        }
    }

    //ADDED: helper class for PRAGMA table_info and table listing
    public class TableInfo
    {
        public string Name { get; set; } //CHANGED: simplified for table listing
        public int Cid { get; set; }
        public string Type { get; set; }
        public int NotNull { get; set; }
        public string DefaultValue { get; set; }
        public int PrimaryKey { get; set; }
    }
}