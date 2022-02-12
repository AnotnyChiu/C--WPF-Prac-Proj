using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
        // using mahapp message dialog, first grap our MainWindow (now is metro window)
        private MetroWindow _metroWindow  => (MetroWindow)App.Current.MainWindow; 
        public async Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title)
        {
            // use show message async method
            var result = await _metroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegative);

            // (old one using default wpf message dialog)
            // var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            //return result == MessageBoxResult.OK
            //    ? MessageDialogResult.OK
            //    : MessageDialogResult.Cancel;

            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }

        // void method still naming the return type to Task
        public async Task ShowInfoDialogAsync(string text) 
        {
            await _metroWindow.ShowMessageAsync("Info", text);

            // (old one using default wpf message dialog)
            //MessageBox.Show(text, "Info");
        }
    }

    // custom dialog results
    public enum MessageDialogResult 
    {
        OK,
        Cancel
    }
}
