﻿using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class WaitForTemplateExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public WaitForTemplateExecutionViewModel()
        {
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;

            if (execution.ResultImagePath != null)
                ShowResultImage?.Invoke(execution.ResultImagePath);
        }
    }
}
