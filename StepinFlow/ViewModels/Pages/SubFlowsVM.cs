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
using Wpf.Ui.Abstractions.Controls;

namespace StepinFlow.ViewModels.Pages
{
    public partial class SubFlowsVM : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        private readonly ISystemService _systemService;

        public event IsLockedChangedEvent? IsLockedChanged;
        public delegate void IsLockedChangedEvent(bool isLocked);

        public event LoadFlowsAndSelectFlowEvent? LoadFlowsAndSelectFlow;
        public delegate Task LoadFlowsAndSelectFlowEvent(int id);
        public event LoadFlowsAndSelectFlowStepEvent? LoadFlowsAndSelectFlowStep;
        public delegate Task LoadFlowsAndSelectFlowStepEvent(int id);
        public event LoadFlowsAndSelectFlowParameterEvent? LoadFlowsAndSelectFlowParameter;
        public delegate Task LoadFlowsAndSelectFlowParameterEvent(int id);

        public event LoadFlowsEvent? LoadFlows;
        public delegate Task LoadFlowsEvent(int flowId = 0, bool isSubFlow = false);

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

        public SubFlowsVM(
            IDataService dataService,
            ISystemService systemService)
        {
            _dataService = dataService;
            _systemService = systemService;


        }

        public void RefreshData()
        {
            LoadFlows?.Invoke(-1, true);
        }
        public void OnSaveFlow(int id)
        {
            LoadFlowsAndSelectFlow?.Invoke(id);
        }
        public void OnSaveFlowStep(int id)
        {
            LoadFlowsAndSelectFlowStep?.Invoke(id);
        }
        public void OnSaveFlowParameter(int id)
        {
            LoadFlowsAndSelectFlowParameter?.Invoke(id);
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
                Name = "Sub-Flow",
                IsSelected = true,
                FlowStep = flowSteps,
                FlowParameter = flowRarameter,
                Type = FlowTypesEnum.SUB_FLOW
            };

            _dataService.Flows.Add(flow);
            await _dataService.SaveChangesAsync();

            flow.FlowStepId = flowSteps.Id;
            flow.FlowParameterId = flowRarameter.Id;
            await _dataService.SaveChangesAsync();

            LoadFlows?.Invoke(-1, true);
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
            await LoadFlows?.Invoke(-1, true);
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



        //public void OnNavigatedToAsync() { LoadFlows?.Invoke(-1, true); }

        //public void OnNavigatedFrom() { }

        public Task OnNavigatedToAsync()
        {
            LoadFlows?.Invoke(-1, true);
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync()
        {
            return Task.CompletedTask;
        }
    }
}
