using FriendOrganizer.Data;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // ObservableCollection<T> >> from System.Collections.ObjectModel
        // this is the collection that notifies data binding when the data is changed
        // is important when add or remove the data items
        public ObservableCollection<Friend> Friends { get; set; }
        private Friend _selectedFriend;
        private readonly IFriendDataService _friendDataService;

        public MainViewModel(IFriendDataService friendDataService)
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

        public void Load() 
        {
            var friends = _friendDataService.GetAll();

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
}
