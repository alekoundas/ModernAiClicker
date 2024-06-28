using Business.Helpers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class TemplateSearchExecutionWorker : IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        public TemplateSearchExecutionWorker(IBaseDatawork baseDatawork, ITemplateSearchService templateSearchService, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _templateSearchService = templateSearchService;
            _systemService = systemService;
        }

        public async Task<Execution> CreateExecutionModel(int flowStepId, int? parentExecutionId)
        {
            Execution execution = new Execution();
            execution.FlowStepId = flowStepId;
            execution.ParentExecutionId = parentExecutionId;

            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return;
            execution.FlowStep.IsSelected = true;
            Rectangle searchRectangle;

            if (execution.FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();


            TemplateMatchingResult result = _templateSearchService.SearchForTemplate(execution.FlowStep.TemplateImagePath, searchRectangle);
            int x = searchRectangle.Left + result.ResultRectangle.Top;
            int y = searchRectangle.Top + result.ResultRectangle.Left;

            execution.IsSuccessful = execution.FlowStep.Accuracy <= result.Confidence;
            execution.ResultLocation = new Point(x, y);

            await _baseDatawork.SaveChangesAsync();
        }

        public async Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep;

            // Get next executable child.
            if (execution.IsSuccessful)
                nextFlowStep = _baseDatawork.FlowSteps
                    .Where(x => x.Id == execution.FlowStepId)
                    .Select(x => x?.ChildrenFlowSteps?.First(y => y.FlowStepType == FlowStepTypesEnum.IS_SUCCESS))
                    .Select(x => x?.ChildrenFlowSteps?.FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW && x.OrderingNum == 0))
                    .FirstOrDefault();
            else
                nextFlowStep = _baseDatawork.FlowSteps
                    .Where(x => x.Id == execution.FlowStepId)
                    .Select(x => x?.ChildrenFlowSteps?.First(y => y.FlowStepType == FlowStepTypesEnum.IS_FAILURE))
                    .Select(x => x?.ChildrenFlowSteps?.FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW && x.OrderingNum == 0))
                    .FirstOrDefault();


            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // Get next sibling flow step. 
            Expression<Func<FlowStep, bool>> nextStepFilter;

            if (execution.FlowStep.ParentFlowStepId != null)
                nextStepFilter = (x) =>
                       x.FlowStepType != FlowStepTypesEnum.IS_NEW
                    && x.OrderingNum > execution.FlowStep.OrderingNum
                    && x.ParentFlowStepId == execution.FlowStep.ParentFlowStepId;
            else
                nextStepFilter = (x) =>
                       x.FlowStepType != FlowStepTypesEnum.IS_NEW
                    && x.OrderingNum > execution.FlowStep.OrderingNum
                    && x.FlowId == execution.FlowStep.FlowId;

            List<FlowStep>? nextFlowSteps = await _baseDatawork.Query.FlowSteps
                .Where(nextStepFilter)
                .ToListAsync();

            FlowStep? nextFlowStep = nextFlowSteps.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);


            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }

        public async Task SetExecutionModelStateRunning(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            await _baseDatawork.SaveChangesAsync();
        }

        public async Task SetExecutionModelStateComplete(Execution execution)
        {
            execution.Status = execution.IsSuccessful ? ExecutionStatusEnum.COMPLETED : ExecutionStatusEnum.ACCURACY_FAIL;
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
