using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repository;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.ViewModel.EntityExtend;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMessageDialogService _messageDialogService;
        private IMeetingRepository _meetingRepository;

        // take eventAggregator(from base class), dialog service, repository
        public MeetingDetailViewModel
            (
                IEventAggregator eventAggregator,
                IMessageDialogService messageDialogService,
                IMeetingRepository meetingRepository
            ): base(eventAggregator)
        {
            _messageDialogService = messageDialogService;
            _meetingRepository = meetingRepository;
        }

        // take in model as model wrapper
        private MeetingWrapper _meeting;
        public MeetingWrapper Meeting 
        {
            get => _meeting;
            private set 
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }

        // 1. load meeting or create new one
        public async override Task LoadAsync(int? meetingId)
        {
            var meeting = meetingId.HasValue
                ? await _meetingRepository.GetByIdAsync(meetingId.Value) // 要使用 .Value 去除可能null
                : CreateNewMeeting();

            // build model wrapper and register event
            // do the UI work
            InitializeMeeting(meeting);
        }

        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
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
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            // validation when new meeting created
            if (Meeting.Id == 0) 
            {
                // little trick to trigger validation
                Meeting.Title = "";
            }
        }

        // delete event
        protected async override void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog(
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
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
        }
    }
}
