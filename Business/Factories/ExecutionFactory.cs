using Business.Factories.Workers;
using Business.Interfaces;
using Business.Services;
using DataAccess;
using DataAccess.Repository.Interface;
using Model.Enums;

namespace Business.Factories
{
    public class ExecutionFactory : IExecutionFactory
    {

        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>> _workerCache;

        public ExecutionFactory(IBaseDatawork baseDatawork, ISystemService systemService, ITemplateSearchService templateSearchService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateSearchService = templateSearchService;


            _workerCache = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionWorker>>()
            {
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionWorker>(() => new WindowMoveExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionWorker>(() => new WindowResizeExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionWorker>(() => new MouseMoveExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionWorker>(() => new MouseClickExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionWorker>(() => new MouseScrollExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionWorker>(() => new TemplateSearchExecutionWorker(baseDatawork, systemService, templateSearchService)) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IExecutionWorker>(() => new SleepExecutionWorker(baseDatawork, systemService)) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionWorker>(() => new GoToExecutionWorker(baseDatawork, systemService)) }
            };
        }

        //public IExecutionWorker GetWorker(FlowStepTypesEnum? flowStepType)
        //{
        //    switch (flowStepType)
        //    {
        //        case FlowStepTypesEnum.WINDOW_MOVE:
        //            return new WindowMoveExecutionWorker(_baseDatawork, _systemService);
        //        case FlowStepTypesEnum.WINDOW_RESIZE:
        //            return new WindowResizeExecutionWorker(_baseDatawork, _systemService);
        //        case FlowStepTypesEnum.MOUSE_MOVE_COORDINATES:
        //            return new MouseMoveExecutionWorker(_baseDatawork, _systemService);
        //        case FlowStepTypesEnum.MOUSE_CLICK:
        //            return new MouseClickExecutionWorker(_baseDatawork, _systemService);
        //        case FlowStepTypesEnum.MOUSE_SCROLL:
        //            return new MouseScrollExecutionWorker(_baseDatawork, _systemService);
        //        case FlowStepTypesEnum.TEMPLATE_SEARCH:
        //            return new TemplateSearchExecutionWorker(_baseDatawork, _systemService, _templateSearchService);
        //        case FlowStepTypesEnum.SLEEP:
        //            return new SleepExecutionWorker(_baseDatawork, _systemService);
        //        case FlowStepTypesEnum.GO_TO:
        //            return new GoToExecutionWorker(_baseDatawork, _systemService);
        //        default:
        //            return new FlowExecutionWorker(_baseDatawork, _systemService);
        //    }
        //}

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
