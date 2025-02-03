using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using Business.Helpers;
using Model.Business;
using Model.Enums;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using Rectangle = Model.Structs.Rectangle;
namespace StepinFlow.ViewModels.Pages
{
    public partial class TemplateSearchLoopFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;
        private string _previousTestResultImagePath = "";

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private string _templateImgPath = "";

        [ObservableProperty]
        private FlowStep _flowStep;

        public event ShowTemplateImgEvent? ShowTemplateImg;
        public delegate void ShowTemplateImgEvent(string filePath);

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public TemplateSearchLoopFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;

            TemplateImgPath = flowStep.TemplateImagePath;
            ShowTemplateImg?.Invoke(TemplateImgPath);

        }

        [RelayCommand]
        private void OnButtonOpenFileClick()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = PathHelper.GetAppDataPath();
            openFileDialog.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                TemplateImgPath = openFileDialog.FileName;
                FlowStep.TemplateImage = File.ReadAllBytes(TemplateImgPath);
                ShowTemplateImg?.Invoke(openFileDialog.FileName);
            }

        }


        [RelayCommand]
        private void OnButtonClearTestClick()
        {
            _previousTestResultImagePath = "";
            ShowResultImage?.Invoke(_previousTestResultImagePath);
        }

        [RelayCommand]
        private void OnButtonTestClick()
        {
            if (FlowStep.TemplateImage == null)
                return;

            // Find search area.
            Rectangle searchRectangle;
            if (FlowStep.ProcessName.Length > 0 && TemplateImgPath != null)
                searchRectangle = _systemService.GetWindowSize(FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            // New if not previous exists.
            // Get previous one if exists.
            Bitmap? screenshot = null;
            if (_previousTestResultImagePath.Length > 0)
                screenshot = (Bitmap)Image.FromFile(_previousTestResultImagePath);
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle);

            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(FlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(templateImage, screenshot, FlowStep.RemoveTemplateFromResult);

                int x = searchRectangle.Left + result.ResultRectangle.Top;
                int y = searchRectangle.Top + result.ResultRectangle.Left;

                if (result.ResultImagePath.Length > 0)
                {
                    _previousTestResultImagePath = result.ResultImagePath;
                    ShowResultImage?.Invoke(result.ResultImagePath);
                }

            }
        }

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            // Edit mode
            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FindAsync(FlowStep.Id);
                updateFlowStep.Accuracy = FlowStep.Accuracy;
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.ProcessName = FlowStep.ProcessName;
                updateFlowStep.TemplateImagePath = FlowStep.TemplateImagePath;
                updateFlowStep.MaxLoopCount= FlowStep.MaxLoopCount;
            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else
                    isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(FlowStep.FlowId.Value);

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();


                // "Add" Flow steps
                FlowStep newFlowStep = new FlowStep();
                FlowStep newFlowStep2 = new FlowStep();
                newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;
                newFlowStep2.FlowStepType = FlowStepTypesEnum.IS_NEW;

                // "Success" Flow step
                FlowStep successFlowStep = new FlowStep();
                successFlowStep.Name = "Success";
                successFlowStep.IsExpanded = false;
                successFlowStep.FlowStepType = FlowStepTypesEnum.IS_SUCCESS;
                successFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    newFlowStep
                };

                // "Fail" Flow step
                FlowStep failFlowStep = new FlowStep();
                failFlowStep.Name = "Fail";
                failFlowStep.IsExpanded = false;
                failFlowStep.FlowStepType = FlowStepTypesEnum.IS_FAILURE;
                failFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    newFlowStep2
                };

                FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    successFlowStep,
                    failFlowStep
                };

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Template search loop.";

                FlowStep.IsExpanded = true;

                _baseDatawork.FlowSteps.Add(FlowStep);
            }



            await _baseDatawork.SaveChangesAsync();
            await _flowsViewModel.RefreshData();
        }
    }
}

