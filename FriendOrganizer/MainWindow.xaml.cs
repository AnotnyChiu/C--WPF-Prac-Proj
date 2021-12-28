using FriendOrganizer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FriendOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        // pass main viewModel in
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            // need to call the load method, but don't put it in the constructor
            // put it in the event handler
            Loaded += MainWindow_Loaded; // 打 += ，再加上 alt + enter >> 自動生成event handler
            // 這邊是在Loaded這個handler裡面加上load朋友資料的event
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // sender 跟 args 都不一定要用，只要有人觸發event就會call下面的code
            _viewModel.Load();
        }
    }
}
