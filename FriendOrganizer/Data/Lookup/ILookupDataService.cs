using FriendOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Lookup
{
    public interface ILookupDataService
    {
        Task<IEnumerable<LookupItem>> GetFriendLookupAsync();
    }
}