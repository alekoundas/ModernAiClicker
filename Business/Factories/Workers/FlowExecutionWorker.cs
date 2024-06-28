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

        public FlowExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task<Execution> CreateExecutionModel(int flowId, int? _)
        {
            Execution execution = new Execution();
            execution.FlowId = flowId;

            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            return execution;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            // Should never execute.
            throw new NotImplementedException();
        }

        public async Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            Expression<Func<FlowStep, bool>> firstFlowStepFilter =
                (x) => x.FlowId == execution.FlowId
                    && x.OrderingNum == 0
                    && x.FlowStepType != FlowStepTypesEnum.IS_NEW;

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(firstFlowStepFilter);

            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            // Flow steps dont have siblings
            return await Task.FromResult<FlowStep?>(null);
        }

        public async Task SetExecutionModelStateRunning(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            await _baseDatawork.SaveChangesAsync();
        }

        public async Task SetExecutionModelStateComplete(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.COMPLETED;
            execution.EndedOn = DateTime.Now;

            await _baseDatawork.SaveChangesAsync();
        }

        public void ExpandAndSelectFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            execution.FlowStep.IsSelected = true;
            execution.FlowStep.IsExpanded = true;
        }
    }
}
