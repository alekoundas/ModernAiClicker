using Model.Models;
using CommunityToolkit.Mvvm.Input;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using StepinFlow.ViewModels.UserControls;

namespace StepinFlow.ViewModels.Pages
{
    public partial class FlowsViewModel : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly TreeViewUserControlViewModel _treeViewUserControlViewModel;
        private readonly FlowStepFrameUserControlViewModel _flowStepFrameUserControlViewModel;

        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        public FlowsViewModel(
            IBaseDatawork baseDatawork,
            ISystemService systemService,
            TreeViewUserControlViewModel treeViewUserControlViewModel,
            FlowStepFrameUserControlViewModel flowStepFrameUserControlViewModel)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _treeViewUserControlViewModel = treeViewUserControlViewModel;
            _flowStepFrameUserControlViewModel = flowStepFrameUserControlViewModel;

            Task.Run(async () => await RefreshData());
        }

        public async Task RefreshData()
        {
            await _treeViewUserControlViewModel.LoadFlows();
        }

        public void OnAddFlowStepClick(FlowStep newFlowStep)
        {
            _flowStepFrameUserControlViewModel.NavigateToNewFlowStep(newFlowStep);
        }

        public async Task OnTreeViewItemSelected(int id)
        {
            await _flowStepFrameUserControlViewModel.NavigateToFlowStep(id);
        }


        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            await _treeViewUserControlViewModel.AddNewFlow();
        }

        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;
            _treeViewUserControlViewModel.IsLocked = IsLocked;
        }

        [RelayCommand]
        private async Task OnButtonSyncClick()
        {
            await _treeViewUserControlViewModel.LoadFlows();
        }

        [RelayCommand]
        private async Task OnButtonExpandAllClick()
        {
            await _treeViewUserControlViewModel.ExpandAll();
        }

        [RelayCommand]
        private async Task OnButtonCollapseAllClick()
        {
            await _treeViewUserControlViewModel.CollapseAll();
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
