﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using Model.Structs;
using Business.Helpers;
using DataAccess.Repository.Interface;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class WindowResizeFlowStepViewModel : BaseFlowStepDetailVM
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        public WindowResizeFlowStepViewModel(FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork) : base(baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowsViewModel = flowsViewModel;

        }


        [RelayCommand]
        private void OnButtonRecordClick()
        {
            if (FlowStep.ProcessName.Length <= 1)
                return;

            Rectangle windowRect = _systemService.GetWindowSize(FlowStep.ProcessName);

            FlowStep.Height = Math.Abs(windowRect.Bottom - windowRect.Top);
            FlowStep.Width = Math.Abs(windowRect.Left - windowRect.Right);
        }

        [RelayCommand]
        private void OnButtonTestClick()
        {
            if (FlowStep.ProcessName.Length <= 1)
                return;

            Rectangle windowRect = _systemService.GetWindowSize(FlowStep.ProcessName);
            Rectangle newWindowRect = new Rectangle();

            newWindowRect.Left = windowRect.Left;
            newWindowRect.Top = windowRect.Top;
            newWindowRect.Right = windowRect.Left + FlowStep.Width;
            newWindowRect.Bottom = windowRect.Top + FlowStep.Height;

            _systemService.MoveWindow(FlowStep.ProcessName, newWindowRect);
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
                updateFlowStep.Height = FlowStep.Height;
                updateFlowStep.Width = FlowStep.Width;
                updateFlowStep.ProcessName = FlowStep.ProcessName;
                updateFlowStep.Type = FlowStep.Type;
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


                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Set window size.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            _flowsViewModel.RefreshData();
        }
    }
}
