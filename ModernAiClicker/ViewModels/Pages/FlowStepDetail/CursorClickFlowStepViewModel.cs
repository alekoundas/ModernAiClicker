﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class CursorClickFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private FlowStep _flowStep;


        [ObservableProperty]
        private IEnumerable<MouseButtonsEnum> _mouseButtonsEnum;


        [ObservableProperty]
        private IEnumerable<MouseActionsEnum> _mouseActionsEnum;


        public CursorClickFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork) 
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;

            MouseButtonsEnum = Enum.GetValues(typeof(MouseButtonsEnum)).Cast<MouseButtonsEnum>();
            MouseActionsEnum = Enum.GetValues(typeof(MouseActionsEnum)).Cast<MouseActionsEnum>();
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

            }

            /// Add mode
            else
            {
                if (FlowStep.ParentFlowStepId != null)
                {
                    FlowStep isNewSimpling = _baseDatawork.FlowSteps
                        .Where(x => x.Id == FlowStep.ParentFlowStepId)
                        .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }
                else
                {
                    FlowStep isNewSimpling = _baseDatawork.Flows
                        .Where(x => x.Id == FlowStep.FlowId)
                        .Select(x => x.FlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }
                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Set cursor Action.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
                await _flowsViewModel.RefreshData();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }
    }
}
