using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Data
{
    // 可以直接從class extract interface出來
    // alt + enter 中就會有這個選項
    public class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            // Using mock data just for now
            // Later will load data from real database
            yield return new Friend { FirstName = "Thomas", LastName = "Huber" };
            yield return new Friend { FirstName = "Antony", LastName = "Chiu" };
            yield return new Friend { FirstName = "Kenny", LastName = "Hsiao" };
            yield return new Friend { FirstName = "Frank", LastName = "Wang" };
        }
    }
}
