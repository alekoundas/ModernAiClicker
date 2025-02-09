using Business.Factories;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using StepinFlow.ViewModels.Pages;
using StepinFlow.ViewModels.Pages.Executions;
using StepinFlow.Views.Pages.Executions;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages
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
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionViewModel>(() => new WaitForTemplateExecutionViewModel()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionViewModel>(() => new TemplateSearchExecutionViewModel()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionViewModel>(() => new TemplateSearchLoopExecutionViewModel()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionViewModel>(() => new MultipleTemplateSearchExecutionViewModel(baseDatawork)) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionViewModel>(() => new MultipleTemplateSearchLoopExecutionViewModel(baseDatawork)) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionViewModel>(() => new SleepExecutionViewModel  ()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionViewModel>(() => new GoToExecutionViewModel(baseDatawork)) },
                { FlowStepTypesEnum.LOOP, new Lazy<IExecutionViewModel>(() => new LoopExecutionViewModel()) }
            };

            _executionPageCache = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionPage>(() => new WindowMoveExecutionPage()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionPage>(() => new WindowResizeExecutionPage()) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionPage>(() => new CursorMoveExecutionPage(baseDatawork)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionPage>(() => new CursorClickExecutionPage()) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionPage>(() => new CursorScrollExecutionPage()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionPage>(() => new WaitForTemplateExecutionPage()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => new TemplateSearchExecutionPage()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => new TemplateSearchLoopExecutionPage()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => new MultipleTemplateSearchExecutionPage(baseDatawork)) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => new MultipleTemplateSearchLoopExecutionPage(baseDatawork)) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionPage>(() => new SleepExecutionPage()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionPage>(() => new GoToExecutionPage(baseDatawork)) },
                { FlowStepTypesEnum.LOOP, new Lazy<IExecutionPage>(() => new LoopExecutionPage()) }
            };
        }

        public void NavigateToExecutionDetailEvent(FlowStepTypesEnum flowStepType, Execution? execution)
        {
            //TODO reset frame to default
            if (execution == null)
                return;

            if (flowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
                flowStepType = FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH;

            if (flowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD)
                flowStepType = FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP;


            IExecutionViewModel? viewModel = GetViewModel(flowStepType);
            IExecutionPage? page = GetPage(flowStepType);

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
