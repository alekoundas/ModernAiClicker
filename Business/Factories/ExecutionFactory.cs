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
        private readonly IMapper _mapper;

        public ExecutionFactory(IBaseDatawork baseDatawork, ISystemService systemService, IMapper mapper, ITemplateSearchService templateSearchService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _mapper = mapper;
            _templateSearchService = templateSearchService;
        }

        public IExecutionWorker GetWorker(FlowStep? flowStep)
        {
            switch (flowStep?.FlowStepType)
            {
                case FlowStepTypesEnum.MOUSE_MOVE_COORDINATES:
                    return new ExecutionWorkerMouseMove(_baseDatawork);
                //case FlowStepTypesEnum.MOUSE_CLICK:
                //    return new ExecutionWorkerMouseMove(_baseDatawork);
                case FlowStepTypesEnum.TEMPLATE_SEARCH:
                    return new TemplateSearchExecutionWorker(_baseDatawork, _templateSearchService, _systemService);
                default:
                    return new FlowExecutionWorker(_baseDatawork, _systemService, _mapper);
            }
        }
    }
}
