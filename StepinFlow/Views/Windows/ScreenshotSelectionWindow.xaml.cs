using System.Windows;
using StepinFlow.ViewModels.Windows;

namespace StepinFlow.Views.Windows
{
    public partial class ScreenshotSelectionWindow : Window
    {
        public ScreenshotSelectionWindowViewModel ViewModel { get; }

        public ScreenshotSelectionWindow(ScreenshotSelectionWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            viewModel.CloseWindow += () => this.Close();
        }
    }
}