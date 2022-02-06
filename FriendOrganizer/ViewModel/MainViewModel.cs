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
using Prism.Commands;
using Autofac.Features.Indexed;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        // QQ 拜託property的話讓他public 不然view之間溝通會出問題QQ
        // 一個private變public的問題耗了我3個小時阿QQ
        public INavigationViewModel NavigationViewModel { get; }
        public ICommand CreateNewDetailCommand { get; }

        // for open programming kanguage tab
        public ICommand OpenSingleDetailViewCommand { get; }

        private IIndex<string, IDetailViewModel> _detailViewModelCreator;

        //private Func<IFriendDetailViewModel> _friendDetailViewModelCreator { get; set; }
        //private Func<IMeetingDetailViewModel> _meetingDetailViewModelCreator { get; set; }
        // 不用了

        private IMessageDialogService _messageDialogService;
        private IDetailViewModel _selectedDetailViewModel;

        // for tab UI, we need multiple view models
        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel 
        {
            get { return _selectedDetailViewModel; }
            set 
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(
            INavigationViewModel navigationViewModel,
            //Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            //Func<IMeetingDetailViewModel> meetingDetailViewModelCreator, 
            // >> 這樣會造成這邊ctor被汙染以及有太多field >> 使用autofac IIndex 功能dynamically生成
            IIndex<string, IDetailViewModel> detailViewModelCreator, // 用string(viewmodel名)作為key找到某一類的IDettailViewModel
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService
            )
        {
            //_friendDetailViewModelCreator = friendDetailViewModelCreator;
            //_meetingDetailViewModelCreator = meetingDetailViewModelCreator;
            // 不用了
            _detailViewModelCreator = detailViewModelCreator;

            _messageDialogService = messageDialogService;

            // 在constructor中subcribe event，然後建一個method來handle event
            _eventAggregator = eventAggregator;

            // also have to make the event argument nullable
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                            .Subscribe(OnOpenDetailView);
            // 注意這邊subscribe event的寫法，不需要()跟傳入參數
            // 只需要告訴他是哪個function即可

            // hide detail view after friend deleted >> subscribe to delete event
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                            .Subscribe(AfterDetailDeleted);

            // subscribe to tab closing event
            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                            .Subscribe(AfterDetailClosed);

            NavigationViewModel = navigationViewModel;

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailViewExecute);

            // for tabs
            DetailViewModels = new ObservableCollection<IDetailViewModel>();
        }

        public async Task LoadAsync() 
        {
           // 不用return東西 而是call LoadAsync function去抓資料
           await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args) // add a ? to make it nullable
        {
            // tabs UI : get the current opened(clicked) view model
            var detailViewModel = DetailViewModels.SingleOrDefault(
                vm => vm.Id == args.Id
                && vm.GetType().Name == args.ViewModelName
                );

            // if clicked thing doesn't exist yet, create a DetailViewModel for it
            if (detailViewModel == null) 
            {
                // get type of view model
                // IIndex: 類似字典去找view model
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                
                // load data
                await detailViewModel.LoadAsync(args.Id);
                // refresh UI by adding it to collection
                DetailViewModels.Add(detailViewModel);
            }

                SelectedDetailViewModel = detailViewModel;


            // 每次點擊一個新的朋友，都會從新開啟一個repository，所以注意在切換之前
            // 要先提醒使用者舊的部分如果有做修改的話會不見
            //if (SelectedDetailViewModel != null && SelectedDetailViewModel.HasChanges) 
            //{
            //    // 注意 Messagebox不要直接用在viewModel裡面，不然會打斷unitest
            //    var result = _messageDialogService.ShowOkCancelDialog(
            //        "You've made chnages. Navigate away may lose the data. Sure to leave?",
            //        "Question");

            //    // 注意這邊他的result是enum 不用property方式取
            //    // 注意2: 可以用js寫法了 不用開block
            //    if (result == MessageDialogResult.Cancel) return;
            //}

            // check which detail view model to create
            //switch (args.ViewModelName)
            //{
            //    case nameof(FriendDetailViewModel):
            //        SelectedDetailViewModel = _friendDetailViewModelCreator();
            //        break;
            //    case nameof(MeetingDetailViewModel):
            //        SelectedDetailViewModel = _meetingDetailViewModelCreator();
            //        break;
            //    default:
            //        throw new Exception($"view model: {args.ViewModelName} not mapped!");
            //} >> 不要這樣生成
        }

        private Dictionary<string, int> _nextNewItemIdDict = new Dictionary<string, int>();
           
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            //for counting order
            if (_nextNewItemIdDict.ContainsKey(viewModelType.Name))
            {
                _nextNewItemIdDict[viewModelType.Name]--;
            }
            else
            {
                _nextNewItemIdDict.Add(viewModelType.Name, 0);
            }

            // just open a new view with empty friend
            OnOpenDetailView(
                new OpenDetailViewEventArgs
                {
                    Id = _nextNewItemIdDict[viewModelType.Name],
                    ViewModelName = viewModelType.Name
                    // make it flexible so it can open different viewModel
                });
        }

        private void OnOpenSingleDetailViewExecute(Type viewModelType)
        {
            // programming language only has a single tab
            OnOpenDetailView(
                new OpenDetailViewEventArgs
                {
                    Id = -1, // will always has single tab
                    ViewModelName = viewModelType.Name
                    // make it flexible so it can open different viewModel
                });
        }


        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args)
        {
            // same logic with delete detail
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            // tabs UI : get the current opened(clicked) view model
            var detailViewModel = DetailViewModels.SingleOrDefault(
                vm => vm.Id == id
                && vm.GetType().Name == viewModelName
                );

            // remove it from the collection
            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }
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