using Business.Interfaces;
using DataAccess.Repository.Interface;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    
    public partial class CursorMoveExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public CursorMoveExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new CursorMoveExecutionViewModel(baseDatawork);
            InitializeComponent();
            DataContext = this;
        }
    }
}
