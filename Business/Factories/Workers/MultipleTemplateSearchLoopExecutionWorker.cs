using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;

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


        public async override Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution parentExecution, Execution latestParent )
        {
            if (parentExecution == null)
                throw new ArgumentNullException(nameof(parentExecution));

            // Get first child template search flow step that isnt completed.
            FlowStep? childTemplateSearchFlowStep = await GetChildTemplateSearchFlowStep(flowStep.Id, parentExecution.Id);


            Execution execution = new Execution
            {
                FlowStepId = childTemplateSearchFlowStep?.Id,
                ParentExecutionId = latestParent.Id,
                ParentLoopExecutionId = parentExecution.Id,
                ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory,
                LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1
            };

            // Save execution.
            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            // Save relation IDs
            parentExecution.ChildExecutionId = execution.Id;// TODO propably also this is wrong
            parentExecution.ChildLoopExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            // Return execution with relations.
            execution = await _baseDatawork.Executions.Query
                .Include(x => x.FlowStep)
                .ThenInclude(x => x.ParentTemplateSearchFlowStep)
                .FirstAsync(x => x.Id == execution.Id);

            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep?.ParentTemplateSearchFlowStep == null)
                return;


            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (execution.FlowStep.ParentTemplateSearchFlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ParentTemplateSearchFlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            // New if not previous exists.
            // Get previous one if exists.
            Bitmap? screenshot = null;
            Execution parentLoopExecution = await _baseDatawork.Executions.FirstAsync(x => x.Id == execution.ParentLoopExecutionId);
            bool canUseParentResult = 
                parentLoopExecution?.FlowStepId == execution.FlowStepId &&
                parentLoopExecution?.ResultImagePath?.Length > 0 &&
                execution.FlowStep.RemoveTemplateFromResult; // Refresh image if parent is not the same step or remove template from result is false.

            if (canUseParentResult)
                screenshot = (Bitmap)Image.FromFile(parentLoopExecution.ResultImagePath);
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle);

            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(execution.FlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, screenshot, execution.FlowStep.RemoveTemplateFromResult);
                ImageSizeResult imageSizeResult = _systemService.GetImageSize(execution.FlowStep.TemplateImage);

                int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
                int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);
                bool isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;

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
            if (execution.FlowStep?.ParentTemplateSearchFlowStepId == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextChild(execution.FlowStep.ParentTemplateSearchFlowStepId.Value, execution.ExecutionResultEnum);
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
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.ParentTemplateSearchFlowStepId.Value);
            return nextFlowStep;
        }

        public async override Task SaveToDisk(Execution execution)
        {
            if (!execution.ParentExecutionId.HasValue || execution.ExecutionFolderDirectory.Length == 0)
                return;

            if (execution.StartedOn.HasValue)
            {
                string fileDate = execution.StartedOn.Value.ToString("yy-MM-dd hh.mm.ss.fff");
                string newFilePath = execution.ExecutionFolderDirectory + "\\" + fileDate + ".png";

                if (_resultImage != null)
                    await _systemService.SaveImageToDisk(newFilePath, _resultImage);

                execution.ResultImagePath = newFilePath;
                await _baseDatawork.SaveChangesAsync();
            }
        }

        private async Task<FlowStep?> GetChildTemplateSearchFlowStep(int flowStepId, int parentExecutionId)
        {
            // Get all parents of loop execution.
            List<Execution> parentLoopExecutions = await _baseDatawork.Executions.GetAllParentLoopExecutions(parentExecutionId);

            // Get all completed children template flow steps.
            List<int> completedChildrenTemplateFlowStepIds = parentLoopExecutions
                .Where(x => 
                    x.ExecutionResultEnum == ExecutionResultEnum.FAIL || 
                    (x?.FlowStep?.ParentTemplateSearchFlowStep?.MaxLoopCount > 0 && x.LoopCount >= x.FlowStep.ParentTemplateSearchFlowStep.MaxLoopCount))
                .Select(x => x.FlowStepId ?? 0)
                .Where(x => x != 0)
                .ToList();

            // Get all child template search flow steps.
            List<FlowStep> children = await _baseDatawork.Query.FlowSteps
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == flowStepId)
                .Where(x => x.Type == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD)
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