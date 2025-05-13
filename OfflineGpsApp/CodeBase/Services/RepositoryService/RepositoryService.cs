using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.RepositoryService
{
    public class RepositoryService<T>
    {
        private readonly IRepositoryService<T> oIRepositoryConcreteService;

        public RepositoryService(IRepositoryService<T> oIRepositoryConcreteService)
        {
            //SQLite or RESTAPI concrete repository downcasted to Interface is going to constructor from DI container
            this.oIRepositoryConcreteService = oIRepositoryConcreteService;
        }

        public virtual Task<IEnumerable<T>> GetAllItemsAsync() => oIRepositoryConcreteService.GetAllAsync();
        public Task<T> GetItemAsync(int id) => oIRepositoryConcreteService.GetByIdAsync(id);
        public Task AddItemAsync(T item) => oIRepositoryConcreteService.AddAsync(item);
        public Task UpdateItemAsync(T item) => oIRepositoryConcreteService.UpdateAsync(item);
        public Task DeleteItemAsync(int id) => oIRepositoryConcreteService.DeleteAsync(id);
    }
}
