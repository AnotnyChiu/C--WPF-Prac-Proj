using FriendOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repository
{
    // 注意: repository繼承generic repository
    // 他的interface也要繼承 generic interface
    public interface IMeetingRepository : IGenericRepository<Meeting>
    {
        Task<List<Friend>> GetAllFriendsAsync();
        Task ReloadFriendAsync(int friendId);
    }
}