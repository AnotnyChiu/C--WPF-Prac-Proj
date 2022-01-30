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

namespace FriendOrganizer.UI.ViewModel
{
    // every viewModel also need an interface to register in boostrapper
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly IFriendLookupDataService _friendLookupService;
        private readonly IEventAggregator _eventAggregator;

        // entity
        public ObservableCollection<LookupItem> Friends { get; }

        // send event when a friend is clicked
        private LookupItem _selectedFriend;
        public LookupItem SelectedFriend
        {
            get { return _selectedFriend; }
            set { 
                _selectedFriend = value;
                OnPropertyChanged();

                // publish event
                if (_selectedFriend != null) 
                {
                    // 先從event list中抓到要的event後publish
                    // publish 同時要傳入argument
                    _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                                    .Publish(_selectedFriend.Id);
                }
            }
        }


        public NavigationViewModel(
            IFriendLookupDataService friendLookupDataService,
            IEventAggregator eventAggregator
            // event sender view model need this event aggregator
            )
        {
            _friendLookupService = friendLookupDataService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupService.GetFriendLookupAsync();

            // clean local data
            Friends.Clear();

            foreach (var item in lookup)
            {
                Friends.Add(item);
            }
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