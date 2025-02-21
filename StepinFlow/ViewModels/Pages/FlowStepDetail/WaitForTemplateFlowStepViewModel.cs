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
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class WaitForTemplateFlowStepViewModel : BaseFlowStepDetailVM
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public WaitForTemplateFlowStepViewModel(FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork) : base(baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _flowsViewModel = flowsViewModel;


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
                FlowStep.TemplateImage = File.ReadAllBytes(openFileDialog.FileName);
            }

        }


        [RelayCommand]
        private void OnButtonTestClick()
        {
            // Find search area.
            Model.Structs.Rectangle searchRectangle;
            if (FlowStep.ProcessName.Length > 0 )
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

                x += result.ResultRectangle.Top;
                y += result.ResultRectangle.Left;


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

            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Accuracy = FlowStep.Accuracy;
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.ProcessName = FlowStep.ProcessName;
                updateFlowStep.WaitForHours = FlowStep.WaitForHours;
                updateFlowStep.WaitForMinutes = FlowStep.WaitForMinutes;
                updateFlowStep.WaitForSeconds = FlowStep.WaitForSeconds;
                updateFlowStep.WaitForMilliseconds = FlowStep.WaitForMilliseconds;
            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else if (FlowStep.FlowId.HasValue)
                    isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(FlowStep.FlowId.Value);
                else
                    return;

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();



                // "Add" Flow steps
                FlowStep newFlowStep = new FlowStep();
                newFlowStep.Type = FlowStepTypesEnum.NEW;

                // "Success" Flow step
                FlowStep successFlowStep = new FlowStep();
                successFlowStep.Name = "Success";
                successFlowStep.IsExpanded = false;
                successFlowStep.Type = FlowStepTypesEnum.SUCCESS;
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
                _flowsViewModel.RefreshData();
            }
        }
    }
}

