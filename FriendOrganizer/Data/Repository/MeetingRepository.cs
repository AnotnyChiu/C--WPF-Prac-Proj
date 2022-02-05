using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FriendOrganizer.UI.Data.Repository
{
    public class MeetingRepository : GenericRepository<Meeting, FriendOrganizerDbContext>, IMeetingRepository
    {
        public MeetingRepository(FriendOrganizerDbContext context) : base(context)
        {
        }

        public async override Task<Meeting> GetByIdAsync(int id)
        {
            // 要記得using System.Data.Entity;
            // Include 就是join 的概念
            return await _context.Meetings
                .Include(m => m.Friends)
                .SingleAsync(m => m.Id == id);
        }

        // get all friends for the picklist
        public async Task<List<Friend>> GetAllFriendsAsync() 
        {
            return await _context.Set<Friend>().ToListAsync();
        }

        // reload single friend
        public async Task ReloadFriendAsync(int friendId)
        {
            // 從change tracker 中取出我們在找的friend
            var dbEntityEntry = _context.ChangeTracker.Entries<Friend>()
                .SingleOrDefault(db => db.Entity.Id == friendId);

            // 使用change tracker的reload async功能來強制重整這個friend的info
            // 先確認是否存在
            if (dbEntityEntry != null) 
            {
                // reload the entity in this context from the database
                await dbEntityEntry.ReloadAsync();
            }
        }
    }
}
