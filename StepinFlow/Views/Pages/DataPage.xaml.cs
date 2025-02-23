using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages
{
    public partial class DataPage : INavigableView<DataVM>
    {
        public DataVM ViewModel { get; }

        public DataPage(DataVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
