using Business.Interfaces;
using DataAccess.Repository.Interface;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class GoToExecutionPage : Page, IExecutionPage
    {
        public GoToExecutionViewModel ViewModel { get; set; }
        public GoToExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new GoToExecutionViewModel(baseDatawork);
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (GoToExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
