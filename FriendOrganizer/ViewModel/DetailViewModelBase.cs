using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
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
    // base class for all detail view model
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
     
        public bool HasChanges
        {
            get => _hasChanges;
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

        private int _id;
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        // title property for show in tab title
        private string _title;
        public string Title
        {
            get => _title;
            protected set 
            { 
                _title = value;
                OnPropertyChanged();
            }
        }


        protected readonly IEventAggregator EventAggregator;

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CloseDetailViewCommand { get; }
        public IMessageDialogService MessageDialogService { get; }

        public DetailViewModelBase(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            MessageDialogService = messageDialogService;
        }

        protected abstract void OnDeleteExecute();
        protected abstract bool OnSaveCanExecute();
        protected abstract void OnSaveExecute();
        public abstract Task LoadAsync(int id);

        // raise event
        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish
                (
                    new AfterDetailDeletedEventArgs
                    {
                        Id = modelId,
                        ViewModelName = this.GetType().Name // get the name from the class inherit it
                    }
                ); 
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember) 
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish
                (
                    new AfterDetailSavedEventArgs
                    {
                        Id = modelId,
                        DisplayMember = displayMember,
                        ViewModelName = this.GetType().Name
                    }
                );
        }

        // raise event for collection save
        protected virtual void RaiseCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(new AfterCollectionSavedEventArgs
                {
                    ViewModelName = this.GetType().Name
                });
        }
        protected async virtual void OnCloseDetailViewExecute()
        {
            // if info changed, ask if want to leave
            if (HasChanges) 
            {
                var result = await MessageDialogService.ShowOkCancelDialogAsync(
                    "You've made changes. Close this item?", "Question");
                if (result == MessageDialogResult.Cancel) return;
            }
            // for subclass to override
            // close the instance
            EventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Publish(
                new AfterDetailClosedEventArgs
                {
                    Id = this.Id,
                    ViewModelName = this.GetType().Name
                });
        }
    }
}
