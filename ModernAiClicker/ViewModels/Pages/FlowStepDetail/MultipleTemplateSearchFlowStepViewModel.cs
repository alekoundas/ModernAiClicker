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
using Model.ConverterModels;
using Microsoft.EntityFrameworkCore;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class MultipleTemplateSearchFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private ObservableCollection<FlowStep> _childrenTemplateSearchFlowSteps;
        private List<FlowStep> _childrenTemplateSearchFlowStepsToRemove=new List<FlowStep>();

        [ObservableProperty]
        private string _templateImgPath = "";

        [ObservableProperty]
        private FlowStep _flowStep;

        public event ShowTemplateImgEvent? ShowTemplateImg;
        public delegate void ShowTemplateImgEvent(string filePath);

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public MultipleTemplateSearchFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;

            TemplateImgPath = flowStep.TemplateImagePath;
            List<FlowStep> flowSteps = flowStep.ChildrenTemplateSearchFlowSteps.Where(x => x.FlowStepType == FlowStepTypesEnum.NO_SELECTION).ToList();
            _childrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
            //ShowTemplateImg?.Invoke(TemplateImgPath);

        }

        [RelayCommand]
        private void OnButtonOpenFileClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;

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
        }


        [RelayCommand]
        private void OnButtonUpClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplingsAbove = ChildrenTemplateSearchFlowSteps
                        .Where(x => x.OrderingNum < flowStep.OrderingNum)
                        .ToList();

                flowStep.OrderingNum++;

                ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(ChildrenTemplateSearchFlowSteps.OrderBy(x => x.OrderingNum).ToList());

            }
        }

        [RelayCommand]
        private void OnButtonDownClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplingsBellow = ChildrenTemplateSearchFlowSteps
                        .Where(x => x.OrderingNum > flowStep.OrderingNum)
                        .ToList();

                flowStep.OrderingNum--;

                ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(ChildrenTemplateSearchFlowSteps.OrderBy(x => x.OrderingNum).ToList());
            }
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
        private void OnButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep templateFlowStep = (FlowStep)eventParameters.FlowId;

                if (templateFlowStep.Id > 0)
                    _childrenTemplateSearchFlowStepsToRemove.Add(templateFlowStep);

                ChildrenTemplateSearchFlowSteps.Remove(templateFlowStep);
            }
        }

        [RelayCommand]
        private void OnButtonClearTestClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep templateFlowStep = (FlowStep)eventParameters.FlowId;
                templateFlowStep.LoopResultImagePath = "";
            }
            ShowResultImage?.Invoke("");
        }

        [RelayCommand]
        private void OnButtonTestClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep templateFlowStep = (FlowStep)eventParameters.FlowId;
                if (templateFlowStep.TemplateImage == null)
                    return;

                // Find search area.
                Rectangle searchRectangle;
                if (FlowStep.ProcessName.Length > 0 && TemplateImgPath != null)
                    searchRectangle = _systemService.GetWindowSize(FlowStep.ProcessName);
                else
                    searchRectangle = _systemService.GetScreenSize();

                // Get screenshot.
                Bitmap? screenshot = _systemService.TakeScreenShot(searchRectangle);
                if (screenshot == null)
                    return;

                using (var ms = new MemoryStream(templateFlowStep.TemplateImage))
                {
                    Bitmap templateImage = new Bitmap(ms);
                    TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(templateImage, screenshot, templateFlowStep.RemoveTemplateFromResult);

                    int x = searchRectangle.Left + result.ResultRectangle.Top;
                    int y = searchRectangle.Top + result.ResultRectangle.Left;

                    if (result.ResultImagePath.Length > 0)
                    {
                        templateFlowStep.LoopResultImagePath = result.ResultImagePath;
                        ShowResultImage?.Invoke(result.ResultImagePath);
                    }

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


            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FindAsync(FlowStep.Id);
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

