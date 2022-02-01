using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repository
{
    // 可以直接從class extract interface出來
    // alt + enter 中就會有這個選項
    // Ctrl R R 可以快速從新命名class
    public class FriendRepository : IFriendRepository
    {
        private FriendOrganizerDbContext _context;

        public FriendRepository(FriendOrganizerDbContext context)
        {
            //!! 注意 1. 傳入的參數可以是call一個回傳generic的function >> Func<T>
            // 2.這邊這樣做DI是因為要讓Autofac去產生 FriendOrganizerDbContext 的instance
            // 3.記得要去boostrapper那邊設定autofac
            _context = context;
        }

        // return single frined's detail info
        public async Task<Friend> GetByIdAsync(int friendId)
        {
            // load from db
            var firends = await _context.Friends.SingleAsync(f => f.Id == friendId);
                
            // as no tracking : 重新從DB取 不是先去快取找
            // 使用System.Data.Entity的ToListAsync，讓method變asyncronous

            // async 測試
            // await Task.Delay(5000);

            return firends;
        }

        public bool HasChanges()
        {
            // track db context's changes
            return _context.ChangeTracker.HasChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Add(Friend friend)
        {
            // call the db set
            _context.Friends.Add(friend);
        }

        public void Remove(Friend friend)
        {
            _context.Friends.Remove(friend);
        }
    }
}
