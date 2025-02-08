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
using Microsoft.EntityFrameworkCore;
using Rectangle = Model.Structs.Rectangle;

namespace StepinFlow.ViewModels.Pages
{
    public partial class MultipleTemplateSearchLoopFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private ObservableCollection<FlowStep> _childrenTemplateSearchFlowSteps;
        private readonly List<FlowStep> _childrenTemplateSearchFlowStepsToRemove = new List<FlowStep>();

        [ObservableProperty]
        private string _templateImgPath = "";

        [ObservableProperty]
        private FlowStep _flowStep;

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public MultipleTemplateSearchLoopFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;

            List<FlowStep> flowSteps = flowStep.ChildrenTemplateSearchFlowSteps.Where(x => x.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD).ToList();
            _childrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
            TemplateImgPath = flowStep.TemplateImagePath;

        }

        [RelayCommand]
        private void OnButtonOpenFileClick(FlowStep flowStep)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = PathHelper.GetAppDataPath();
            openFileDialog.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                TemplateImgPath = openFileDialog.FileName;
                flowStep.TemplateImage = File.ReadAllBytes(TemplateImgPath);
            }
        }

        [RelayCommand]
        private void OnButtonUpClick(FlowStep flowStep)
        {
            List<FlowStep> simplingsAbove = ChildrenTemplateSearchFlowSteps
                    .Where(x => x.OrderingNum < flowStep.OrderingNum)
                    .ToList();

            flowStep.OrderingNum++;
            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(ChildrenTemplateSearchFlowSteps.OrderBy(x => x.OrderingNum).ToList());

        }

        [RelayCommand]
        private void OnButtonDownClick(FlowStep flowStep)
        {
            List<FlowStep> simplingsBellow = ChildrenTemplateSearchFlowSteps
                    .Where(x => x.OrderingNum > flowStep.OrderingNum)
                    .ToList();

            flowStep.OrderingNum--;
            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(ChildrenTemplateSearchFlowSteps.OrderBy(x => x.OrderingNum).ToList());
        }

        [RelayCommand]
        public void OnButtonAddClick()
        {
            FlowStep newFlowStep = new FlowStep();

            // In edit mode set parent ID.
            if (FlowStep.Id > 0)
                newFlowStep.ParentTemplateSearchFlowStepId = FlowStep.Id;

            ChildrenTemplateSearchFlowSteps.Add(newFlowStep);
        }


        [RelayCommand]
        private void OnButtonDeleteClick(FlowStep flowStep)
        {
            if (flowStep.Id > 0)
                _childrenTemplateSearchFlowStepsToRemove.Add(flowStep);

            ChildrenTemplateSearchFlowSteps.Remove(flowStep);
        }


        [RelayCommand]
        private void OnButtonClearTestClick(FlowStep flowStep)
        {
            flowStep.LoopResultImagePath = "";
            ShowResultImage?.Invoke("");
        }

        [RelayCommand]
        private void OnButtonTestClick(FlowStep flowStep)
        {
            if (flowStep.TemplateImage == null)
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
            Bitmap? screenshot;
            if (flowStep.LoopResultImagePath.Length > 0)
                screenshot = (Bitmap)Image.FromFile(flowStep.LoopResultImagePath);
            else
                screenshot = _systemService.TakeScreenShot(searchRectangle);

            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(flowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(templateImage, screenshot, flowStep.RemoveTemplateFromResult);

                int x = searchRectangle.Left + result.ResultRectangle.Top;
                int y = searchRectangle.Top + result.ResultRectangle.Left;

                if (result.ResultImagePath.Length > 0)
                {
                    flowStep.LoopResultImagePath = result.ResultImagePath;
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
            // Remove flow steps that dont contain a template image.
            List<FlowStep> templateFlowSteps = ChildrenTemplateSearchFlowSteps
                .Where(x => x.TemplateImage == null)
                .ToList();

            foreach (var templateFlowStep in templateFlowSteps)
                ChildrenTemplateSearchFlowSteps.Remove(templateFlowStep);

            foreach (var templateFlowStep in ChildrenTemplateSearchFlowSteps)
                templateFlowStep.FlowStepType = FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD;


            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.ProcessName = FlowStep.ProcessName;

                _baseDatawork.UpdateRange(ChildrenTemplateSearchFlowSteps.Where(x => x.Id > 0).ToList());
                _baseDatawork.FlowSteps.AddRange(ChildrenTemplateSearchFlowSteps.Where(x => x.Id == 0).ToList());
                _baseDatawork.FlowSteps.RemoveRange(_childrenTemplateSearchFlowStepsToRemove);
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
                    FlowStep.Name = "Multiple template search loop.";

                FlowStep.IsExpanded = true;
                FlowStep.ChildrenTemplateSearchFlowSteps = ChildrenTemplateSearchFlowSteps;

                _baseDatawork.FlowSteps.Add(FlowStep);
            }



            await _baseDatawork.SaveChangesAsync();
            await _flowsViewModel.RefreshData();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.Query
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == FlowStep.Id)
                .Where(x => x.FlowStepType == FlowStepTypesEnum.NO_SELECTION)
                .ToListAsync();
            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
        }
    }
}

