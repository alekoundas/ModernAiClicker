using Business.Extensions;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class MultipleTemplateSearchExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        private byte[]? _resultImage = null;

        public MultipleTemplateSearchExecutionWorker(
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

            // Get first child template search flow step that isnt completed.
            FlowStep? childTemplateSearchFlowStep = await GetChildTemplateSearchFlowStep(flowStep.Id, parentExecution.Id);

            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = parentExecution.Id;// TODO This is wrong!
            execution.ParentLoopExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;
            execution.LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1;
            execution.CurrentMultipleTemplateSearchFlowStepId = childTemplateSearchFlowStep?.Id;


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

            FlowStep? currentMultipleTemplateSearchFlowStep = await _baseDatawork.FlowSteps.Query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == execution.CurrentMultipleTemplateSearchFlowStepId);

            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (execution.FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            // New if not previous exists.
            // Get previous one if exists.
            Execution? parentLoopExecution = await _baseDatawork.Executions.FirstOrDefaultAsync(x => x.Id == execution.ParentLoopExecutionId);
            Bitmap? screenshot = null;
            if (parentLoopExecution.ResultImagePath?.Length > 0 && currentMultipleTemplateSearchFlowStep.RemoveTemplateFromResult)
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
            if (execution.FlowStepId == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextChild(execution.FlowStepId.Value, execution.ExecutionResultEnum);
            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // Get next child TemplateSearchFlowStep.
            FlowStep? nextChildTemplateSearchFlowStep = await GetChildTemplateSearchFlowStep(execution.FlowStep.Id, execution.Id);

            if (nextChildTemplateSearchFlowStep != null)
                return execution.FlowStep;

            // If not, get next sibling flow step. 
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.Id);
            return nextFlowStep;
        }

        public async override Task SaveToDisk(Execution execution)
        {
            if (!execution.ParentExecutionId.HasValue   || execution.ExecutionFolderDirectory.Length == 0)
                return;

            string fileDate = execution.StartedOn.Value.ToString("yy-MM-dd hh.mm.ss.fff");
            string newFilePath = execution.ExecutionFolderDirectory + "\\" + fileDate + ".png";

            //_systemService.CopyImageToDisk(execution.ResultImagePath, newFilePath);_resultImage
            if (_resultImage != null)
                await _systemService.SaveImageToDisk(newFilePath, _resultImage);
            execution.ResultImagePath = newFilePath;
                await _baseDatawork.SaveChangesAsync();
        }

        private async Task<FlowStep?> GetChildTemplateSearchFlowStep(int flowStepId, int parentExecutionId)
        {
            // Get all parents of loop execution.
            List<Execution> parentLoopExecutions = await _baseDatawork.Executions.GetAllParentLoopExecutions(parentExecutionId);

            // Get all completed children template flow steps.
            List<int> completedChildrenTemplateFlowStepIds = parentLoopExecutions
                .Select(x => x.CurrentMultipleTemplateSearchFlowStepId ?? 0)
                .Where(x => x != 0)
                .ToList();

            // Get all child template search flow steps.
            List<FlowStep> children = await _baseDatawork.Query.FlowSteps
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == flowStepId)
                .Where(x => x.FlowStepType == FlowStepTypesEnum.NO_SELECTION)
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