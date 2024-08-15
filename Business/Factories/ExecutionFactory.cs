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
        private readonly ITemplateSearchService _templateSearchService;

        public ExecutionFactory(IBaseDatawork baseDatawork, ISystemService systemService, ITemplateSearchService templateSearchService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateSearchService = templateSearchService;
        }

        public IExecutionWorker GetWorker(FlowStepTypesEnum? flowStepType)
        {
            switch (flowStepType)
            {
                case FlowStepTypesEnum.WINDOW_MOVE:
                    return new WindowMoveExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.WINDOW_RESIZE:
                    return new WindowResizeExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.MOUSE_MOVE_COORDINATES:
                    return new MouseMoveExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.MOUSE_CLICK:
                    return new MouseClickExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.MOUSE_SCROLL:
                    return new MouseScrollExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.TEMPLATE_SEARCH:
                    return new TemplateSearchExecutionWorker(_baseDatawork, _systemService, _templateSearchService);
                case FlowStepTypesEnum.SLEEP:
                    return new SleepExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.GO_TO:
                    return new GoToExecutionWorker(_baseDatawork, _systemService);
                default:
                    return new FlowExecutionWorker(_baseDatawork, _systemService);
            }
        }
    }
}
