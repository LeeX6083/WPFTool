using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;
using WpfTool.LogViewerModule.Views.Pages;

namespace WpfTool.MainApp.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "WPF UI -  工具";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems =
        [
            new NavigationViewItem()
            {
                Content = "日志",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ContentView20 },
                TargetPageType = typeof(LogViewerPage)
            },
            new NavigationViewItem()
            {
                 Content = "首页",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Cloud16 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            }
        ];

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems =
        [
            //new NavigationViewItem()
            //{
            //    Content = "Settings",
            //    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            //    TargetPageType = typeof(Views.Pages.SettingsPage)
            //}
        ];

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems =
        [
            new MenuItem { Header = "Home", Tag = "tray_home" }
        ];
    }
}
