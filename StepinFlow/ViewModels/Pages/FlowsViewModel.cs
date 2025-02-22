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
using Model.Enums;

namespace StepinFlow.ViewModels.Pages
{
    public partial class FlowsViewModel : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public event IsLockedChangedEvent? IsLockedChanged;
        public delegate void IsLockedChangedEvent(bool isLocked);

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

        public event NavigateToNewFlowParameterEvent? NavigateToNewFlowParameter;
        public delegate void NavigateToNewFlowParameterEvent(FlowParameter flowParameter);

        public event NavigateToFlowStepEvent? NavigateToFlowStep;
        public delegate Task NavigateToFlowStepEvent(int id);

        public event NavigateToFlowEvent? NavigateToFlow;
        public delegate Task NavigateToFlowEvent(int id);

        public event NavigateToFlowParameterEvent? NavigateToFlowParameter;
        public delegate Task NavigateToFlowParameterEvent(int id);



        [ObservableProperty]
        private bool _isLocked = true;
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

            LoadFlows?.Invoke();
        }

        public void RefreshData()
        {
            LoadFlows?.Invoke();
        }

        public void OnAddFlowStepClick(FlowStep newFlowStep)
        {
            NavigateToNewFlowStep?.Invoke(newFlowStep);
        }
        public void OnAddFlowParameterClick(FlowParameter newFlowParameter)
        {
            NavigateToNewFlowParameter?.Invoke(newFlowParameter);
        }

        public async Task OnTreeViewItemFlowStepSelected(int id)
        {
            await NavigateToFlowStep?.Invoke(id);
        }

        public async Task OnTreeViewItemFlowSelected(int id)
        {
            await NavigateToFlow?.Invoke(id);
        }
        public async Task OnTreeViewItemFlowParameterSelected(int id)
        {
            await NavigateToFlowParameter?.Invoke(id);
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
            ClearCopy?.Invoke();
        }

        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            FlowParameter flowRarameter = new FlowParameter
            {
                Name = "Flow parameters.",
                Type = FlowParameterTypesEnum.FLOW_PARAMETERS,
                ChildrenFlowParameters = new ObservableCollection<FlowParameter>() { new FlowParameter { Type = FlowParameterTypesEnum.NEW } }
            };

            FlowStep flowSteps = new FlowStep
            {
                Name = "Flow steps.",
                Type = FlowStepTypesEnum.FLOW_STEPS,
                ChildrenFlowSteps = new ObservableCollection<FlowStep>() { new FlowStep { Type = FlowStepTypesEnum.NEW } }
            };

            Flow flow = new Flow
            {
                Name = "Flow",
                IsSelected = true,
                FlowStep = flowSteps,
                FlowParameter = flowRarameter,
            };

            _baseDatawork.Flows.Add(flow);
            await _baseDatawork.SaveChangesAsync();

            flow.FlowStepId = flowSteps.Id;
            flow.FlowParameterId = flowRarameter.Id;
            await _baseDatawork.SaveChangesAsync();

            LoadFlows?.Invoke();
        }


        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;

            if (IsLocked)
                VisibleAddFlow = Visibility.Collapsed;
            else
                VisibleAddFlow = Visibility.Visible;

            IsLockedChanged?.Invoke(IsLocked);
        }

        [RelayCommand]
        private async Task OnButtonSyncClick()
        {
            await LoadFlows?.Invoke();
        }

        [RelayCommand]
        private async Task OnButtonExpandAllClick()
        {
            await ExpandAll?.Invoke();
        }

        [RelayCommand]
        private async Task OnButtonCollapseAllClick()
        {
            await CollapseAll?.Invoke();
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
