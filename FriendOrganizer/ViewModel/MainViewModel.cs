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
using System.Windows;
using FriendOrganizer.UI.View.Services;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        // QQ 拜託property的話讓他public 不然view之間溝通會出問題QQ
        // 一個private變public的問題耗了我3個小時阿QQ
        public INavigationViewModel NavigationViewModel { get; }
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator { get; set; }

        private IMessageDialogService _messageDialogService;
        private IFriendDetailViewModel _friendDetailViewModel;
        public IFriendDetailViewModel FriendDetailViewModel
        {
            get { return _friendDetailViewModel; }
            private set 
            {
                _friendDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(
            INavigationViewModel navigationViewModel,
            Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService
            )
        {
            _friendDetailViewModelCreator = friendDetailViewModelCreator;

            _messageDialogService = messageDialogService;

            // 在constructor中subcribe event，然後建一個method來handle event
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                            .Subscribe(OnOpenFriendDetailView);
            // 注意這邊subscribe event的寫法，不需要()跟傳入參數
            // 只需要告訴他是哪個function即可

            NavigationViewModel = navigationViewModel;
        }

        public async Task LoadAsync() 
        {
           // 不用return東西 而是call LoadAsync function去抓資料
           await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenFriendDetailView(int friendId)
        {
            // 每次點擊一個新的朋友，都會從新開啟一個repository，所以注意在切換之前
            // 要先提醒使用者舊的部分如果有做修改的話會不見
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges) 
            {
                // 注意 Messagebox不要直接用在viewModel裡面，不然會打斷unitest
                var result = _messageDialogService.ShowOkCancelDialog(
                    "You've made chnages. Navigate away may lose the data. Sure to leave?",
                    "Question");

                // 注意這邊他的result是enum 不用property方式取
                // 注意2: 可以用js寫法了 不用開block
                if (result == MessageDialogResult.Cancel) return;
            }

            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(friendId);
        }
    }
}

/* 
 Old version: before decoupling the ui

public class MainViewModel : ViewModelBase
    {
        // ObservableCollection<T> >> from System.Collections.ObjectModel
        // this is the collection that notifies data binding when the data is changed
        // is important when add or remove the data items
        public ObservableCollection<Friend> Friends { get; set; }
        private Friend _selectedFriend;
        private readonly IFriendRepository _friendDataService;

        public MainViewModel(IFriendRepository friendDataService)
        {
            Friends = new ObservableCollection<Friend>();
            _friendDataService = friendDataService;
        }

        // note: "propfull" can create a private field also the public property as well
        public Friend SelectedFriend
        {
            // notice when the selected friend is set
            // we have to notify the data biding that it is changed
            // >> raise property chaged event in the setup
            // first have to implement INotifyPropertyChanged interface in the main view model
            get { return _selectedFriend; }
            set
            { 
                _selectedFriend = value;
                // when value set, also call the property changed event

                // OnPropertyChanged("Selected Friend"); >> 作法一: 直接宣告 但很爛
                // 作法二 : OnPropertyChanged(nameof(SelectedFriend));
                // 作法三 : 不在這邊傳，而是在method那邊去偵測誰call他並且自動抓property name (參見下方參數設定)
                // 這樣在runtime時會去抓call這個method的人
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync() 
        {
            var friends = await _friendDataService.GetAllAsync();

            // since the load method can be called multiple times
            // remember to clear the old data
            Friends.Clear();

            foreach (var f in friends)
            {
                // add to observable collection
                Friends.Add(f);
            }
        }
    }
 
 
 */