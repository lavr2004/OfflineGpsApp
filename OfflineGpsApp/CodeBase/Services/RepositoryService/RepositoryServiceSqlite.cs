using System.Diagnostics;
using SQLite;

namespace OfflineGpsApp.CodeBase.Services.RepositoryService;

public class RepositoryConcreteServiceSqlite<T> : IRepositoryService<T> where T : new()
{
    public event EventHandler<T> OnItemAdded;
    public event EventHandler<T> OnItemUpdated;
    public event EventHandler<T> OnItemDeleted;
    public bool IsDatabaseInitialized { get; private set; }

    private SQLiteAsyncConnection _oSqLiteAsyncConnection;

    public RepositoryConcreteServiceSqlite(string databasePath, List<T> initialDataModels)
    {
        //CreateConnection(databasePath, initialDataModels);
        //Task.Run( async () => await CreateConnectionAsync(databasePath, initialDataModels));//пустая страница в результате
        Task.Run(async () => await CreateConnectionAsync(databasePath, initialDataModels)).Wait();//все грузится. Дожидаемся вторичного потока на паузе
        //CreateConnectionAsync(databasePath, initialDataModels).ConfigureAwait(false).GetAwaiter();
        //CreateConnectionAsync(databasePath, initialDataModels).Wait();
        Debug.WriteLine($"OK: database connected bypath: {databasePath}");
    }

    public static async Task<RepositoryConcreteServiceSqlite<T>> FabricSelfAsync(string databasePath, List<T> initialDataModels)
    {
        var self = new RepositoryConcreteServiceSqlite<T>(databasePath, initialDataModels);
        await self.CreateConnectionAsync(databasePath, initialDataModels);
        return self;
    }

    private async Task CreateConnectionAsync(string databasePath, List<T> initialDataModelsList)
    {
        if (_oSqLiteAsyncConnection == null)
        {
            _oSqLiteAsyncConnection = new SQLiteAsyncConnection(databasePath);
            await _oSqLiteAsyncConnection.CreateTableAsync<T>();

            var countRecords = await _oSqLiteAsyncConnection.Table<T>().CountAsync();
            if (countRecords == 0 && initialDataModelsList?.Any() == true)
            {
                await _oSqLiteAsyncConnection.InsertAllAsync(initialDataModelsList);
            }

            IsDatabaseInitialized = true;
        }
    }

    // private async Task CreateConnectionAsync(string databasePath, List<T> initialDataModelsList)
    // {
    //     if (_oSqLiteAsyncConnection == null)
    //     {
    //         _oSqLiteAsyncConnection = new SQLiteAsyncConnection(databasePath);
    //         await _oSqLiteAsyncConnection.CreateTableAsync<T>().ConfigureAwait(false);
    //         var countrecords = await _oSqLiteAsyncConnection.Table<T>().CountAsync().ConfigureAwait(false);
    //         if (countrecords == 0 && initialDataModelsList?.Any() == true)
    //         {
    //             await _oSqLiteAsyncConnection.InsertAllAsync(initialDataModelsList).ConfigureAwait(false);
    //         }
    //         
    //         IsDatabaseInitialized = true;
    //     }
    // }

    // private void CreateConnection(string databasePath, List<T> initialDataModelsList)
    // {
    //     if (_oSqLiteAsyncConnection == null)
    //     {
    //         _oSqLiteAsyncConnection = new SQLiteAsyncConnection(databasePath);
    //         _oSqLiteAsyncConnection.CreateTableAsync<T>().Wait();
    //         var countrecords = _oSqLiteAsyncConnection.Table<T>().CountAsync().GetAwaiter().GetResult();
    //         if (countrecords == 0 && initialDataModelsList?.Any() == true)
    //         { 
    //             _oSqLiteAsyncConnection.InsertAllAsync(initialDataModelsList).Wait();
    //         }
    //         
    //         IsDatabaseInitialized = true;
    //     }
    // }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _oSqLiteAsyncConnection.Table<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _oSqLiteAsyncConnection.FindAsync<T>(id);
    }

    public async Task AddAsync(T entity)
    {
        await _oSqLiteAsyncConnection.InsertAsync(entity);
        OnItemAdded?.Invoke(this, entity);
    }

    public async Task UpdateAsync(T entity)
    {
        await _oSqLiteAsyncConnection.UpdateAsync(entity);
        OnItemUpdated?.Invoke(this, entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await _oSqLiteAsyncConnection.DeleteAsync(entity);
            OnItemDeleted?.Invoke(this, entity);
        }
    }

    /// <summary>
    /// Method added for unit testing purposes - to close SQLite connection
    /// </summary>
    /// <returns></returns>
    public async Task DisposeAsync()
    {
        if (_oSqLiteAsyncConnection != null)
        {
            await _oSqLiteAsyncConnection.CloseAsync();
            _oSqLiteAsyncConnection = null;
        }
    }
}