using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class WindowMoveExecutionPage : Page, IExecutionPage
    {
        public WindowMoveExecutionViewModel ViewModel { get; set; }
        public WindowMoveExecutionPage()
        {
            DataContext = this;
            ViewModel = new WindowMoveExecutionViewModel();
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (WindowMoveExecutionViewModel)executionViewModel;
            DataContext = ViewModel;

        }
    }
}
