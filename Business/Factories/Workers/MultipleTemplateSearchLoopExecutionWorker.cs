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
            execution.ParentExecutionId = parentExecution.Id;// TODO This is wrong!
            execution.ParentLoopExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;
            execution.LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1;


            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;// TODO propably also this is wrong
            parentExecution.ChildLoopExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            execution.FlowStep = flowStep;
            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            List<Execution> executions = _baseDatawork.Executions.GetAll();


            // Get recursively all parents of loop execution.
            List<Execution?> parentLoopExecutions = executions
                .First(x => x.Id == execution.ParentLoopExecutionId)
                .SelectRecursive<Execution?>(x => x.ParentLoopExecution)
                .Where(x => x != null)
                .ToList();

            // Get all completed children template flow steps.
            List<int> completedChildrenTemplateFlowStepIds = executions
                .Where(x => parentLoopExecutions.Any(y => y.Id == x.Id))
                .Where(x => x.ExecutionResultEnum == ExecutionResultEnum.FAIL || (x.FlowStep.MaxLoopCount > 0 && x.LoopCount >= x.FlowStep.MaxLoopCount))
                .Select(x => x.FlowStepId ?? 0)
                .ToList();

            // Get first child template search flow step that isnt completed.
            // TODO 
            var children = await _baseDatawork.Query.FlowSteps
             .AsNoTracking()
            .Where(x => x.ParentTemplateSearchFlowStepId == execution.FlowStepId)
             .ToListAsync();
            children = children.Where(x => !completedChildrenTemplateFlowStepIds.Any(y => y == x.id)).ToList();
            //var children = await _baseDatawork.Query.FlowSteps
            //             .AsNoTracking()
            //             .Where(x => x.ParentTemplateSearchFlowStepId == execution.FlowStepId)
            //             .ToListAsync();

            //children = children
            //    .Where(x => !completedChildrenTemplateFlowStepIds.Any(y => y == x.Id)).ToList();

            FlowStep? childTemplateSearchFlowStep = children
            .OrderBy(x => x.Id)
            .FirstOrDefault();


            // If all children are completed, set execution as complete.
            // Else execute step.
            if (childTemplateSearchFlowStep == null)
            {
                execution.ExecutionResultEnum = ExecutionResultEnum.NO_RESULT; // TODO Propably need to be changed to complete. Check!
                return;
            }

            if (childTemplateSearchFlowStep.TemplateImage == null)
            {
                execution.ExecutionResultEnum = ExecutionResultEnum.FAIL;
                return;
            }


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
            Execution? parentLoopExecution = await _baseDatawork.Executions.FirstOrDefaultAsync(x => x.Id == execution.ParentLoopExecutionId);
            if (parentLoopExecution.ResultImagePath?.Length > 0 && childTemplateSearchFlowStep.RemoveTemplateFromResult)
                screenshot = (Bitmap)Image.FromFile(parentLoopExecution.ResultImagePath);
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle);

            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(childTemplateSearchFlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, screenshot, execution.FlowStep.RemoveTemplateFromResult);
                ImageSizeResult imageSizeResult = _systemService.GetImageSize(childTemplateSearchFlowStep.TemplateImage);

                int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
                int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);

                bool isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;
                execution.ExecutionResultEnum = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;
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
                else if (execution.LoopCount < execution.FlowStep.MaxLoopCount)
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