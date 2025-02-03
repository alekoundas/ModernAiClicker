﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using Business.Helpers;
using Model.Enums;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
namespace StepinFlow.ViewModels.Pages
{
    public partial class LoopFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
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

        //public event ShowResultImageEvent? ShowResultImage;
        //public delegate void ShowResultImageEvent(string filePath);

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
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.MaxLoopCount = FlowStep.MaxLoopCount;
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

                // "Add" Flow step
                FlowStep newFlowStep = new FlowStep();
                newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;

                FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep> { newFlowStep };

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Loop";

                FlowStep.IsExpanded = true;

                _baseDatawork.FlowSteps.Add(FlowStep);
            }



            await _baseDatawork.SaveChangesAsync();
            await _flowsViewModel.RefreshData();
        }
    }
}

