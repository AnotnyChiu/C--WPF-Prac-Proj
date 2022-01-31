using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.EntityExtend
{
    /// <note>
    ///  原來在class上面打三條///的話，會自動幫你跳出summary的block !!
    ///  
    /// 這邊這個是原本LookupItem entity的響應是版本
    /// 在save之後資料更新時，因為這邊有OnPropertyChanged()
    /// 所以畫面才能夠即時響應
    /// </note>
    public class NavigationItemViewModel : ViewModelBase
    {
        public NavigationItemViewModel(int id, string displayMember)
        {
            Id = id;
            DisplayMember = displayMember;
        }

        public int Id { get; }

        private string _displayMember;
        public string DisplayMember 
        { 
            get { return _displayMember; }
            set 
            {
                _displayMember = value;
                OnPropertyChanged();
            }    
        }
    }
}
