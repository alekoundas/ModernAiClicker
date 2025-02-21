using Business.Factories.Workers;
using Business.Interfaces;
using Business.Services;
using DataAccess.Repository.Interface;
using Model.Enums;

namespace Business.Factories
{
    public class ExecutionFactory : IExecutionFactory
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateSearchService;
        private Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>>? _workerCache = null;

        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();


        public ExecutionFactory(IBaseDatawork baseDatawork, ISystemService systemService, ITemplateSearchService templateSearchService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateSearchService = templateSearchService;
        }

        public IExecutionWorker GetWorker(FlowStepTypesEnum? Type)
        {
            if (_workerCache == null)
                _workerCache = GetWorkers();

            // Lazy initialization (only created on the first access).
            if (Type.HasValue && _workerCache.TryGetValue(Type.Value, out var lazyWorker))
                return lazyWorker.Value;

            // Default worker for null or unrecognized types.
            // should never get here
            return new FlowExecutionWorker(_baseDatawork, _systemService);
        }

        public void SetCancellationToken(CancellationTokenSource cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public void DestroyWorkers()
        {
            _workerCache = null;
        }

        private Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>> GetWorkers()
        {
            return new Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionWorker>(() => new WindowMoveExecutionWorker(_baseDatawork, _systemService)) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionWorker>(() => new WindowResizeExecutionWorker(_baseDatawork, _systemService)) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionWorker>(() => new MouseMoveExecutionWorker(_baseDatawork, _systemService)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionWorker>(() => new MouseClickExecutionWorker(_baseDatawork, _systemService)) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionWorker>(() => new MouseScrollExecutionWorker(_baseDatawork, _systemService)) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionWorker>(() => new WaitForTemplateExecutionWorker(_baseDatawork, _systemService, _templateSearchService)) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionWorker>(() => new TemplateSearchExecutionWorker(_baseDatawork, _systemService, _templateSearchService)) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionWorker>(() => new TemplateSearchLoopExecutionWorker(_baseDatawork, _systemService, _templateSearchService)) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionWorker>(() => new MultipleTemplateSearchExecutionWorker(_baseDatawork, _systemService, _templateSearchService)) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionWorker>(() => new MultipleTemplateSearchLoopExecutionWorker(_baseDatawork, _systemService, _templateSearchService)) },
                { FlowStepTypesEnum.WAIT, new Lazy<IExecutionWorker>(() => new SleepExecutionWorker(_baseDatawork, _systemService,_cancellationToken)) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionWorker>(() => new GoToExecutionWorker(_baseDatawork, _systemService)) },
                { FlowStepTypesEnum.LOOP, new Lazy<IExecutionWorker>(() => new LoopExecutionWorker(_baseDatawork, _systemService)) }
            };
        }

    }
}
