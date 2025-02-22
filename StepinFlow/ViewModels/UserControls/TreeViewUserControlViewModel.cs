using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using StepinFlow.Converters;
using System.Reflection.Metadata;

namespace StepinFlow.ViewModels.UserControls
{
    public partial class TreeViewUserControlViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;



        public event OnSelectedFlowIdChanged? OnSelectedFlowIdChangedEvent;
        public delegate void OnSelectedFlowIdChanged(int Id);

        public event OnSelectedFlowStepIdChanged? OnSelectedFlowStepIdChangedEvent;
        public delegate void OnSelectedFlowStepIdChanged(int Id);

        public event OnFlowStepClone? OnFlowStepCloneEvent;
        public delegate void OnFlowStepClone(int Id);

        public event OnAddFlowStepClick? OnAddFlowStepClickEvent;
        public delegate void OnAddFlowStepClick(FlowStep newFlowStep);

        public event OnAddFlowParameterClick? OnAddFlowParameterClickEvent;
        public delegate void OnAddFlowParameterClick(FlowParameter newFlowParameter);

        private Flow? _selectedFlow = null;
        private FlowStep? _selectedFlowStep = null;
        private FlowParameter? _selectedFlowParameter = null;


        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        [ObservableProperty]
        private Visibility? _pasteVisibility = Visibility.Collapsed;

        public TreeViewUserControlViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task LoadFlows(int? flowId = 0)
        {
            List<Flow> flows = new List<Flow>();

            if (flowId > 0)
                flows = await _baseDatawork.Query.Flows
                    .Include(x => x.FlowStep)
                    .ThenInclude(x => x.ChildrenFlowSteps)
                    .Include(x => x.FlowParameter)
                    .ThenInclude(x => x.ChildrenFlowParameters)
                    .Where(x => x.Id == flowId)
                    .ToListAsync();
            else
                flows = await _baseDatawork.Query.Flows
                    .Include(x => x.FlowStep)
                    .ThenInclude(x => x.ChildrenFlowSteps)
                    .Include(x => x.FlowParameter)
                    .ThenInclude(x => x.ChildrenFlowParameters)
                    .ToListAsync();

            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowStep.ChildrenFlowSteps)
                    await _baseDatawork.FlowSteps.LoadAllExpandedChildren(flowStep);

            //FlowsList = null;
            FlowsList = new ObservableCollection<Flow>(flows);
        }

        public void ClearCopy()
        {
            CoppiedFlowStepId = null;
            PasteVisibility = Visibility.Collapsed;
        }

        public async Task ExpandAll()
        {
            List<Flow> flows = await _baseDatawork.Flows.LoadAllExpanded();
            await _baseDatawork.SaveChangesAsync();
            FlowsList = new ObservableCollection<Flow>(flows);

        }

        public async Task CollapseAll()
        {
            List<Flow> flows = await _baseDatawork.Flows.LoadAllCollapsed();
            await _baseDatawork.SaveChangesAsync();
            FlowsList = new ObservableCollection<Flow>(flows);
        }
        public async Task ExpandAndSelectFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            FlowStep? uiFlowStep = await _baseDatawork.FlowSteps.Query.FirstOrDefaultAsync(x => x.Id == execution.FlowStepId);

            if (uiFlowStep != null)
            {
                uiFlowStep.IsExpanded = true;
                uiFlowStep.IsSelected = true;
            }

            if (uiFlowStep?.ParentFlowStep != null)
                uiFlowStep.ParentFlowStep.IsExpanded = true;
            if (uiFlowStep?.ParentFlowStep?.ParentFlowStep != null)
                uiFlowStep.ParentFlowStep.ParentFlowStep.IsExpanded = true;
            if (uiFlowStep?.Flow != null)
                uiFlowStep.Flow.IsExpanded = true;

            return;
        }



        [RelayCommand]
        private void OnButtonCopyClick(FlowStep flowStep)
        {
            CoppiedFlowStepId = flowStep.Id;
            PasteVisibility = Visibility.Visible;

            // Fire event.
            OnFlowStepCloneEvent?.Invoke(flowStep.Id);
        }

        [RelayCommand]
        private void OnButtonNewClick(FlowStep flowStep)
        {
            FlowStep newFlowStep = new FlowStep();

            if (flowStep.ParentFlowStepId.HasValue)
                newFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;

            if (flowStep.FlowId.HasValue)
                newFlowStep.FlowId = flowStep.FlowId;

            OnAddFlowStepClickEvent?.Invoke(newFlowStep);
        }
        [RelayCommand]
        private void OnButtonNewParameterClick(FlowParameter flowParameter)
        {
            FlowParameter newFlowParameter = new FlowParameter();

            newFlowParameter.ParentFlowParameterId = flowParameter.ParentFlowParameterId;


            OnAddFlowParameterClickEvent?.Invoke(newFlowParameter);
        }

        [RelayCommand]
        private async Task OnButtonPasteClick(FlowStep flowStep)
        {
            FlowStep? clonedFlowStep = null;
            int? parentId = flowStep.ParentFlowStepId ?? flowStep.FlowId ?? null;
            if (CoppiedFlowStepId.HasValue)
                clonedFlowStep = await _baseDatawork.FlowSteps.GetFlowStepClone(CoppiedFlowStepId.Value);

            if (flowStep.ParentFlowStepId.HasValue && clonedFlowStep != null)
            {
                //  Load the target parent.
                FlowStep? targetParent = await _baseDatawork.FlowSteps.Query
                .Include(fs => fs.ChildrenFlowSteps)
                .FirstOrDefaultAsync(fs => fs.Id == flowStep.ParentFlowStepId.Value);

                if (targetParent == null)
                    return;

                FlowStep isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(targetParent.Id);
                clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;

                // Attach the cloned root to the target parent.
                targetParent.ChildrenFlowSteps.Add(clonedFlowStep);
            }
            else if (flowStep.FlowId.HasValue && clonedFlowStep != null)
            {
                //  Load the target parent.
                Flow? targetParent = await _baseDatawork.Flows.Query
                .Include(fs => fs.FlowStep.ChildrenFlowSteps)
                .FirstOrDefaultAsync(fs => fs.Id == flowStep.FlowId.Value);

                if (targetParent == null)
                    return;

                FlowStep isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(targetParent.Id);
                clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;

                // Attach the cloned root to the target parent.
                targetParent.FlowStep.ChildrenFlowSteps.Add(clonedFlowStep);
            }

            // Save changes.
            await _baseDatawork.SaveChangesAsync();
        }



        [RelayCommand]
        private async Task OnFlowStepButtonDeleteClick(FlowStep flowStep)
        {
            _baseDatawork.FlowSteps.Remove(flowStep);

            await _baseDatawork.SaveChangesAsync();
            await LoadFlows();
        }
        [RelayCommand]
        private async Task OnFlowParameterButtonDeleteClick(FlowParameter flowParameter)
        {
            _baseDatawork.FlowParameters.Remove(flowParameter);

            await _baseDatawork.SaveChangesAsync();
            await LoadFlows();
        }
        

        [RelayCommand]
        private async Task OnFlowButtonDeleteClick(Flow flow)
        {
            _baseDatawork.Flows.Remove(flow);

            await _baseDatawork.SaveChangesAsync();
            await LoadFlows();
        }


        [RelayCommand]
        private async Task OnButtonUpClick(FlowStep flowStep)
        {
            List<FlowStep> simplings = await _baseDatawork.FlowSteps.GetSiblings(flowStep.Id);
            List<FlowStep> simplingsAbove = simplings
                .Where(x => x.OrderingNum < flowStep.OrderingNum)
                .Where(x => x.Type != FlowStepTypesEnum.NEW)
                .ToList();

            if (simplingsAbove.Any())
            {
                // Find max
                FlowStep simplingAbove = simplingsAbove.Aggregate((currentMax, x) => x.OrderingNum > currentMax.OrderingNum ? x : currentMax);

                // Swap values
                (flowStep.OrderingNum, simplingAbove.OrderingNum) = (simplingAbove.OrderingNum, flowStep.OrderingNum);

                await _baseDatawork.SaveChangesAsync();
                await LoadFlows();
            }
        }

        [RelayCommand]
        private async Task OnButtonDownClick(FlowStep flowStep)
        {
            List<FlowStep> simplings = await _baseDatawork.FlowSteps.GetSiblings(flowStep.Id);
            List<FlowStep> simplingsBellow = simplings
                .Where(x => x.OrderingNum > flowStep.OrderingNum)
                .Where(x => x.Type != FlowStepTypesEnum.NEW)
                .ToList();

            if (simplingsBellow.Any())
            {
                // Find min
                FlowStep simplingBellow = simplingsBellow.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);

                // Swap values
                (flowStep.OrderingNum, simplingBellow.OrderingNum) = (simplingBellow.OrderingNum, flowStep.OrderingNum);

                await _baseDatawork.SaveChangesAsync();
                await LoadFlows();
            }
        }


        [RelayCommand]
        private void OnSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is FlowStep flowStep)
            {
                _selectedFlow = null;
                _selectedFlowStep = flowStep;
                _selectedFlowParameter = null;
                OnSelectedFlowStepIdChangedEvent?.Invoke(flowStep.Id);
            }
            else if (selectedItem is Flow flow)
            {
                _selectedFlow = flow;
                _selectedFlowStep = null;
                _selectedFlowParameter = null;
                OnSelectedFlowIdChangedEvent?.Invoke(flow.Id);
            }
            else if (selectedItem is FlowParameter flowParameter)
            {
                _selectedFlow = null;
                _selectedFlowStep = null;
                _selectedFlowParameter = flowParameter;
            }
        }


        [RelayCommand]
        private void OnDoubleClick()
        {
            if (_selectedFlowStep != null)
                _selectedFlowStep.IsExpanded = !_selectedFlowStep.IsExpanded;

            if (_selectedFlow != null)
                _selectedFlow.IsExpanded = !_selectedFlow.IsExpanded;

            if (_selectedFlowParameter != null)
                _selectedFlowParameter.IsExpanded = !_selectedFlowParameter.IsExpanded;
        }

        [RelayCommand]
        private async Task OnExpanded(object eventParameter)
        {
            if (eventParameter is FlowStep flowStep)
                await _baseDatawork.FlowSteps.LoadAllExpandedChildren(flowStep);

            else if (eventParameter is Flow flow)
                foreach (var childFlowStep in flow.FlowStep.ChildrenFlowSteps)
                    await _baseDatawork.FlowSteps.LoadAllExpandedChildren(childFlowStep);
        }
    }
}
