﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Business.Extensions;
using System.Collections.ObjectModel;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class NewSelectTypeFlowStepViewModel : ObservableObject
    {
        public event NavigateToFlowStepDetailPageEvent? NavigateToFlowStepDetailPage;
        public delegate void NavigateToFlowStepDetailPageEvent(FlowStep flowStep);

        private readonly IBaseDatawork _baseDatawork;


        [ObservableProperty]
        private FlowStep? _flowStep;


        public NewSelectTypeFlowStepViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }
       

        [RelayCommand]
        private void Combobox_OnDropDownClosed()
        {
            if (FlowStep != null && NavigateToFlowStepDetailPage != null)
                NavigateToFlowStepDetailPage.Invoke(FlowStep);
        }
    }
}
