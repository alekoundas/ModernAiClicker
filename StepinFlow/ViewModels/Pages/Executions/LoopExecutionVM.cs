﻿using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class LoopExecutionVM : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        public LoopExecutionVM()
        {
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }
    }
}

