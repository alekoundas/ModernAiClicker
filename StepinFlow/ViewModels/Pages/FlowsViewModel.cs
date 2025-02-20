using Model.Models;
using CommunityToolkit.Mvvm.Input;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;
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

        public event LoadFlowsEvent? LoadFlows;
        public delegate Task LoadFlowsEvent(int? id = 0);

        public event ClearCopyEvent? ClearCopy;
        public delegate void ClearCopyEvent();

        public event AddNewFlowEvent? AddNewFlow;
        public delegate Task AddNewFlowEvent();

        public event ExpandAllEvent? ExpandAll;
        public delegate Task ExpandAllEvent();

        public event CollapseAllEvent? CollapseAll;
        public delegate Task CollapseAllEvent();

        public event NavigateToNewFlowStepEvent? NavigateToNewFlowStep;
        public delegate void NavigateToNewFlowStepEvent(FlowStep flowStep);

        public event NavigateToFlowStepEvent? NavigateToFlowStep;
        public delegate Task NavigateToFlowStepEvent(int id);

        public event NavigateToFlowEvent? NavigateToFlow;
        public delegate Task NavigateToFlowEvent(int id);



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
            ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            //Task.Run(async () => await RefreshData());
        }

        public void RefreshData()
        {
            LoadFlows?.Invoke();
            //await _treeViewUserControlViewModel.LoadFlows();
        }

        public void OnAddFlowStepClick(FlowStep newFlowStep)
        {
            //_flowStepFrameUserControlViewModel.NavigateToNewFlowStep(newFlowStep);
            NavigateToNewFlowStep?.Invoke(newFlowStep);
        }

        public async Task OnTreeViewItemFlowStepSelected(int id)
        {
            //await _flowStepFrameUserControlViewModel.NavigateToFlowStep(id);
            await NavigateToFlowStep?.Invoke(id);
        }

        public async Task OnTreeViewItemFlowSelected(int id)
        {
            //await _flowStepFrameUserControlViewModel.NavigateToFlow(id);
            await NavigateToFlow?.Invoke(id);
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
            //_treeViewUserControlViewModel.ClearCopy();
            ClearCopy?.Invoke();
        }

        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            await AddNewFlow?.Invoke();
        }

        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;

            if (IsLocked)
                VisibleAddFlow = Visibility.Collapsed;
            else
                VisibleAddFlow = Visibility.Visible;
        }

        [RelayCommand]
        private async Task OnButtonSyncClick()
        {
            //await _treeViewUserControlViewModel.LoadFlows();
            await LoadFlows?.Invoke();
        }

        [RelayCommand]
        private async Task OnButtonExpandAllClick()
        {
            //await _treeViewUserControlViewModel.ExpandAll();
            await ExpandAll?.Invoke();
        }

        [RelayCommand]
        private async Task OnButtonCollapseAllClick()
        {
            //await _treeViewUserControlViewModel.CollapseAll();
            await CollapseAll?.Invoke();
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
