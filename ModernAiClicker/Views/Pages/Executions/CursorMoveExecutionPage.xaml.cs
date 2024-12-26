using Business.Interfaces;
using DataAccess.Repository.Interface;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    
    public partial class CursorMoveExecutionPage : Page, IExecutionPage
    {
        public CursorMoveExecutionViewModel ViewModel { get; set; }

        public CursorMoveExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new CursorMoveExecutionViewModel(baseDatawork);
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (CursorMoveExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }

    }
}
