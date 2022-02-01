using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Data.Repository;
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
        private IFriendRepository _friendRepository;
        private IEventAggregator _eventAggregator;
        private bool _hasChanges;

        public bool HasChanges
        {
            get  => _hasChanges;
            set 
            {
                // make sure it really changed
                if (_hasChanges != value) 
                {
                    _hasChanges = value;
                    OnPropertyChanged();

                    // raise save event (RaiseCanExecuteChanged)
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

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
            IFriendRepository friendDataService,
            IEventAggregator eventAggregator
            )
        {
            _friendRepository = friendDataService;
            _eventAggregator = eventAggregator;

            // using prism's delegate method
            // it takes an execute method and a boolean to indecate canExecuteMethod
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(int friendId)
        {
            var friendEntity = await _friendRepository.GetByIdAsync(friendId);

            // convert Friend to Friend Wrapper
            Friend = new FriendWrapper(friendEntity);

            // install event handler for the property changed event
            Friend.PropertyChanged += (s, e) =>
            {
                // create a check so it won't call the HasChanges everytime
                if (!HasChanges) 
                {
                    HasChanges = _friendRepository.HasChanges();
                }
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
            return Friend!=null && !Friend.HasErrors && HasChanges;
        }

        private async void OnSaveExecute()
        {
            // save the input friend into db
            // pass the property into function!
            await _friendRepository.SaveAsync();

            // update hasChange property from db context
            HasChanges = _friendRepository.HasChanges();

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
