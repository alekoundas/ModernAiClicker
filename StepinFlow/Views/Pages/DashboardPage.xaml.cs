using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardVM>
    {
        public DashboardVM ViewModel { get; }

        public DashboardPage(DashboardVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
