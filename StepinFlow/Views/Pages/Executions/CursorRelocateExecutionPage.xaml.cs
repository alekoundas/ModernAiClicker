using Business.Interfaces;
using DataAccess.Repository.Interface;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    
    public partial class CursorRelocateExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public CursorRelocateExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new CursorRelocateExecutionVM(baseDatawork);
            InitializeComponent();
            DataContext = this;
        }
    }
}
