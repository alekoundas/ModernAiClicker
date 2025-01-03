using Business.Factories.Workers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;

namespace Business.Factories
{
    public class ExecutionFactory : IExecutionFactory
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>> _workerCache;

        public ExecutionFactory(IBaseDatawork baseDatawork, ISystemService systemService, ITemplateSearchService templateSearchService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _workerCache = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionWorker>(() => new WindowMoveExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionWorker>(() => new WindowResizeExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionWorker>(() => new MouseMoveExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionWorker>(() => new MouseClickExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionWorker>(() => new MouseScrollExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionWorker>(() => new WaitForTemplateExecutionWorker(baseDatawork, systemService, templateSearchService)) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionWorker>(() => new TemplateSearchExecutionWorker(baseDatawork, systemService, templateSearchService)) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionWorker>(() => new TemplateSearchLoopExecutionWorker(baseDatawork, systemService, templateSearchService)) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionWorker>(() => new MultipleTemplateSearchExecutionWorker(baseDatawork, systemService, templateSearchService)) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionWorker>(() => new MultipleTemplateSearchLoopExecutionWorker(baseDatawork, systemService, templateSearchService)) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionWorker>(() => new SleepExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionWorker>(() => new GoToExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.LOOP, new Lazy<IExecutionWorker>(() => new LoopExecutionWorker(baseDatawork, systemService)) }
            };
        }

        public IExecutionWorker GetWorker(FlowStepTypesEnum? flowStepType)
        {
            // Lazy initialization (only created on the first access).
            if (flowStepType.HasValue && _workerCache.TryGetValue(flowStepType.Value, out var lazyWorker))
                return lazyWorker.Value;

            // Default worker for null or unrecognized types.
            // should never get here
            return new FlowExecutionWorker(_baseDatawork, _systemService);
        }
    }
}
