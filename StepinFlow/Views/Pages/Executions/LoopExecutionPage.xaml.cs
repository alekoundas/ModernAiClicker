﻿using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class LoopExecutionPage : Page, IExecutionPage
    {
        public LoopExecutionViewModel ViewModel { get; set; }

        public LoopExecutionPage()
        {
            ViewModel = new LoopExecutionViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (LoopExecutionViewModel)executionViewModel;
            DataContext = this;
        }
    }
}
