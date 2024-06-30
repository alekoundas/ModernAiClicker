using Business.Factories;
using Business.Interfaces;
using DataAccess;
using DataAccess.Repository.Interface;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages
{
    public partial class ExecutionPage : INavigableView<ExecutionViewModel>
    {
        public ExecutionViewModel ViewModel { get; }
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly IExecutionFactory _executionFactory;


        public ExecutionPage(
              ExecutionViewModel viewModel
            , ISystemService systemService
            , ITemplateSearchService templateMatchingService
            , IBaseDatawork baseDatawork
            , IExecutionFactory executionFactory)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _executionFactory = executionFactory;

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            viewModel.FrameNavigateToFlow += FrameNavigateToFlow;
        }


        public void FrameNavigateToFlow(Flow flow, ObservableCollection<Execution> executions)
        {
            FrameExecutionFlowViewModel frameViewModel = new FrameExecutionFlowViewModel(_baseDatawork, _executionFactory);
            frameViewModel.Flow = flow;
            frameViewModel.Executions = executions;

            UIFlowStepDetailFrame.Navigate(new FrameExecutionFlowPage(frameViewModel));
        }

    }
}
