using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;

namespace Business.Factories.Workers
{
    public class TemplateSearchExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        private byte[]? _resultImage = null;

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
            if (execution.FlowStep == null || execution.FlowStep.TemplateMatchMode == null)
                return;

            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (execution.FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            Bitmap? screenshot = _systemService.TakeScreenShot(searchRectangle);
            if (screenshot == null)
                return;

            ImageSizeResult imageSizeResult = _systemService.GetImageSize(execution.FlowStep.TemplateImage);
            using (var ms = new MemoryStream(execution.FlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateSearchService.SearchForTemplate(templateImage, screenshot, execution.FlowStep.TemplateMatchMode.Value, false);

                int x = searchRectangle.Left + result.ResultRectangle.Left + (imageSizeResult.Width / 2);
                int y = searchRectangle.Top + result.ResultRectangle.Top + (imageSizeResult.Height / 2);

                bool isSuccessful = execution.FlowStep.Accuracy <= result.Confidence;
                execution.ExecutionResultEnum = isSuccessful ? ExecutionResultEnum.SUCCESS : ExecutionResultEnum.FAIL;

                execution.ResultLocationX = x;
                execution.ResultLocationY = y;
                //execution.ResultImage = result.ResultImage;
                //execution.ResultImagePath = result.ResultImagePath;
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

            // Get next sibling flow step. 
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.Id);
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