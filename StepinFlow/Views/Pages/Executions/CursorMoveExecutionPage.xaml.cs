using Business.Interfaces;
using DataAccess.Repository.Interface;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    
    public partial class CursorMoveExecutionPage : Page, IExecutionPage
    {
        public CursorMoveExecutionViewModel ViewModel { get; set; }

        public CursorMoveExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new CursorMoveExecutionViewModel(baseDatawork);
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (CursorMoveExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }

    }
}
