using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;

namespace Business.Factories.Workers
{
    public class WaitForTemplateExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        private byte[]? _resultImage = null;

        public WaitForTemplateExecutionWorker(
              IBaseDatawork baseDatawork
            , ISystemService systemService
            , ITemplateSearchService templateSearchService
            ) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _templateSearchService = templateSearchService;
            _systemService = systemService;
        }

        public async override Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution parentExecution, Execution latestParentExecution)
        {
            if (parentExecution == null)
                throw new ArgumentNullException(nameof(parentExecution));

            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = latestParentExecution.Id;
            execution.ParentLoopExecutionId = parentExecution.Id;

            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;
            execution.LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1;


            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            execution.FlowStep = flowStep;
            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null || execution.FlowStep.TemplateImage == null)
                return;

            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (execution.FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            bool isSuccessful = false;
            while (!isSuccessful)
            {
                ImageSizeResult imageSizeResult = _systemService.GetImageSize(execution.FlowStep.TemplateImage);
                using (var ms = new MemoryStream(execution.FlowStep.TemplateImage))
                {
                    // Get screenshot.
                    Bitmap? screenshot = _systemService.TakeScreenShot(searchRectangle);
                    if (screenshot == null)
                        return;

                    Bitmap templateImage = new Bitmap(ms);
                    TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, screenshot, execution.FlowStep.RemoveTemplateFromResult);

                    int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
                    int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);

                    isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;
                    execution.ExecutionResultEnum = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;
                    execution.ResultLocationX = x;
                    execution.ResultLocationY = y;
                    execution.ResultImagePath = result.ResultImagePath;
                    execution.ResultAccuracy = result.Confidence;

                    await _baseDatawork.SaveChangesAsync();
                    _resultImage = result.ResultImage;


                    int miliseconds = 0;

                    miliseconds += execution.FlowStep.SleepForMilliseconds;
                    miliseconds += execution.FlowStep.SleepForSeconds * 1000;
                    miliseconds += execution.FlowStep.SleepForMinutes * 60 * 1000;
                    miliseconds += execution.FlowStep.SleepForHours * 60 * 60 * 1000;

                    Thread.Sleep(miliseconds);
                }
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

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.Id);
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

                //_systemService.CopyImageToDisk(execution.ResultImagePath, newFilePath);_resultImage
                if (_resultImage != null)
                    await _systemService.SaveImageToDisk(newFilePath, _resultImage);
                execution.ResultImagePath = newFilePath;
                await _baseDatawork.SaveChangesAsync();
            }
        }
    }
}