﻿using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using Business.Helpers;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class WindowResizeExecutionVM : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        public WindowResizeExecutionVM()
        {
            _execution = new Execution() { FlowStep = new FlowStep() };
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }
    }
}
