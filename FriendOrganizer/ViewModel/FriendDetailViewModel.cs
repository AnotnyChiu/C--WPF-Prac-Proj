using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.ViewModel.EntityExtend;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendDataService _dataService;
        private IEventAggregator _eventAggregator;
        // setup friend property
        private FriendWrapper _friend;
        public FriendWrapper Friend
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
        public ICommand SaveCommand { get; }

        // ctor
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

            // using prism's delegate method
            // it takes an execute method and a boolean to indecate canExecuteMethod
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private async void OnOpenFriendDetailView(int friendId)
        {
            await LoadAsync(friendId);
        }

        public async Task LoadAsync(int friendId)
        {
            var friendEntity = await _dataService.GetByIdAsync(friendId);

            // convert Friend to Friend Wrapper
            Friend = new FriendWrapper(friendEntity);

            // install event handler for the property changed event
            Friend.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                    // finally make sure the HasErrors property raise >> check NotifyDataErrorInfoBase
                }
            };

            // raise save event (RaiseCanExecuteChanged)
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnSaveCanExecute()
        {
            // check input friend is valid，如果不行會自動被disable
            // make sure this method is called > this will be called
            // when we raise "CanExecuteChangeEvent" >> go to LoadAsync method
            return Friend!=null && !Friend.HasErrors;
        }

        private async void OnSaveExecute()
        {
            // save the input friend into db
            // pass the property into function!
            await _dataService.SaveAsync(Friend.Model); // here need to pass pure entity

            // publish event to refresh the navigation part >> then go to navigationViewModel to subcribe it
            _eventAggregator.GetEvent<AfterFriendSvaeEvent>()
                .Publish(new AfterFriendSvaeEventArgs
                {
                    Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        }
    }
}
