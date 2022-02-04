using FriendOrganizer.Model;
using System.Collections.Generic;

namespace FriendOrganizer.UI.Data.Repository
{
    public interface IFriendRepository : IGenericRepository<Friend>
    {
        void RemovePhoneNumber(FriendPhoneNumber model);
    }
}