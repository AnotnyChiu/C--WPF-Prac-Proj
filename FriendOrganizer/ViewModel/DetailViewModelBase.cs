using FriendOrganizer.UI.Event;
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

        protected readonly IEventAggregator EventAggregator;

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        protected abstract void OnDeleteExecute();
        protected abstract bool OnSaveCanExecute();
        protected abstract void OnSaveExecute();
        public abstract Task LoadAsync(int? id);

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
    }
}
