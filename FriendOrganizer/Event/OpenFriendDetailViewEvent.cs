using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Event
{
    // using prism event aggregator, and inherit PubSubEvent<T>
    // here since the argument(parameter) is friend id, so let it be int
    public class OpenFriendDetailViewEvent: PubSubEvent<int>
    {
    }
}
