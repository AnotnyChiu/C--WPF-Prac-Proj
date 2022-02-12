using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repository;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.ViewModel.EntityExtend;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        private IProgrammingLanguageRepository _programmingLanguageRepository;

        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        private ProgrammingLanguageWrapper _selectedProgrammingLanguage;

        public ProgrammingLanguageWrapper SelectedProgrammingLanguage
        {
            get { return _selectedProgrammingLanguage; }
            set 
            { 
                _selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }


        public ProgrammingLanguageDetailViewModel(
            IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService,
            IProgrammingLanguageRepository programmingLanguageRepository) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";
            _programmingLanguageRepository = programmingLanguageRepository;
            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();

            // command for add and delete language
            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        public async override Task LoadAsync(int id)
        {
            Id = id;

            // clear and reset the events
            foreach (var wrapper in ProgrammingLanguages)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            // clear the cache
            ProgrammingLanguages.Clear();

            // load from db
            var languages = await _programmingLanguageRepository.GetAllAsync();

            // create wrapper and add to collection
            foreach (var model in languages)
            {
                var wrapper = new ProgrammingLanguageWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges) 
            {
                HasChanges = _programmingLanguageRepository.HasChanges();
            }
            // if there are errors, don't allow for save
            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors)) 
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && ProgrammingLanguages.All(p => !p.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            // throw exception if a language is used by a friend
            try
            {
                await _programmingLanguageRepository.SaveAsync();
                HasChanges = _programmingLanguageRepository.HasChanges();

                // raise the event and let friendDetailViewModel to subscribe to it
                RaiseCollectionSavedEvent();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException; // 只秀出最裡層(底層)的exceptioin
                }
                await MessageDialogService.ShowInfoDialogAsync(ex.Message);
                await LoadAsync(Id);
            }
           
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedProgrammingLanguage != null;
        }

        private async void OnRemoveExecute()
        {
            // check if the language is referenced by a friend
            var isReferenced =
                await _programmingLanguageRepository
                .IsReferencedByFriendAsync(SelectedProgrammingLanguage.Id);
            if (isReferenced.IsReferenced) 
            {
                string info = $"The language {SelectedProgrammingLanguage.Name}" +
                              $" can't be removed as it's referenced by at lease one friend." +
                              $"\n\nThe friends who reference {SelectedProgrammingLanguage.Name}:\n";
                foreach (var lan in isReferenced.WhoReference)
                {
                    info += $"\n{lan}";
                }
                await MessageDialogService.ShowInfoDialogAsync(info); 
                return;
            }

            // detach property changed event
            SelectedProgrammingLanguage.PropertyChanged -= Wrapper_PropertyChanged;
            // remove from db
            _programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            // remove from collectioin for UI
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            // reset selected language
            SelectedProgrammingLanguage = null;
            // update has change status
            HasChanges = _programmingLanguageRepository.HasChanges();
            // raise can save event
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            // create a wrapper
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage() 
            {
                Name = "New Language"
            });
            // attach event
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            // add to db
            _programmingLanguageRepository.Add(wrapper.Model);
            // add to collection for UI
            ProgrammingLanguages.Add(wrapper);

            // trigger validation
            // wrapper.Name = "";
        }

    }
}
