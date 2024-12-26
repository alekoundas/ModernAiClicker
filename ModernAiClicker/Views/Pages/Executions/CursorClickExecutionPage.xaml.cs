using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class CursorClickExecutionPage : Page, IExecutionPage
    {
        public CursorClickExecutionViewModel ViewModel { get; set; }
        public CursorClickExecutionPage()
        {
            ViewModel = new CursorClickExecutionViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (CursorClickExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
