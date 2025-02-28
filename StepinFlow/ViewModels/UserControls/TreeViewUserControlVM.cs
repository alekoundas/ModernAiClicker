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
using System.Linq.Expressions;

namespace StepinFlow.ViewModels.UserControls
{
    public partial class TreeViewUserControlVM : ObservableObject, INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        private readonly ISystemService _systemService;
        private readonly ICloneService _cloneService;



        public event OnSelectedFlowIdChanged? OnSelectedFlowIdChangedEvent;
        public delegate void OnSelectedFlowIdChanged(int Id);

        public event OnSelectedFlowParameterIdChanged? OnSelectedFlowParameterIdChangedEvent;
        public delegate void OnSelectedFlowParameterIdChanged(int Id);

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
        private bool _isLocked = true;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        [ObservableProperty]
        private Visibility? _pasteVisibility = Visibility.Collapsed;

        public TreeViewUserControlVM(IDataService dataService, ISystemService systemService, ICloneService cloneService)
        {
            _dataService = dataService;
            _systemService = systemService;
            _cloneService = cloneService;
        }

        public async Task LoadFlows(int flowId = 0, bool isSubFlow = false)
        {
            List<Expression<Func<Flow, bool>>>? filters = new List<Expression<Func<Flow, bool>>>();

            if (isSubFlow)
                filters.Add(x => x.Type == FlowTypesEnum.SUB_FLOW);
            else
                filters.Add(x => x.Type == FlowTypesEnum.FLOW);

            if (flowId > 0)
                filters.Add(x => x.Id == flowId);

            IQueryable<Flow> query = _dataService.Query.Flows
                .Include(x => x.FlowStep)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.SubFlow)
                .Include(x => x.FlowParameter)
                .ThenInclude(x => x.ChildrenFlowParameters);

            foreach (var filter in filters)
                query = query.Where(filter);


            // Clear trackers from dbcontext and execute query.
            _dataService.Query.ChangeTracker.Clear();
            List<Flow> flows = await query.ToListAsync();

            // Load children.
            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowStep.ChildrenFlowSteps)
                    await _dataService.FlowSteps.LoadAllExpandedChildren(flowStep);

            FlowsList = new ObservableCollection<Flow>(flows);
        }

        public async Task LoadFlowsAndSelectFlowStep(int id)
        {
            await _dataService.SaveChangesAsync();
            await LoadFlows();
            await ExpandAndSelectFlowStep(id);
        }

        public void ClearCopy()
        {
            CoppiedFlowStepId = null;
            PasteVisibility = Visibility.Collapsed;
        }

        public async Task ExpandAll()
        {
            List<Flow> flows = await _dataService.Flows.LoadAllExpanded();
            await _dataService.SaveChangesAsync();
            FlowsList = new ObservableCollection<Flow>(flows);

        }

        public async Task CollapseAll()
        {
            List<Flow> flows = await _dataService.Flows.LoadAllCollapsed();
            await _dataService.SaveChangesAsync();
            FlowsList = new ObservableCollection<Flow>(flows);
        }
        public async Task ExpandAndSelectFlowStep(int id)
        {
            FlowStep? uiFlowStep = await _dataService.FlowSteps.Query.FirstOrDefaultAsync(x => x.Id == id);

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
                clonedFlowStep = await _cloneService.GetFlowStepClone(CoppiedFlowStepId.Value);
            //clonedFlowStep = await _dataService.FlowSteps.GetFlowStepClone(CoppiedFlowStepId.Value);

            if (flowStep.ParentFlowStepId.HasValue && clonedFlowStep != null)
            {
                //  Load the target parent.
                FlowStep? targetParent = await _dataService.FlowSteps.Query
                .Include(fs => fs.ChildrenFlowSteps)
                .FirstOrDefaultAsync(fs => fs.Id == flowStep.ParentFlowStepId.Value);

                if (targetParent == null)
                    return;

                FlowStep isNewSimpling = await _dataService.FlowSteps.GetIsNewSibling(targetParent.Id);
                clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;

                // Attach the cloned root to the target parent.
                targetParent.ChildrenFlowSteps.Add(clonedFlowStep);
            }
            else if (flowStep.FlowId.HasValue && clonedFlowStep != null)
            {
                //  Load the target parent.
                Flow? targetParent = await _dataService.Flows.Query
                .Include(fs => fs.FlowStep.ChildrenFlowSteps)
                .FirstOrDefaultAsync(fs => fs.Id == flowStep.FlowId.Value);

                if (targetParent == null)
                    return;

                FlowStep isNewSimpling = await _dataService.Flows.GetIsNewSibling(targetParent.Id);
                clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;

                // Attach the cloned root to the target parent.
                targetParent.FlowStep.ChildrenFlowSteps.Add(clonedFlowStep);
            }

            // Save changes.
            await _dataService.SaveChangesAsync();
        }



        [RelayCommand]
        private async Task OnFlowStepButtonDeleteClick(FlowStep flowStep)
        {
            _dataService.FlowSteps.Remove(flowStep);

            await _dataService.SaveChangesAsync();
            await LoadFlows();
        }
        [RelayCommand]
        private async Task OnFlowParameterButtonDeleteClick(FlowParameter flowParameter)
        {
            _dataService.FlowParameters.Remove(flowParameter);

            await _dataService.SaveChangesAsync();
            await LoadFlows();
        }


        [RelayCommand]
        private async Task OnFlowButtonDeleteClick(Flow flow)
        {
            _dataService.Flows.Remove(flow);

            await _dataService.SaveChangesAsync();
            await LoadFlows();
        }


        [RelayCommand]
        private async Task OnButtonUpClick(FlowStep flowStep)
        {
            List<FlowStep> simplings = await _dataService.FlowSteps.GetSiblings(flowStep.Id);
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

                await _dataService.SaveChangesAsync();
                await LoadFlows();
            }
        }

        [RelayCommand]
        private async Task OnButtonDownClick(FlowStep flowStep)
        {
            List<FlowStep> simplings = await _dataService.FlowSteps.GetSiblings(flowStep.Id);
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

                await _dataService.SaveChangesAsync();
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
                OnSelectedFlowParameterIdChangedEvent?.Invoke(flowParameter.Id);

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
                await _dataService.FlowSteps.LoadAllExpandedChildren(flowStep);

            else if (eventParameter is Flow flow)
                foreach (var childFlowStep in flow.FlowStep.ChildrenFlowSteps)
                    await _dataService.FlowSteps.LoadAllExpandedChildren(childFlowStep);

            await _dataService.SaveChangesAsync();
        }
    }
}
