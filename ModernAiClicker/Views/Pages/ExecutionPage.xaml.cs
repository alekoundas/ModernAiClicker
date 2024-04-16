using Business.Interfaces;
using DataAccess;
using DataAccess.Repository.Interface;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using System.Windows;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages
{
    public partial class ExecutionPage : INavigableView<ExecutionViewModel>
    {
        public ExecutionViewModel ViewModel { get; }
        private readonly ISystemService _systemService;
        private readonly ITemplateMatchingService _templateMatchingService;
        private readonly IFlowService _flowService;
        private readonly IBaseDatawork _baseDatawork;


        public ExecutionPage(ExecutionViewModel viewModel, ISystemService systemService, ITemplateMatchingService templateMatchingService, IFlowService flowService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _flowService = flowService;

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            viewModel.FrameNavigateToFlow += FrameNavigateToFlow;
        }


        public void FrameNavigateToFlow(Flow flow)
        {
            FrameExecutionFlowViewModel frameViewModel = new FrameExecutionFlowViewModel(_baseDatawork, _flowService);
            frameViewModel.Flow= flow;
            _flowService.InitializeExecutionModels(flow);
            UIFlowStepDetailFrame.Navigate(new FrameExecutionFlowPage(frameViewModel,_baseDatawork));
        }

    }
}
