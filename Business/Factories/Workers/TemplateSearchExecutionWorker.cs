﻿using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class TemplateSearchExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        public TemplateSearchExecutionWorker(
              IBaseDatawork baseDatawork
            , ISystemService systemService
            , ITemplateSearchService templateSearchService
            ) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _templateSearchService = templateSearchService;
            _systemService = systemService;
        }


        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            Model.Structs.Rectangle searchRectangle;
            if (execution.FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            ImageSizeResult imageSizeResult = _systemService.GetImageSize(execution.FlowStep.TemplateImage);
            Bitmap templateImage;
            using (var ms = new MemoryStream(execution.FlowStep.TemplateImage))
            {
                templateImage = new Bitmap(ms);
            }

            TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, searchRectangle);

            int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
            int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);

            bool isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;
            execution.ExecutionResultEnum = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;

            execution.ResultLocationX = x;
            execution.ResultLocationY = y;
            execution.ResultImage = result.ResultImage;
            execution.ResultImagePath = result.ResultImagePath;
            execution.ResultAccuracy = result.Confidence;

            await _baseDatawork.SaveChangesAsync();
        }

        public async override Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep;

            // Get next executable child.
            nextFlowStep = await _baseDatawork.Query.FlowSteps.AsNoTracking()
                .Include(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .FirstOrDefaultAsync(x => x.Id == execution.FlowStepId);


            if (execution.ExecutionResultEnum == ExecutionResultEnum.SUCCESS)
                nextFlowStep = nextFlowStep.ChildrenFlowSteps
                    .First(x => x.FlowStepType == FlowStepTypesEnum.IS_SUCCESS)
                    .ChildrenFlowSteps
                    .FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW && x.OrderingNum == 0);
            else
                nextFlowStep = nextFlowStep.ChildrenFlowSteps
                    .First(x => x.FlowStepType == FlowStepTypesEnum.IS_FAILURE)
                    .ChildrenFlowSteps
                    .FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW && x.OrderingNum == 0);


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

            List<FlowStep>? nextFlowSteps = await _baseDatawork.Query.FlowSteps.AsNoTracking()
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

        //public async override Task ExpandAndSelectFlowStep(Execution execution)
        //{
        //    if (execution.FlowStep == null)
        //        return;
        //    FlowStep? nextFlowStep = null;

        //    if (execution.ExecutionResultEnum == ExecutionResultEnum.SUCCESS)
        //        nextFlowStep = await _baseDatawork.Query.FlowSteps.AsNoTracking()
        //            .Include(x => x.ChildrenFlowSteps)
        //            .Where(x => x.Id == execution.FlowStepId.Value && x.ChildrenFlowSteps != null)
        //            .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_SUCCESS))
        //            .FirstOrDefaultAsync();
        //    else
        //        nextFlowStep = await _baseDatawork.Query.FlowSteps.AsNoTracking()
        //            .Include(x => x.ChildrenFlowSteps)
        //            .Where(x => x.Id == execution.FlowStepId)
        //            .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_FAILURE))
        //            .FirstOrDefaultAsync();

        //    if (nextFlowStep != null)
        //        nextFlowStep.IsExpanded = true;

        //    if (execution.FlowStep.ChildrenFlowSteps != null && execution.FlowStep.ChildrenFlowSteps.Any())
        //        execution.FlowStep.ChildrenFlowSteps.First().IsExpanded = true;

        //    execution.FlowStep.IsExpanded = true;
        //    execution.FlowStep.IsSelected = true;
        //}

        public async override Task SaveToDisk(Execution execution)
        {
            //if (!execution.ParentExecutionId.HasValue)
            //    return;

            //string folderDir = (await _baseDatawork.Executions.FindAsync(execution.ParentExecutionId.Value)).ExecutionFolderDirectory;

            //if (execution.ParentExecution == null || execution.ResultImage == null)
            //    return;

            //byte[] resultImage = execution.ResultImage;
            //string imagePath = folderDir + "\\";
            //imagePath += execution.Id;
            //imagePath += " - ";
            //imagePath += execution.StartedOn.Value.ToString("yy-MM-dd hh.mm");
            //imagePath += ".png";


            //await _systemService.SaveImageToDisk(imagePath, resultImage);

            //// Remove image from execution in order to free up RAM.
            //// Also assign new image path.
            //execution.ResultImage = null;
            //execution.ResultImagePath = imagePath;
        }
    }
}
