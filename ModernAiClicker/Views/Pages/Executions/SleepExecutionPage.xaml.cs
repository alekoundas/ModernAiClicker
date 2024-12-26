using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class SleepExecutionPage : Page, IExecutionPage
    {
        public SleepExecutionViewModel ViewModel { get; set; }
        public SleepExecutionPage()
        {
            ViewModel = new SleepExecutionViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (SleepExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
