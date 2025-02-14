﻿using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class MultipleTemplateSearchFlowStepPage : Page, IPage
    {
        public IFlowStepViewModel ViewModel { get; set; }
        public MultipleTemplateSearchFlowStepPage(MultipleTemplateSearchFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
