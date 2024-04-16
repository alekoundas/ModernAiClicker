using Business.Helpers;
using Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private List<string> _flowsList = SystemProcessHelper.GetProcessWindowTitles();

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }
    }
}
