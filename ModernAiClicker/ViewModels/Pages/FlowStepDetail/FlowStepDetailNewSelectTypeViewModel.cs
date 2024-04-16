using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using System.Collections.ObjectModel;
using DataAccess.Repository.Interface;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowStepDetailNewSelectTypeViewModel : ObservableObject
    {
        public event NavigateToFlowStepDetailPageEvent? NavigateToFlowStepDetailPage;
        public delegate void NavigateToFlowStepDetailPageEvent(FlowStep flowStep);

        private readonly IBaseDatawork _baseDatawork;



        [ObservableProperty]
        private FlowStep? _flowStep;

        private bool _isInitialized = false;


        public FlowStepDetailNewSelectTypeViewModel(IBaseDatawork baseDatawork)
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
