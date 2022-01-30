using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.ViewModel
{
    // ViewModelBase 主要在處理當資料變化時的響應是變化
    // 所以如果不是會有被動資料變動的viewModel 可以不用繼承此ViewModelBase
    public class ViewModelBase : INotifyPropertyChanged // using system.ComponentModel
    {
        // raise this event when any property changed (through OnPropertyChanged event handler)
        public event PropertyChangedEventHandler PropertyChanged;

        // seperate out as a method for all property changed events
        // private void OnPropertyChanged(string propertyName) >> 舊做法
        // using System.Runtime.CompilerServices; >> 在runtime去偵測誰call他，抓他的property name (這邊首先是selectedFriend)
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // * 注意這邊要把它extract出去，要給subclass用且可以override，所以是 procted virtual
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // Invoke 第一個參數傳sender 在這邊就是這個class
        }
    }
}
