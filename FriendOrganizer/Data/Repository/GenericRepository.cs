using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repository
{
    // also the db context should be specific to the object
    public class GenericRepository<TEntity,TContext> : IGenericRepository<TEntity>
        // 用 where 關鍵字限制generic type的範圍! 讓我們能取得db context內容
        where TContext : DbContext
        // 同時限制TEntity 必須是一個class
        where TEntity : class
    {
        protected readonly TContext _context;

        protected GenericRepository(TContext context)
        {
            _context = context;
        }
        public void Add(TEntity model)
        {
            _context.Set<TEntity>().Add(model);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public bool HasChanges()
        {
            // track db context's changes
            return _context.ChangeTracker.HasChanges();
        }

        public void Remove(TEntity model)
        {
            _context.Set<TEntity>().Remove(model);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
