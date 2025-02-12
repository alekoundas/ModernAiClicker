using Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Enums;
using StepinFlow.ViewModels.UserControls;
using StepinFlow.Views.Pages.FlowStepDetail;
using System.Windows.Controls;


namespace StepinFlow.Views.UserControls
{
    public partial class FlowStepFrameUserControl : UserControl
    {
        public FlowStepFrameUserControlViewModel ViewModel { get; set; }

        public FlowStepFrameUserControl()
        {
            FlowStepFrameUserControlViewModel? viewModel = App.GetService<FlowStepFrameUserControlViewModel>();

            if (viewModel == null )
                throw new InvalidOperationException("Failed to resolve from DI container.");


            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
