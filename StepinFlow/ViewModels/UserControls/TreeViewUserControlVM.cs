using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using Business.Repository;
using Business.Services.Interfaces;

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


        private List<Expression<Func<Flow, bool>>> _loadFilters = new List<Expression<Func<Flow, bool>>>();

        public TreeViewUserControlVM(IDataService dataService, ISystemService systemService, ICloneService cloneService)
        {
            _dataService = dataService;
            _systemService = systemService;
            _cloneService = cloneService;
        }

        public async Task LoadFlows(int flowId = 0, bool isSubFlow = false)
        {
            List<Expression<Func<Flow, bool>>> filters = new List<Expression<Func<Flow, bool>>>();

            if (isSubFlow)
                filters.Add(x => x.Type == FlowTypesEnum.SUB_FLOW);
            else
                filters.Add(x => x.Type == FlowTypesEnum.FLOW);

            if (flowId > 0)
                filters.Add(x => x.Id == flowId);

            IQueryable<Flow> query = _dataService.Query.Flows
                .AsNoTracking()
                .Include(x => x.FlowStep)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.SubFlow)
                .Include(x => x.FlowParameter)
                .ThenInclude(x => x.ChildrenFlowParameters);

            if (_loadFilters.Count > 0)
                foreach (var filter in _loadFilters)
                    query = query.Where(filter);
            else
            {
                _loadFilters = filters;
                foreach (var filter in filters)
                    query = query.Where(filter);
            }


            // Clear trackers from dbcontext and execute query.
            _dataService.Query.ChangeTracker.Clear();
            await Task.Run(async () =>
            {
                List<Flow> flows = await query.ToListAsync();

                // Load children.
                foreach (Flow flow in flows)
                {
                    List<FlowStep> childrenFlowSteps = new List<FlowStep>();
                    foreach (FlowStep flowStep in flow.FlowStep.ChildrenFlowSteps)
                        childrenFlowSteps.Add(await _dataService.FlowSteps.LoadAllExpandedChildren(flowStep));

                    flow.FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(childrenFlowSteps);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    FlowsList = new ObservableCollection<Flow>(flows);
                });
            });
        }

        public async Task LoadFlowsAndSelectFlow(int id)
        {
            //await _dataService.SaveChangesAsync();
            await LoadFlows();
            await ExpandAndSelectFlow(id);
        }

        public async Task LoadFlowsAndSelectFlowStep(int id)
        {
            //await _dataService.SaveChangesAsync();
            await LoadFlows();
            await ExpandAndSelectFlowStep(id);
        }

        public async Task LoadFlowsAndSelectFlowParameter(int id)
        {
            //await _dataService.SaveChangesAsync();
            await LoadFlows();
            await ExpandAndSelectFlowParameter(id);
        }

        public void ClearCopy()
        {
            CoppiedFlowStepId = null;
            PasteVisibility = Visibility.Collapsed;
        }

        public async Task ExpandAll()
        {
            //_dataService.Query.ChangeTracker.Clear();
            //List<Flow> flows = await _dataService.Flows.LoadAllExpanded();
            //await _dataService.SaveChangesAsync();
            await LoadFlows();
        }

        public async Task CollapseAll()
        {
            //_dataService.Query.ChangeTracker.Clear();
            //List<Flow> flows = await _dataService.Flows.LoadAllCollapsed();
            //await _dataService.SaveChangesAsync();
            await LoadFlows();
        }

        public async Task ExpandAndSelectFlow(int id)
        {
            _dataService.Query.ChangeTracker.Clear();
            Flow? uiFlowStep = await _dataService.Flows.Query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (uiFlowStep != null)
            {
                uiFlowStep.IsExpanded = true;
                uiFlowStep.IsSelected = true;
            }
            await _dataService.UpdateAsync(uiFlowStep);

            return;
        }

        public async Task ExpandAndSelectFlowStep(int id)
        {
            _dataService.Query.ChangeTracker.Clear();
            FlowStep? uiFlowStep = await _dataService.FlowSteps.Query.FirstOrDefaultAsync(x => x.Id == id);

            if (uiFlowStep != null)
            {
                uiFlowStep.IsExpanded = true;
                uiFlowStep.IsSelected = true;
            }

            //if (uiFlowStep?.ParentFlowStep != null)
            //    uiFlowStep.ParentFlowStep.IsExpanded = true;
            //if (uiFlowStep?.ParentFlowStep?.ParentFlowStep != null)
            //    uiFlowStep.ParentFlowStep.ParentFlowStep.IsExpanded = true;
            //if (uiFlowStep?.Flow != null)
            //    uiFlowStep.Flow.IsExpanded = true;

            return;
        }

        public async Task ExpandAndSelectFlowParameter(int id)
        {
            _dataService.Query.ChangeTracker.Clear();
            FlowParameter? uiFlowStep = await _dataService.FlowParameters.Query.FirstOrDefaultAsync(x => x.Id == id);

            if (uiFlowStep != null)
            {
                uiFlowStep.IsExpanded = true;
                uiFlowStep.IsSelected = true;
            }

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
            _dataService.Query.ChangeTracker.Clear();
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
                await _dataService.UpdateAsync(targetParent);

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
                await _dataService.UpdateAsync(targetParent);
            }

            // Save changes.
            await LoadFlows();
        }



        [RelayCommand]
        private async Task OnFlowStepButtonDeleteClick(FlowStep flowStep)
        {
            _dataService.Query.ChangeTracker.Clear();
            FlowStep? deleteFlowStep = await _dataService.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStep.Id);
            if (deleteFlowStep != null)
                await _dataService.FlowSteps.RemoveAsync(deleteFlowStep);

            await LoadFlows();
        }
        [RelayCommand]
        private async Task OnFlowParameterButtonDeleteClick(FlowParameter flowParameter)
        {
            _dataService.Query.ChangeTracker.Clear();
            FlowParameter? deleteFlowParameter = await _dataService.FlowParameters.FirstOrDefaultAsync(x => x.Id == flowParameter.Id);
            if (deleteFlowParameter != null)
                await _dataService.FlowParameters.RemoveAsync(deleteFlowParameter);

            await LoadFlows();
        }


        [RelayCommand]
        private async Task OnFlowButtonDeleteClick(Flow flow)
        {
            _dataService.Query.ChangeTracker.Clear();
            Flow? deleteFlow = await _dataService.Flows.FirstOrDefaultAsync(x => x.Id == flow.Id);
            if (deleteFlow != null)
                await _dataService.Flows.RemoveAsync(deleteFlow);

            await LoadFlows();
        }


        [RelayCommand]
        private async Task OnButtonUpClick(FlowStep flowStep)
        {
            _dataService.Query.ChangeTracker.Clear();
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

                await _dataService.UpdateAsync(flowStep);
                await _dataService.UpdateAsync(simplingAbove);
                await LoadFlows();
            }
        }

        [RelayCommand]
        private async Task OnButtonDownClick(FlowStep flowStep)
        {
            _dataService.Query.ChangeTracker.Clear();
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

                await _dataService.UpdateAsync(flowStep);
                await _dataService.UpdateAsync(simplingBellow);
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
            _dataService.Query.ChangeTracker.Clear();
            if (eventParameter is FlowStep flowStep)
            {
                FlowStep? updateFlowStep = await _dataService.FlowSteps.Query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowStep.Id);
                if (updateFlowStep != null)
                {
                    updateFlowStep.IsExpanded = true;
                    updateFlowStep.IsSelected = true;
                    await _dataService.UpdateAsync(updateFlowStep);
                }
            }
            else if (eventParameter is Flow flow)
            {
                Flow? updateFlow = await _dataService.Flows.Query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flow.Id);
                if (updateFlow != null)
                {
                    updateFlow.IsExpanded = true;
                    updateFlow.IsSelected = true;
                    await _dataService.UpdateAsync(updateFlow);
                }
            }

            await LoadFlows();
        }
    }
}
