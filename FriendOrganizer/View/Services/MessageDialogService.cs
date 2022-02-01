using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.UI.View.Services
{
    // 不要直接把message box 寫在viewModel裡面
    // 所以另外拉出來讓它獨立，再用DI把它灌到viewModel裡面
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }
    }

    // custom dialog results
    public enum MessageDialogResult 
    {
        OK,
        Cancel
    }
}
