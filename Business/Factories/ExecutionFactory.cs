using AutoMapper;
using Business.Factories.Workers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;

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
                case FlowStepTypesEnum.MOUSE_MOVE_COORDINATES:
                    return new MouseMoveExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.MOUSE_CLICK:
                    return new MouseClickExecutionWorker(_baseDatawork, _systemService);
                case FlowStepTypesEnum.TEMPLATE_SEARCH:
                    return new TemplateSearchExecutionWorker(_baseDatawork, _templateSearchService, _systemService);
                default:
                    return new FlowExecutionWorker(_baseDatawork, _systemService);
            }
        }
    }
}
