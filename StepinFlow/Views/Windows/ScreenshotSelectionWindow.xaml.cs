using System.Windows;
using StepinFlow.ViewModels.Windows;

namespace StepinFlow.Views.Windows
{
    public partial class ScreenshotSelectionWindow : Window
    {
        public ScreenshotSelectionWindowVM ViewModel { get; }

        public ScreenshotSelectionWindow(ScreenshotSelectionWindowVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            viewModel.CloseWindow += () => this.Close();
        }
    }
}