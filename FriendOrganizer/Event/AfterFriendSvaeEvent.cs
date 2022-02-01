using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Event
{
    // 做畫面即時更新 refresh the navigation part
    public class AfterFriendSvaeEvent : PubSubEvent<AfterFriendSvaeEventArgs>
    {
    }

    public class AfterFriendSvaeEventArgs
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
