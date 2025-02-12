using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class SleepFlowStepPage : Page, IPage
    {
        public IFlowStepViewModel ViewModel { get; set; }
        public SleepFlowStepPage(SleepFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
