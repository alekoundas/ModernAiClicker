﻿using Business.Helpers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories.Workers
{
    public class MouseClickExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public MouseClickExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService):base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task<Execution> CreateExecutionModel(int flowStepId, Execution parentExecution)
        {
            Execution execution = new Execution();
            execution.FlowStepId = flowStepId;
            execution.ParentExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;

            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            return execution;
        }


        public Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return Task.CompletedTask;

            switch (execution.FlowStep.MouseAction)
            {
                case MouseActionsEnum.SINGLE_CLICK:
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    break;
                case MouseActionsEnum.DOUBLE_CLICK:
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    break;
                // TODO
                case MouseActionsEnum.LOOP_CLICK:
                    do
                    {
                        _systemService.CursorClick(execution.FlowStep.MouseButton);
                    } while (true);
                default:
                    break;
            }

            return Task.CompletedTask;
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

            FlowStep? nextFlowStep = null;
            if (nextFlowSteps.Any())
                nextFlowStep = nextFlowSteps.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);

            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            // Cursor move doesnt contain any children.
            return await Task.FromResult<FlowStep?>(null);
        }

        public async Task SetExecutionModelStateRunning(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            if (execution.ParentExecution != null)
                execution.ExecutionFolderDirectory = execution.ParentExecution.ExecutionFolderDirectory;

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

            execution.FlowStep.IsExpanded = true;
            execution.FlowStep.IsSelected = true;
        }

    }
}
