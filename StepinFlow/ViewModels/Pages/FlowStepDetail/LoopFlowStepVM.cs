﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Helpers;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.BaseViewModels;
using Business.Services.Interfaces;
namespace StepinFlow.ViewModels.Pages
{
    public partial class LoopFlowStepVM : BaseFlowStepDetailVM
    {
        private readonly IDataService _dataService;
        //public override event Action<int> OnSave;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private string _templateImgPath = "";


        public LoopFlowStepVM(IDataService dataService) : base(dataService)
        {
            _dataService = dataService;
        }

        public override async Task OnSave()
        {
            _dataService.Query.ChangeTracker.Clear();
            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _dataService.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.LoopMaxCount = FlowStep.LoopMaxCount;
            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _dataService.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else if (FlowStep.FlowId.HasValue)
                    isNewSimpling = await _dataService.Flows.GetIsNewSibling(FlowStep.FlowId.Value);
                else
                    return;

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;

                // "Add" Flow step
                FlowStep newFlowStep = new FlowStep();
                newFlowStep.Type = FlowStepTypesEnum.NEW;

                FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep> { newFlowStep };

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Loop";

                FlowStep.IsExpanded = true;

                _dataService.FlowSteps.Add(FlowStep);
            }



            await _dataService.SaveChangesAsync();
            //OnSave?.Invoke(FlowStep.Id);
        }
    }
}

