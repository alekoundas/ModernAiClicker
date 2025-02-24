using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace StepinFlow.ViewModels.Windows
{
    public partial class MainWindowVM : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "StepinFlow";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Data",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },
            new NavigationViewItem()
            {
                Content = "Sub-Flows",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Flow16},
                TargetPageType = typeof(Views.Pages.SubFlowsPage)
            },
            new NavigationViewItem()
            {
                Content = "Flows",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Flow16},
                TargetPageType = typeof(Views.Pages.FlowsPage)
            },
            new NavigationViewItem()
            {
                Content = "Execute",
                Icon = new SymbolIcon { Symbol = SymbolRegular.PersonRunning20},
                TargetPageType = typeof(Views.Pages.ExecutionPage)
            },
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
