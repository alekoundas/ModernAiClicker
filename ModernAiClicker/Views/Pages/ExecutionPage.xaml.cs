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
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionViewModel>> _executionViewModelCache;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>> _executionPageCache;


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


            _executionViewModelCache = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionViewModel>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionViewModel>(() => new WindowMoveExecutionViewModel()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionViewModel>(() => new WindowResizeExecutionViewModel   ()) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionViewModel>(() => new CursorMoveExecutionViewModel(baseDatawork)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionViewModel>(() => new CursorClickExecutionViewModel  ()) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionViewModel>(() => new CursorScrollExecutionViewModel()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionViewModel>(() => new TemplateSearchExecutionViewModel()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionViewModel>(() => new TemplateSearchLoopExecutionViewModel()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionViewModel>(() => new MultipleTemplateSearchExecutionViewModel()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionViewModel>(() => new MultipleTemplateSearchLoopExecutionViewModel()) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionViewModel>(() => new SleepExecutionViewModel  ()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionViewModel>(() => new GoToExecutionViewModel(baseDatawork)) }
            };

            _executionPageCache = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionPage>(() => new WindowMoveExecutionPage()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionPage>(() => new WindowResizeExecutionPage()) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionPage>(() => new CursorMoveExecutionPage(baseDatawork)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionPage>(() => new CursorClickExecutionPage()) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionPage>(() => new CursorScrollExecutionPage()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => new TemplateSearchExecutionPage()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => new TemplateSearchLoopExecutionPage()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => new MultipleTemplateSearchExecutionPage()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => new MultipleTemplateSearchLoopExecutionPage()) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionPage>(() => new SleepExecutionPage()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionPage>(() => new GoToExecutionPage(baseDatawork)) }
            };
        }

        public void NavigateToExecutionDetailEvent(FlowStepTypesEnum flowStepType, Execution? execution)
        {
            //TODO reset frame to default
            if (execution == null)
                return;

            var viewModel = GetViewModel(flowStepType);
            var page = GetPage(flowStepType);

            if (viewModel == null || page == null)
                return;

            viewModel.SetExecution(execution);
            page.SetViewModel(viewModel);
            this.UIExecutionDetailFrame.Navigate(page);
        }

        private IExecutionViewModel? GetViewModel(FlowStepTypesEnum? flowStepType)
        {
            // Lazy initialization (only created on the first access).
            if (flowStepType.HasValue && _executionViewModelCache.TryGetValue(flowStepType.Value, out var lazyWorker))
                return lazyWorker.Value;

            return null;
        }


        private IExecutionPage? GetPage(FlowStepTypesEnum? flowStepType)
        {
            // Lazy initialization (only created on the first access).
            if (flowStepType.HasValue && _executionPageCache.TryGetValue(flowStepType.Value, out var lazyWorker))
                return lazyWorker.Value;

            return null;
        }

    }
}
