﻿using ModernAiClicker.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    
    public partial class CursorMoveFlowStepPage : INavigableView<CursorMoveFlowStepViewModel>
    {

        public CursorMoveFlowStepViewModel ViewModel { get; }

        public CursorMoveFlowStepPage(CursorMoveFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            DataContext = this;

        }

    }
}
