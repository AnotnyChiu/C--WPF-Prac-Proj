using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendDataService _dataService;
        private IEventAggregator _eventAggregator;

        public FriendDetailViewModel(
            IFriendDataService friendDataService,
            IEventAggregator eventAggregator
            )
        {
            _dataService = friendDataService;
            _eventAggregator = eventAggregator;

            // 在constructor中subcribe event，然後建一個method來handle event
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                            .Subscribe(OnOpenFriendDetailView);
                            // 注意這邊subscribe event的寫法，不需要()跟傳入參數
                            // 只需要告訴他是哪個function即可
        }

        private async void OnOpenFriendDetailView(int friendId)
        {
            await LoadAsync(friendId);
        }

        public async Task LoadAsync(int friendId)
        {
            Friend = await _dataService.GetByIdAsync(friendId);
        }

        // setup friend property
        private Friend _friend;

        public Friend Friend
        {
            get { return _friend; }
            private set
            {
                _friend = value;
                OnPropertyChanged();
                // this method inherit from ViewModelBase class
                // 任何會有變化的地方，都會需要這樣設定property
            }
        }
    }
}
