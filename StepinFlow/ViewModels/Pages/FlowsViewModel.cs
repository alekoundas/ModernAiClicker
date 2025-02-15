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
using System.Windows;

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
        private Visibility _visibleAddFlow = Visibility.Collapsed;

        [ObservableProperty]
        private int? _coppiedFlowStepId = null;
        [ObservableProperty]
        private int? _coppiedFlowId = null;
        [ObservableProperty]
        private string? _coppiedDisplayText = "";
        [ObservableProperty]
        private Visibility _visible = Visibility.Collapsed;

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

        public async Task OnTreeViewItemFlowStepSelected(int id)
        {
            await _flowStepFrameUserControlViewModel.NavigateToFlowStep(id);
        }

        public async Task OnTreeViewItemFlowSelected(int id)
        {
            await _flowStepFrameUserControlViewModel.NavigateToFlow(id);
        }

        public void OnFlowStepCopy(int id)
        {
            CoppiedFlowStepId = id;
            CoppiedDisplayText = "Coppied FlowStep ID: ";
            Visible = Visibility.Visible;
        }


        [RelayCommand]
        private void OnButtonClearCopyClick()
        {
            CoppiedFlowStepId = null;
            CoppiedFlowId = null;
            Visible = Visibility.Collapsed;
            _treeViewUserControlViewModel.ClearCopy();
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
            if (IsLocked)
                VisibleAddFlow = Visibility.Collapsed;
            else
                VisibleAddFlow = Visibility.Visible;
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
