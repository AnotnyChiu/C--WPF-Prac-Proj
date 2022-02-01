﻿using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Data.Lookup;
using FriendOrganizer.UI.Data.Repository;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.ViewModel.EntityExtend;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IMessageDialogService _messageDialogService;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
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
        public ICommand DeleteCommand { get; }
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }

        // ctor
        public FriendDetailViewModel(
            IFriendRepository friendDataService,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService
            )
        {
            _friendRepository = friendDataService;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            // using prism's delegate method
            // it takes an execute method and a boolean to indecate canExecuteMethod
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);

            // get programming languages
            ProgrammingLanguages = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync(int? friendId)
        {
            // check the id pass in is null or not
            // if not load a friend
            // if yes then create a new friend
            // 注意這邊的nullable check，用HasValue去check，然後下面不能塞null的部份去取value的property
            var friendEntity = friendId.HasValue ?
                await _friendRepository.GetByIdAsync(friendId.Value)
                : CreateNewFriend();

            // initialize friend
            InitializeFriend(friendEntity);

            // load programming languages
            await LoadProgrammmingLanguagesLookupAsync();
        }

        private void InitializeFriend(Friend friendEntity)
        {
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

            // little trick to trigger input validation
            if (Friend.Id == 0) Friend.FirstName = "";
        }

        private async Task LoadProgrammmingLanguagesLookupAsync()
        {
            ProgrammingLanguages.Clear();

            // add empty entry item
            ProgrammingLanguages.Add(new NullLookupItem());

            // add special item represent empty entry
            var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
            foreach (var item in lookup)
            {
                ProgrammingLanguages.Add(item);
            }
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

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private async void OnDeleteExecute()
        {
            // show message dialog
            var result = _messageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete friend: {Friend.FirstName} {Friend.LastName} ?",
                "Question"
                );
            if (result == MessageDialogResult.Cancel) return;

            // remove and save
            _friendRepository.Remove(Friend.Model);
            await _friendRepository.SaveAsync();

            // tell navigation view the friend is deleted and hide friend detail view
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
        }

    }
}
