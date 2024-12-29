using Business.Extensions;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class MultipleTemplateSearchLoopExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;
        private string _previousResultImagePath = "";

        public MultipleTemplateSearchLoopExecutionWorker(
              IBaseDatawork baseDatawork
            , ISystemService systemService
            , ITemplateSearchService templateSearchService
            ) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _templateSearchService = templateSearchService;
            _systemService = systemService;
        }

        public async override Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution? parentExecution)
        {
            if (parentExecution == null)
                throw new ArgumentNullException(nameof(parentExecution));

            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;
            execution.CurrentLoopCount = parentExecution?.CurrentLoopCount == null ? 0 : parentExecution.CurrentLoopCount + 1;
            _previousResultImagePath = parentExecution?.ResultImagePath ?? "";


            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            execution.FlowStep = flowStep;
            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (execution.FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            // New if not previous exists.
            // Get previous one if exists.
            Bitmap? screenshot = null;
            if (_previousResultImagePath.Length > 0)
                screenshot = (Bitmap)Image.FromFile(_previousResultImagePath);
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle);

            if (screenshot == null)
                return;

            ImageSizeResult imageSizeResult = _systemService.GetImageSize(execution.FlowStep.TemplateImage);
            using (var ms = new MemoryStream(execution.FlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, screenshot, execution.FlowStep.RemoveTemplateFromResult);

                int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
                int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);

                bool isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;
                execution.ExecutionResultEnum = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;
                _previousResultImagePath = isSuccessful ? result.ResultImagePath : "";// TODO Find a better way.

                execution.ResultLocationX = x;
                execution.ResultLocationY = y;
                execution.ResultImagePath = result.ResultImagePath;
                execution.ResultAccuracy = result.Confidence;

                await _baseDatawork.SaveChangesAsync();
            }
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

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // If execution was successfull and (MaxLoopCount is 0 or CurrentLoopCount < MaxLoopCount), return te same flow step.
            if (execution.ExecutionResultEnum == ExecutionResultEnum.SUCCESS)
            {
                if (execution.FlowStep.MaxLoopCount == 0)
                    return execution.FlowStep;
                else if (execution.CurrentLoopCount < execution.FlowStep.MaxLoopCount)
                    return execution.FlowStep;
            }

            // If not, get next sibling flow step. 
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

            return nextFlowStep;
        }

        public async override Task SaveToDisk(Execution execution)
        {
            if (!execution.ParentExecutionId.HasValue || execution.ResultImagePath == null || execution.ExecutionFolderDirectory.Length == 0)
                return;

            string fileDate = execution.StartedOn.Value.ToString("yy-MM-dd hh.mm.ss");
            string newFilePath = execution.ExecutionFolderDirectory + "\\" + fileDate + ".png";

            _systemService.CopyImageToDisk(execution.ResultImagePath, newFilePath);
            execution.ResultImagePath = newFilePath;
        }
    }
}