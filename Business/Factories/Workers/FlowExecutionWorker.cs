using AutoMapper;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories.Workers
{
    public class FlowExecutionWorker : IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly IMapper _mapper;

        public FlowExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService, IMapper mapper)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _mapper = mapper;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            throw new NotImplementedException();
        }

        public async Task<Execution?> GetNextStep(int id)
        {
            Execution execution = new Execution();
            execution.FlowId = id;
            _baseDatawork.Executions.Add(execution);


            Expression<Func<FlowStep, bool>> firstFlowStepFilter =
                (x) => x.FlowId == id
                    && x.OrderingNum == 0
                    && x.FlowStepType != FlowStepTypesEnum.IS_NEW;

            FlowStep? flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(firstFlowStepFilter);

            if (flowStep == null)
                return null;

            execution = new Execution();
            execution.FlowStepId = flowStep.Id;

            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();


            
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            return execution;
        }
    }
}
