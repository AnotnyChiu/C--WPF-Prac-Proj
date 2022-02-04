using Prism.Events;

namespace FriendOrganizer.UI.Event
{
    public class AfterDetailDeletedEvent : PubSubEvent<AfterDetailDeletedEventArgs>
    {
        // using prism event
    }

    public class AfterDetailDeletedEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }

    }
}
