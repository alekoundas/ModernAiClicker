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

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class WaitForTemplateFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

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

        public WaitForTemplateFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
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
        private void OnButtonTestClick()
        {
            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (FlowStep.ProcessName.Length > 0 && TemplateImgPath != null)
                searchRectangle = _systemService.GetWindowSize(FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            Bitmap? screenshot = _systemService.TakeScreenShot(searchRectangle);
            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(FlowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);

                TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(templateImage, screenshot, false);


                int x = searchRectangle.Left;
                int y = searchRectangle.Top;

                x = x + result.ResultRectangle.Top;
                y = y + result.ResultRectangle.Left;


                if (result.ResultImagePath.Length > 1)
                    ShowResultImage?.Invoke(result.ResultImagePath);
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
            FlowStep.TemplateImagePath = TemplateImgPath;

            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FindAsync(FlowStep.Id);
                updateFlowStep.Accuracy = FlowStep.Accuracy;
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.ProcessName = FlowStep.ProcessName;
                updateFlowStep.TemplateImagePath = FlowStep.TemplateImagePath;
                updateFlowStep.SleepForHours = FlowStep.SleepForHours;
                updateFlowStep.SleepForMinutes = FlowStep.SleepForMinutes;
                updateFlowStep.SleepForSeconds = FlowStep.SleepForSeconds;
                updateFlowStep.SleepForMilliseconds = FlowStep.SleepForMilliseconds;
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
                newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;

                // "Success" Flow step
                FlowStep successFlowStep = new FlowStep();
                successFlowStep.Name = "Success";
                successFlowStep.IsExpanded = false;
                successFlowStep.FlowStepType = FlowStepTypesEnum.IS_SUCCESS;
                successFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    newFlowStep
                };

                FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    successFlowStep,
                };

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Wait for Template.";

                FlowStep.IsExpanded = true;

                _baseDatawork.FlowSteps.Add(FlowStep);

                await _baseDatawork.SaveChangesAsync();
                await _flowsViewModel.RefreshData();
            }
        }
    }
}

