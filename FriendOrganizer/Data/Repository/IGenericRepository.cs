using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repository
{
    // also make the repository flexible for all entity
    public interface IGenericRepository<T> 
    {
        Task<T> GetByIdAsync(int id);
        Task SaveAsync();
        bool HasChanges();
        void Add(T model);
        void Remove(T model);
    }
}