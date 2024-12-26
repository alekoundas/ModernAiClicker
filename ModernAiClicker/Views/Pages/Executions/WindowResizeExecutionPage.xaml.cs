using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class WindowResizeExecutionPage : Page, IExecutionPage
    {
        public WindowResizeExecutionViewModel ViewModel { get; set; }
        public WindowResizeExecutionPage()
        {
            ViewModel = new WindowResizeExecutionViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (WindowResizeExecutionViewModel)executionViewModel;
            DataContext = ViewModel;

        }
    }
}
