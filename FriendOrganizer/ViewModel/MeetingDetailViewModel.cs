using FriendOrganizer.Model;
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
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        // take eventAggregator(from base class), dialog service, repository
        public MeetingDetailViewModel
            (
                IEventAggregator eventAggregator,
                IMessageDialogService messageDialogService,
                IMeetingRepository meetingRepository
            ) : base(eventAggregator, messageDialogService)
        {
            _meetingRepository = meetingRepository;

            // event for synchornize the data when edit friend's info
            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);

            // event for synchornize the data when a friend's info is deleted
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);


            // fields for doing friend picklist formeeting
            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        

        private IMeetingRepository _meetingRepository;

        public ObservableCollection<Friend> AddedFriends { get; }
        public ObservableCollection<Friend> AvailableFriends { get; }
        public ICommand AddFriendCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        private List<Friend> _allFriends;

        private Friend _selectedAvailableFriend;
        public Friend SelectedAvailableFriend 
        {
            get => _selectedAvailableFriend;
            set 
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        private Friend _selectedAddedFriend;
        public Friend SelectedAddedFriend
        {
            get => _selectedAddedFriend;
            set
            {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        // take in model as model wrapper
        private MeetingWrapper _meeting;
        public MeetingWrapper Meeting 
        {
            get => _meeting;
            set 
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }

        /// ================================= Methods ===========================================


        // 1. load meeting or create new one
        public async override Task LoadAsync(int meetingId)
        {
            var meeting = meetingId > 0
                ? await _meetingRepository.GetByIdAsync(meetingId) // 要使用 .Value 去除可能null
                : CreateNewMeeting(meetingId);

            // assign Id for tab UI
            Id = meetingId;

            // build model wrapper and register event
            // do the UI work
            InitializeMeeting(meeting);

            // load friends for the picklist
            _allFriends = await _meetingRepository.GetAllFriendsAsync();

            SetupPicklist();
        }

        private void SetupPicklist()
        {
            // 從DB抓資料之後分成已在meeting裡面跟未在meeting裡面
            var mtgFriendIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
            var addedFriends = _allFriends
                .Where(f => mtgFriendIds.Contains(f.Id)).OrderBy(f => f.FirstName);
            var avaliableFriends = _allFriends
                .Except(addedFriends).OrderBy(f => f.FirstName);

            // update collection for UI
            AddedFriends.Clear();
            AvailableFriends.Clear();
            foreach (var added in addedFriends)
            {
                AddedFriends.Add(added);
            }
            foreach (var avaliable in avaliableFriends)
            {
                AvailableFriends.Add(avaliable);
            }
        }

        private Meeting CreateNewMeeting(int index)
        {
            // convert index
            string order = $"{(index - 1) * -1 }";
            // set default name to new friend with the incomming id
            var meeting = new Meeting
            {
                Title = $"New Mtg {order}",
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };
            _meetingRepository.Add(meeting);
            return meeting;
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                // reset tab title name
                if (e.PropertyName == nameof(Meeting.Title)) 
                {
                    SetTitle();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            // validation when new meeting created
            //if (Meeting.Id == 0) 
            //{
            //    // little trick to trigger validation
            //    //Meeting.Title = "";
            //}

            // set tab title
            SetTitle();
        }

        private void SetTitle()
        {
            Title = Meeting.Title;
        }

        // delete event
        protected async override void OnDeleteExecute()
        {
            var result = MessageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete this meeting: {Meeting.Title} ?"
                ,"Question");
            if (result == MessageDialogResult.OK) 
            {
                _meetingRepository.Remove(Meeting.Model);
                await _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        // save can execute event
        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        // save execute event
        protected override async void OnSaveExecute() 
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            // refresh Id for tab UI
            Id = Meeting.Id;
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
        }

        public bool OnRemoveFriendCanExecute() => SelectedAddedFriend != null;
        private bool OnAddFriendCanExecute() => SelectedAvailableFriend != null;
       
        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;

            // update db
            Meeting.Model.Friends.Remove(friendToRemove);
            // update UI
            AddedFriends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);

            // update status
            HasChanges = _meetingRepository.HasChanges();
            // let save button executable
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            // update db
            Meeting.Model.Friends.Add(friendToAdd);
            // update UI
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);

            // update status
            HasChanges = _meetingRepository.HasChanges();
            // let save button executable
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        // for data synchornization
        // tips: ctrl + - 可以回到游標的上一個地方!
        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            // refresh the name in the picklist
            if (args.ViewModelName == nameof(FriendDetailViewModel)) 
            {
                // notice since entity framework will caching entity
                // so if the tab is opened and the friend been editted
                // the entity here will still be the old one in cache
                // >> _allFriends = await _meetingRepository.GetAllFriendsAsync();

                // so we have to reload a single friend who's been editted
                await _meetingRepository.ReloadFriendAsync(args.Id);
                SetupPicklist();
            }
        }

        // for data synchornization
        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            // refresh the name in the picklist
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                await _meetingRepository.ReloadFriendAsync(args.Id);
                SetupPicklist();
            }
        }
    }
}
