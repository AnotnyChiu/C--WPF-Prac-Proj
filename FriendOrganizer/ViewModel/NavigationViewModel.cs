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
        private readonly IEventAggregator _eventAggregator;

        // entity
        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        // send event when a friend is clicked
        private NavigationItemViewModel _selectedFriend;
        public NavigationItemViewModel SelectedFriend
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
            ILookupDataService friendLookupDataService,
            IEventAggregator eventAggregator
            // event sender view model need this event aggregator
            )
        {
            _friendLookupService = friendLookupDataService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();

            // subscribe save event
            // * 雙向互動，當select一個friend時: navigation publish, detail subscribe
            // 當在detail 按下save時，detail publish, navigation subscribe
            _eventAggregator.GetEvent<AfterFriendSvaeEvent>().Subscribe(AfterFriendSaved);
        }

        private void AfterFriendSaved(AfterFriendSvaeEventArgs obj)
        {
            // Single >> return the only entity match the rule
            // and get into exception if not only one matched
            var lookupItem = Friends.Single(l => l.Id == obj.Id);
            lookupItem.DisplayMember = obj.DisplayMember;

            // 到這邊還差一步，因為DisplayMember這個property還沒implement INotifyChage 的interface
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupService.GetFriendLookupAsync();

            // clean local data
            Friends.Clear();

            foreach (var item in lookup)
            {
                Friends.Add(
                    new NavigationItemViewModel(item.Id, item.DisplayMember)
                    );
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