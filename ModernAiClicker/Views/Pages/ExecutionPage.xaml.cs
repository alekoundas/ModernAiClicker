using Business.Factories;
using Business.Factories.Workers;
using Business.Interfaces;
using Business.Services;
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
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>> _workerCache;


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


            _workerCache = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionWorker>(() => new WindowMoveExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionWorker>(() => new WindowResizeExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionWorker>(() => new MouseMoveExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionWorker>(() => new MouseClickExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionWorker>(() => new MouseScrollExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionWorker>(() => new TemplateSearchExecutionWorker(baseDatawork, systemService, templateMatchingService)) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionWorker>(() => new SleepExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionWorker>(() => new GoToExecutionWorker(baseDatawork, systemService)) }
            };
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
            else if (flowStepType == FlowStepTypesEnum.MOUSE_SCROLL)
            {
                CursorScrollExecutionViewModel viewModel = new CursorScrollExecutionViewModel(execution, _systemService, _baseDatawork);
                this.UIExecutionDetailFrame.Navigate(new CursorScrollExecutionPage(viewModel));
            }
        }
    }
}
