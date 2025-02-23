using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsVM>
    {
        public SettingsVM ViewModel { get; }

        public SettingsPage(SettingsVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
