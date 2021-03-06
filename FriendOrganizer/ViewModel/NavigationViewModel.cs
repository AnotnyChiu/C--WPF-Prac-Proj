using FriendOrganizer.UI.Data;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.ViewModel.EntityExtend;
using FriendOrganizer.UI.Data.Lookup;

namespace FriendOrganizer.UI.ViewModel
{
    // every viewModel also need an interface to register in boostrapper
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly ILookupDataService _friendLookupService;
        private readonly IMeetingLookupDataService _meetingLookupDataService;
        private readonly IEventAggregator _eventAggregator;

        // entity
        public ObservableCollection<NavigationItemViewModel> Friends { get; }
        public ObservableCollection<NavigationItemViewModel> Meetings { get; }

        public NavigationViewModel(
            ILookupDataService friendLookupDataService,
            IMeetingLookupDataService meetingLookupDataService,
            IEventAggregator eventAggregator
            // event sender view model need this event aggregator
            )
        {
            _friendLookupService = friendLookupDataService;
            _meetingLookupDataService = meetingLookupDataService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();

            // subscribe save event
            // * 雙向互動，當select一個friend時: navigation publish, detail subscribe
            // 當在detail 按下save時，detail publish, navigation subscribe
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);

            // subscribe to also delete event
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupService.GetFriendLookupAsync();

            // clean local data
            Friends.Clear();

            foreach (var item in lookup)
            {
                Friends.Add(
                    new NavigationItemViewModel(
                        item.Id, item.DisplayMember,
                        _eventAggregator,
                        nameof(FriendDetailViewModel))
                    );
            }

            // load meetings
            lookup = await _meetingLookupDataService.GetMeetingLookupAsync();

            // clean local data
            Meetings.Clear();

            foreach (var item in lookup)
            {
                Meetings.Add(
                    new NavigationItemViewModel(
                        item.Id, item.DisplayMember,
                        _eventAggregator,
                        nameof(MeetingDetailViewModel))
                    );
            }
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            // switch for different view models
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailSavedGeneric(Friends, args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailSavedGeneric(Meetings, args);
                    break;
                default:
                    break;
            }
            
        }

        private void AfterDetailSavedGeneric(
            ObservableCollection<NavigationItemViewModel> items,
            AfterDetailSavedEventArgs args)
        {
            // Single >> return the only entity match the rule
            // and get into exception if not only one matched
            var lookupItem = items.SingleOrDefault(l => l.Id == args.Id);

            // if the Id is null, means we're creating a new friend here
            if (lookupItem == null)
            {
                items.Add(
                    new NavigationItemViewModel(args.Id, args.DisplayMember,
                    _eventAggregator,
                    args.ViewModelName // pass the name to generate event args
                    ));
            }
            else lookupItem.DisplayMember = args.DisplayMember; // 一樣跟進es6了
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            // switch for different view models
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeletedGeneric(Friends, args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailDeletedGeneric(Meetings, args);
                    break;
                default:
                    break;
            }
        }

        private void AfterDetailDeletedGeneric(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var singleItem = items.SingleOrDefault(f => f.Id == args.Id);
            // update UI by update Friends' collection
            if (singleItem != null) items.Remove(singleItem);
        }
    }
}


/*
-- ObservableCollection<T> --

In many cases the data that you work with is a collection of objects. 

For example, a common scenario in data binding is to use an ItemsControl such as a ListBox, ListView, or TreeView to display a collection of records.

You can enumerate over any collection that implements the IEnumerable interface.

However, to set up dynamic bindings so that insertions or deletions in the collection update the UI automatically,

the collection must implement the INotifyCollectionChanged interface. This interface exposes the CollectionChanged event,

an event that should be raised whenever the underlying collection changes.

WPF provides the ObservableCollection<T> class, which is a built-in implementation of a data collection that implements the INotifyCollectionChanged interface.
 */