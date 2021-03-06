using Autofac;
using FriendOrganizer.UI;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Startup;
using FriendOrganizer.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 在這邊建立main window的instance
            // 然後後面用Show()

            // ** 舊做法: 未用dependency injection
            //var mainWindow = 
            //    new MainWindow(
            //        new MainViewModel(
            //            new FriendRepository()
            //        )
            //    );


            // ** 新做法: 使用autofac去handle dependency injection
            var boostrapper = new Boostrapper();
            var container = boostrapper.Boostrap();

            // 用 container 的 extension method "resolve" 抓出
            var mainWindow = container.Resolve<MainWindow>();

            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(
            object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // display error in message box
            string errMessage = $"Unexpected Error Occured. Please Inform The Admin\n{e.Exception.Message}";
            MessageBox.Show(errMessage, "Unexpected Error");

            e.Handled = true; // mark the exception as handled, and let the program continue
        }
    }
}
