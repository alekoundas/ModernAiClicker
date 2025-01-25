using Business.Extensions;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using OpenCvSharp;
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

        private byte[]? _resultImage = null;

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

            // Get all parents of loop execution.
            List<Execution> parentLoopExecutions = new List<Execution>();
            Execution? currentExecution = parentExecution;
            while (currentExecution.ParentLoopExecutionId != null)
            {
                parentLoopExecutions.Add(currentExecution);

                currentExecution = await _baseDatawork.Executions.Query
                    .AsNoTracking()
                    .Include(x => x.FlowStep)
                    .FirstAsync(x => x.Id == currentExecution.ParentLoopExecutionId.Value);
            }

            // Get all completed children template flow steps.
            List<int> completedChildrenTemplateFlowStepIds = parentLoopExecutions
                .Where(x => x.ExecutionResultEnum == ExecutionResultEnum.FAIL || (x.FlowStep.MaxLoopCount > 0 && x.LoopCount >= x.FlowStep.MaxLoopCount))
                .Select(x => x.CurrentMultipleTemplateSearchFlowStepId ?? 0)
                .Where(x => x != 0)
                .ToList();

            // Get all child template search flow steps.
            var children = await _baseDatawork.FlowSteps.Query
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == flowStep.Id)
                .Where(x => x.FlowStepType == FlowStepTypesEnum.NO_SELECTION)
                .ToListAsync();

            // Get first child template search flow step that isnt completed.
            FlowStep? childTemplateSearchFlowStep = children
                .Where(x => !completedChildrenTemplateFlowStepIds.Any(y => y == x.id))
                .ToList()
                .OrderBy(x => x.Id)
                .FirstOrDefault();









            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = parentExecution.Id;// TODO This is wrong!
            execution.ParentLoopExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;
            execution.LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1;
            execution.CurrentMultipleTemplateSearchFlowStepId = childTemplateSearchFlowStep?.Id;
            //execution.CurrentMultipleTemplateSearchFlowStep = childTemplateSearchFlowStep;


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
            FlowStep? currentMultipleTemplateSearchFlowStep = await _baseDatawork.FlowSteps.Query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == execution.CurrentMultipleTemplateSearchFlowStepId);
            Execution? parentLoopExecution = await _baseDatawork.Executions.FirstOrDefaultAsync(x => x.Id == execution.ParentLoopExecutionId);
            bool canUseParentResult = parentLoopExecution?.CurrentMultipleTemplateSearchFlowStepId == execution.CurrentMultipleTemplateSearchFlowStepId && currentMultipleTemplateSearchFlowStep.RemoveTemplateFromResult; // Refresh image if parent is not the same step or remove template from result is false.
            if (parentLoopExecution?.ResultImagePath?.Length > 0 && canUseParentResult)
                screenshot = (Bitmap)Image.FromFile(parentLoopExecution.ResultImagePath);
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle);

            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(currentMultipleTemplateSearchFlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, screenshot, currentMultipleTemplateSearchFlowStep.RemoveTemplateFromResult);
                ImageSizeResult imageSizeResult = _systemService.GetImageSize(currentMultipleTemplateSearchFlowStep.TemplateImage);

                int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
                int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);

                bool isSuccessful = currentMultipleTemplateSearchFlowStep.Accuracy <= result.Confidence;
                execution.ExecutionResultEnum = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;
                execution.ResultLocationX = x;
                execution.ResultLocationY = y;
                execution.ResultImagePath = result.ResultImagePath;
                execution.ResultAccuracy = result.Confidence;

                await _baseDatawork.SaveChangesAsync();
                _resultImage = result.ResultImage;
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
                    .OrderBy(x => x.OrderingNum)
                    .FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW);
            else
                nextFlowStep = nextFlowStep.ChildrenFlowSteps
                    .First(x => x.FlowStepType == FlowStepTypesEnum.IS_FAILURE)
                    .ChildrenFlowSteps
                    .OrderBy(x => x.OrderingNum)
                    .FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW);

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // Get all parents of loop execution.
            List<Execution> parentLoopExecutions = new List<Execution>();
            Execution? currentExecution = execution;
            while (currentExecution.ParentLoopExecutionId != null)
            {
                parentLoopExecutions.Add(currentExecution);

                currentExecution = await _baseDatawork.Executions.Query
                    .Include(x => x.FlowStep)
                    .FirstAsync(x => x.Id == currentExecution.ParentLoopExecutionId.Value);
            }

            // Get all completed children template flow steps.
            List<int> completedChildrenTemplateFlowStepIds = parentLoopExecutions
                .Where(x => x.ExecutionResultEnum == ExecutionResultEnum.FAIL || (x.FlowStep.MaxLoopCount > 0 && x.LoopCount >= x.FlowStep.MaxLoopCount))
                .Select(x => x.CurrentMultipleTemplateSearchFlowStepId ?? 0)
                .Where(x => x != 0)
                .ToList();

            // Get all child template search flow steps.
            var children = await _baseDatawork.Query.FlowSteps
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == execution.FlowStepId)
                .Where(x => x.FlowStepType == FlowStepTypesEnum.NO_SELECTION)
                .ToListAsync();

            // Find unexecuted flow steps.
            children = children
                .Where(x => !completedChildrenTemplateFlowStepIds.Any(y => y == x.Id)).ToList();

            if (children.Any())
                return execution.FlowStep;







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
            if (!execution.ParentExecutionId.HasValue || execution.ExecutionFolderDirectory.Length == 0)
                return;

            string fileDate = execution.StartedOn.Value.ToString("yy-MM-dd hh.mm.ss.fff");
            string newFilePath = execution.ExecutionFolderDirectory + "\\" + fileDate + ".png";

            //_systemService.CopyImageToDisk(execution.ResultImagePath, newFilePath);_resultImage
            if (_resultImage != null)
                await _systemService.SaveImageToDisk(newFilePath, _resultImage);
            execution.ResultImagePath = newFilePath;
                await _baseDatawork.SaveChangesAsync();
        }
    }
}