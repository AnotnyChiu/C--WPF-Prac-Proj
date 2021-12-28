using FriendOrganizer.Model;
using System.Collections.Generic;

namespace FriendOrganizer.Data
{
    public interface IFriendDataService
    {
        IEnumerable<Friend> GetAll();
    }
}