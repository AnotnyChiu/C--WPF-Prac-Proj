using FriendOrganizer.Model;
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepository;
        private IMessageDialogService _messageDialogService;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
        
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

        // selected number
        private FriendPhoneNumberWrapper _selectedPhoneNumber;
        public FriendPhoneNumberWrapper SelectedPhoneNumber 
        {
            get => _selectedPhoneNumber;
            set 
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                // set remove event executable after value set
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }
        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        // ctor
        public FriendDetailViewModel(
            IFriendRepository friendDataService,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService
            ): base(eventAggregator)
        {
            _friendRepository = friendDataService;
            _messageDialogService = messageDialogService;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            // using prism's delegate method
            // it takes an execute method and a boolean to indecate canExecuteMethod
            //// >> now we put it in DetailViewModelBaseClass
            //SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            //DeleteCommand = new DelegateCommand(OnDeleteExecute);

            // add and remove phone command
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = 
                new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

            // get programming languages
            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            // get phone numbers
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }

        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }
        private void OnRemovePhoneNumberExecute()
        {
            // remove event handler for selected phone number
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            // remove from entity
            _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            // remove from UI
            PhoneNumbers.Remove(SelectedPhoneNumber);
            // reset selected phone number
            SelectedPhoneNumber = null;
            // notify changes
            HasChanges = _friendRepository.HasChanges();
            // let save button executable
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            // for UI to see new number
            PhoneNumbers.Add(newNumber);
            // for entity to save into db
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            // trigger validation (little trick)
            newNumber.Number = "";
        }

        public override async Task LoadAsync(int? friendId)
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

            // initialize phone numbers
            InitializeFriendPhoneNumbers(friendEntity.PhoneNumbers);

            // load programming languages
            await LoadProgrammmingLanguagesLookupAsync();
        }

        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers)
        {
            // 看不懂...
            foreach (var wrapper in PhoneNumbers)
            {
                // cleanup logic if already have phonenumberwrapper in phnoe numbers property 
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged; 
            }

            PhoneNumbers.Clear();
            foreach (var friendPhoneNumber in phoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors)) 
            {
                // if have error in phone number, user shouldn't able to save friend
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
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

        protected override bool OnSaveCanExecute()
        {
            // check input friend is valid，如果不行會自動被disable
            // make sure this method is called > this will be called
            // when we raise "CanExecuteChangeEvent" >> go to LoadAsync method
            return
                Friend != null
                && !Friend.HasErrors
                && HasChanges
                && PhoneNumbers.All(pn => !pn.HasErrors); // check all phone numbers should be valid
        }

        protected override async void OnSaveExecute()
        {
            // save the input friend into db
            // pass the property into function!
            await _friendRepository.SaveAsync();

            // update hasChange property from db context
            HasChanges = _friendRepository.HasChanges();

            RaiseDetailSavedEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");

            // publish event to refresh the navigation part >> then go to navigationViewModel to subcribe it
            //_eventAggregator.GetEvent<AfterDetailSavedEvent>()
            //    .Publish(new AfterDetailSavedEventArgs
            //    {
            //        Id = Friend.Id,
            //        DisplayMember = $"{Friend.FirstName} {Friend.LastName}",
            //        ViewModelName = nameof(FriendDetailViewModel)
            //    });
            // >>> use base class's method
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        protected override async void OnDeleteExecute()
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

            RaiseDetailDeletedEvent(Friend.Id);

            // tell navigation view the friend is deleted and hide friend detail view
            //_eventAggregator.GetEvent<AfterDetailDeletedEvent>()
            //    .Publish(
            //        new AfterDetailDeletedEventArgs
            //        {
            //            Id = Friend.Id,
            //            ViewModelName = nameof(FriendDetailViewModel)
            //        }
            //    );
            // >>> also use the base class's method
        }

    }
}
