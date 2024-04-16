using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using System.Collections.ObjectModel;
using DataAccess.Repository.Interface;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FrameExecutionFlowViewModel : ObservableObject
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IFlowService _flowService;

        [ObservableProperty]
        private Flow? _flow;


        public FrameExecutionFlowViewModel(IBaseDatawork baseDatawork, IFlowService flowService)
        {
            _baseDatawork = baseDatawork;
            _flowService = flowService;
        }


        [RelayCommand]
        private void OnButtonStartClick()
        {
            if (Flow == null)
                return;

            _flowService.StartFlow(Flow);
        }

        [RelayCommand]
        private void OnButtonStopClick()
        {
        }

    }
}
