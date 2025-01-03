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
namespace ModernAiClicker.ViewModels.Pages
{
    public partial class LoopFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
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

        public LoopFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;

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
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FindAsync(FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
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
                    FlowStep.Name = "Loop.";

                FlowStep.IsExpanded = true;

                _baseDatawork.FlowSteps.Add(FlowStep);
            }



            await _baseDatawork.SaveChangesAsync();
            await _flowsViewModel.RefreshData();
        }
    }
}

