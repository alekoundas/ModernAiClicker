﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using DataAccess.Repository.Interface;
using Model.Enums;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class CursorScrollFlowStepVM : BaseFlowStepDetailVM
    {
        private readonly IDataService _dataService;
        public override event Action<int> OnSave;


        [ObservableProperty]
        private IEnumerable<MouseScrollDirectionEnum> _mouseScrollDirectionEnum;


        public CursorScrollFlowStepVM(IDataService dataService) : base(dataService)
        {
            _dataService = dataService;

            MouseScrollDirectionEnum = Enum.GetValues(typeof(MouseScrollDirectionEnum)).Cast<MouseScrollDirectionEnum>();
        }


        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            _dataService.Query.ChangeTracker.Clear();
            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _dataService.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.CursorScrollDirection = FlowStep.CursorScrollDirection;
                updateFlowStep.LoopCount = FlowStep.LoopCount;

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
                await _dataService.SaveChangesAsync();


                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Set cursor Action.";

                _dataService.FlowSteps.Add(FlowStep);
            }


            _dataService.SaveChanges();
            OnSave?.Invoke(FlowStep.Id);
        }
    }
}
