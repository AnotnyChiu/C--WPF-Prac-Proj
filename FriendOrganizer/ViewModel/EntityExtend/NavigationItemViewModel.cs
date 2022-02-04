using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private IEventAggregator _eventAggregator;
        public NavigationItemViewModel(
            int id, string displayMember,
            IEventAggregator eventAggregator,
            string detailViewModelName
            )
        {
            Id = id;
            DisplayMember = displayMember;
            _detailViewModelName = detailViewModelName;

            _eventAggregator = eventAggregator;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
        }

        public int Id { get; }

        public ICommand OpenDetailViewCommand { get; set; }

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

        private string _detailViewModelName;

        private void OnOpenDetailViewExecute()
        {
            // 先從event list中抓到要的event後publish
            // publish 同時要傳入argument
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                            .Publish(new OpenDetailViewEventArgs
                            {
                                Id = this.Id,
                                ViewModelName = _detailViewModelName
                            });
        }
    }
}
