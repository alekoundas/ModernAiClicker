﻿using Business.Extensions;
using Business.Helpers;
using Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;

namespace Business.Factories.Workers
{
    public class MultipleTemplateSearchExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IDataService _dataService;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        private byte[]? _resultImage = null;

        public MultipleTemplateSearchExecutionWorker(
              IDataService dataService
            , ISystemService systemService
            , ITemplateSearchService templateSearchService
            ) : base(dataService, systemService)
        {
            _dataService = dataService;
            _templateSearchService = templateSearchService;
            _systemService = systemService;
        }

        public async override Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution parentExecution, Execution latestParentExecution)
        {
            int loopCount = 0;
            if (parentExecution.FlowStepId == flowStep.Id)
                loopCount = parentExecution.LoopCount.Value + 1;

            // Get first child template search flow step that isnt completed.
            FlowStep? childTemplateSearchFlowStep = await GetChildTemplateSearchFlowStep(flowStep.Id, parentExecution.Id);

            Execution execution = new Execution
            {
                FlowStepId = childTemplateSearchFlowStep?.Id,
                ParentExecutionId = latestParentExecution.Id,
                ParentLoopExecutionId = parentExecution.Id,
                ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory,
                LoopCount = loopCount,
                //LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1,
            };

            // Save execution.
            await _dataService.Executions.AddAsync(execution);

            // Save relation IDs
            parentExecution.ChildExecutionId = execution.Id;// TODO propably also this is wrong
            parentExecution.ChildLoopExecutionId = execution.Id;
            await _dataService.UpdateAsync(execution);

            // Return execution with relations.
            execution = await _dataService.Executions.Query
              .Include(x => x.FlowStep!.ParentTemplateSearchFlowStep)
              .FirstAsync(x => x.Id == execution.Id);

            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep?.ParentTemplateSearchFlowStep == null || execution.FlowStep.ParentTemplateSearchFlowStep.TemplateMatchMode == null)
                return;
            // Find search area.
            Model.Structs.Rectangle? searchRectangle = null;
            switch (execution.FlowStep.ParentTemplateSearchFlowStep.FlowParameter?.TemplateSearchAreaType)
            {
                case TemplateSearchAreaTypesEnum.SELECT_EVERY_MONITOR:
                    searchRectangle = _systemService.GetScreenSize();
                    break;
                case TemplateSearchAreaTypesEnum.SELECT_MONITOR:
                    searchRectangle = _systemService.GetMonitorArea(execution.FlowStep.ParentTemplateSearchFlowStep.FlowParameter.SystemMonitorDeviceName);
                    break;
                case TemplateSearchAreaTypesEnum.SELECT_APPLICATION_WINDOW:
                    searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ParentTemplateSearchFlowStep.FlowParameter.ProcessName);
                    break;
                case TemplateSearchAreaTypesEnum.SELECT_CUSTOM_AREA:
                    break;
                default:
                    searchRectangle = _systemService.GetScreenSize();
                    break;
            }

            if (searchRectangle == null)
                searchRectangle = _systemService.GetScreenSize();



            byte[]? screenshot = null;
            Execution? parentLoopExecution = await _dataService.Executions.Query
                .Include(x => x.FlowStep)
                .FirstOrDefaultAsync(x => x.Id == execution.ParentLoopExecutionId);

            bool canUseParentResult =
               parentLoopExecution?.FlowStepId == execution.FlowStepId &&
               parentLoopExecution?.FlowStep?.IsLoop == true &&
               parentLoopExecution?.TempResultImagePath?.Length > 0 &&
               execution.FlowStep.RemoveTemplateFromResult;

            if (canUseParentResult)
                screenshot = Image.FromFile(parentLoopExecution.TempResultImagePath).ToByteArray();
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle.Value);

            if (screenshot == null)
                return;



            TemplateMatchingResult result = _templateSearchService.SearchForTemplate(execution.FlowStep.TemplateImage, screenshot, execution.FlowStep.ParentTemplateSearchFlowStep.TemplateMatchMode, execution.FlowStep.RemoveTemplateFromResult);
            ImageSizeResult imageSizeResult = _systemService.GetImageSize(execution.FlowStep.TemplateImage);

            int x = searchRectangle.Value.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
            int y = searchRectangle.Value.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);
            bool isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;

            execution.Result = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;
            execution.ResultLocationX = x;
            execution.ResultLocationY = y;
            //execution.ResultImagePath = result.ResultImagePath;
            execution.ResultAccuracy = result.Confidence;

            await _dataService.UpdateAsync(execution);
            _resultImage = result.ResultImage;
        }

        public async override Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            if (execution.FlowStep?.ParentTemplateSearchFlowStepId == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep = await _dataService.FlowSteps.GetNextChild(execution.FlowStep.ParentTemplateSearchFlowStepId.Value, execution.Result);
            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep?.ParentTemplateSearchFlowStepId == null)
                return await Task.FromResult<FlowStep?>(null);

            // Get next child TemplateSearchFlowStep.
            FlowStep? nextChildTemplateSearchFlowStep = await GetChildTemplateSearchFlowStep(execution.FlowStep.ParentTemplateSearchFlowStepId.Value, execution.Id);

            if (nextChildTemplateSearchFlowStep != null)
                return execution.FlowStep.ParentTemplateSearchFlowStep;

            // If not, get next sibling flow step. 
            FlowStep? nextFlowStep = await _dataService.FlowSteps.GetNextSibling(execution.FlowStep.ParentTemplateSearchFlowStepId.Value);
            return nextFlowStep;
        }

        public async override Task SaveToDisk(Execution execution)
        {
            if (!execution.ParentExecutionId.HasValue || execution.ExecutionFolderDirectory.Length == 0)
                return;

            if (execution.StartedOn.HasValue)
            {
                string fileDate = execution.StartedOn.Value.ToString("yy-MM-dd hh.mm.ss.fff");
                string newFilePath = Path.Combine(execution.ExecutionFolderDirectory, fileDate + ".png");

                if (_resultImage != null)
                    await _systemService.SaveImageToDisk(newFilePath, _resultImage);

                if (execution.Result == ExecutionResultEnum.SUCCESS)
                {
                    string tempFilePath = Path.Combine(PathHelper.GetTempDataPath(), fileDate + ".png");
                    if (_resultImage != null)
                        await _systemService.SaveImageToDisk(tempFilePath, _resultImage);

                    execution.TempResultImagePath = tempFilePath;
                }

                execution.ResultImagePath = newFilePath;

            await _dataService.UpdateAsync(execution);
            }
        }

        private async Task<FlowStep?> GetChildTemplateSearchFlowStep(int flowStepId, int parentExecutionId)
        {
            // Get all parents of loop execution.
            List<Execution> parentLoopExecutions = await _dataService.Executions.GetAllParentLoopExecutions(parentExecutionId);

            // Get all completed children template flow steps.
            List<int> completedChildrenTemplateFlowStepIds = parentLoopExecutions
                .Select(x => x.FlowStepId ?? 0)
                .Where(x => x != 0)
                .ToList();

            // Get all child template search flow steps.
            List<FlowStep> children = await _dataService.Query.FlowSteps
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == flowStepId)
                .Where(x => x.Type == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
                .ToListAsync();

            // Get first child template search flow step that isnt completed.
            FlowStep? flowStep = children
                .Where(x => !completedChildrenTemplateFlowStepIds.Any(y => y == x.Id))
                .ToList()
                .OrderBy(x => x.OrderingNum)
                .FirstOrDefault();

            return flowStep;
        }
    }
}