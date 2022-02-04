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
    public class OpenDetailViewEvent: PubSubEvent<OpenDetailViewEventArgs>
    {
    }

    // 說明，要將Detail view event獨立出來的話，在args的地方不一定是拿int 需要包含更多東西
    // 於是建立這個EventArgs的class
    public class OpenDetailViewEventArgs 
    {
        public int? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
