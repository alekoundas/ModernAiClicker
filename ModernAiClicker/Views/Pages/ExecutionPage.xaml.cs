using Business.Factories;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using ModernAiClicker.Views.Pages.Executions;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using System.Collections.ObjectModel;
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

            //viewModel.FrameNavigateToFlow += FrameNavigateToFlow;
            viewModel.NavigateToExecutionDetail += NavigateToExecutionDetailEvent;
        }

        



        public void NavigateToExecutionDetailEvent(FlowStepTypesEnum flowStepType, Execution? execution)
        {
            //TODO reset frame to default
            if (execution == null)
                return;

            if (flowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
            {
                TemplateSearchExecutionViewModel viewModel = new TemplateSearchExecutionViewModel(execution, _systemService, _templateMatchingService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new TemplateSearchExecutionPage(viewModel));
            }
            else if (flowStepType == FlowStepTypesEnum.MOUSE_CLICK)
            {
                CursorClickExecutionViewModel viewModel = new CursorClickExecutionViewModel(execution, _systemService, _templateMatchingService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new CursorClickExecutionPage(viewModel));
            }
            else if (flowStepType == FlowStepTypesEnum.MOUSE_MOVE_COORDINATES)
            {
                CursorMoveExecutionViewModel viewModel = new CursorMoveExecutionViewModel(execution, _systemService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new CursorMoveExecutionPage(viewModel));
            }
            else if (flowStepType == FlowStepTypesEnum.SLEEP)
            {
                SleepExecutionViewModel viewModel = new SleepExecutionViewModel(execution, _systemService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new SleepExecutionPage(viewModel));
            }
            else if (flowStepType == FlowStepTypesEnum.GO_TO)
            {
                GoToExecutionViewModel viewModel = new GoToExecutionViewModel(execution, _systemService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new GoToExecutionPage(viewModel));
            }
            else if (flowStepType == FlowStepTypesEnum.WINDOW_RESIZE)
            {
                WindowResizeExecutionViewModel viewModel = new WindowResizeExecutionViewModel(execution, _systemService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new WindowResizeExecutionPage(viewModel));
            }
            else if (flowStepType == FlowStepTypesEnum.WINDOW_MOVE)
            {
                WindowMoveExecutionViewModel viewModel = new WindowMoveExecutionViewModel(execution, _systemService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new WindowMoveExecutionPage(viewModel));
            }
        }
    }
}
