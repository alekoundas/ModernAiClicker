using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.ConverterModels;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowsViewModel : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);

        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        public FlowsViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            Task.Run(async () => await RefreshData());

        }

        public async Task RefreshData()
        {
            List<Flow> flows = await _baseDatawork.Query.Flows
                .Include(x => x.FlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ToListAsync();

            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowSteps)
                    await LoadFlowStepChildren(flowStep);

            FlowsList = null;
            FlowsList = new ObservableCollection<Flow>(flows);
        }

        private async Task LoadFlowStepChildren(FlowStep flowStep)
        {
            List<FlowStep> flowSteps = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .ThenInclude(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.Id)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .ToListAsync();

            flowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);

            foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
            {
                if (childFlowStep.IsExpanded)
                    await LoadFlowStepChildren(childFlowStep);
            }
        }


        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            Flow flow = new Flow();
            FlowStep newFlowStep = new FlowStep();

            newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;
            newFlowStep.IsSelected = false;
            flow.Name = "Flow";
            flow.IsSelected = true;
            flow.FlowSteps.Add(newFlowStep);
            _baseDatawork.Flows.Add(flow);
            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            FlowsList.Add(flow);
            await RefreshData();
        }


        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;
        }

        [RelayCommand]
        private async Task OnButtonSyncClick()
        {
            await RefreshData();
        }

        [RelayCommand]
        private async Task OnButtonExpandAllClick()
        {
            List<Flow> flows = await _baseDatawork.Flows.GetAllAsync();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.GetAllAsync();

            flows.ForEach(x => x.IsExpanded = true);
            flowSteps.ForEach(x => x.IsExpanded = true);

            await _baseDatawork.SaveChangesAsync();
        }

        [RelayCommand]
        private async Task OnButtonCollapseAllClick()
        {
            List<Flow> flows = await _baseDatawork.Flows.GetAllAsync();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.GetAllAsync();

            flows.ForEach(x => x.IsExpanded = false);
            flowSteps.ForEach(x => x.IsExpanded = false);

            await _baseDatawork.SaveChangesAsync();
        }

        [RelayCommand]
        private void OnTreeViewItemButtonNewClick(EventParammeters eventParameters)
        {
            FlowStep flowStep = new FlowStep();

            // If flowId is available
            if (eventParameters.FlowId != null)
            {
                bool isFlowIdParsable = Int32.TryParse(eventParameters.FlowId.ToString(), out int flowId);

                if (isFlowIdParsable)
                    flowStep.FlowId = flowId;
            }
            // If flowStepId is available
            else if (eventParameters.FlowStepId != null)
            {
                bool isFlowStepIdParsable = Int32.TryParse(eventParameters.FlowStepId.ToString(), out int flowStepId);
                if (isFlowStepIdParsable)
                {

                    int? parentFlowStepId = _baseDatawork.FlowSteps
                        .Where(x => x.Id == flowStepId)
                        .Select(x => x.ParentFlowStepId)
                        .First();

                    flowStep.ParentFlowStepId = parentFlowStepId;
                }
            }

            NavigateToFlowStepTypeSelectionPage?.Invoke(flowStep);
        }

        [RelayCommand]
        private async Task OnTreeViewItemButtonPasteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;

                FlowStep coppyFlowStep = await _baseDatawork.FlowSteps.Query
                    .Include(x => x.ChildrenTemplateSearchFlowSteps)
                    .Include(x => x.ChildrenFlowSteps)
                    .ThenInclude(x => x.ChildrenFlowSteps)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == CoppiedFlowStepId);

                List<FlowStep> flowSteps = new List<FlowStep>();
                flowSteps.Add(coppyFlowStep);

                // Load all children of coppied flow step.
                while (flowSteps.Count > 0)
                {
                    foreach (var loadFlowStep in flowSteps)
                    {
                        foreach (var child in loadFlowStep.ChildrenFlowSteps)
                        {
                            child.ChildrenFlowSteps = new ObservableCollection<FlowStep>(
                                await _baseDatawork.FlowSteps.Query
                                    .AsNoTracking()
                                    .Include(x => x.ChildrenTemplateSearchFlowSteps)
                                    .Include(x => x.ChildrenFlowSteps)
                                    .ThenInclude(x => x.ChildrenFlowSteps)
                                    .Where(x => x.Id == child.Id)
                                    .SelectMany(x => x.ChildrenFlowSteps)
                                    .ToListAsync());
                            child.Id = 0;
                            child.ParentTemplateSearchFlowStepId = 0;
                            child.ParentFlowStepId = 0;
                        }
                        loadFlowStep.Id = 0;
                        loadFlowStep.ParentTemplateSearchFlowStepId = 0;
                        loadFlowStep.ParentFlowStepId = 0;
                    }

                    flowSteps = flowSteps.SelectMany(x => x.ChildrenFlowSteps).SelectMany(x => x.ChildrenFlowSteps).ToList();
                }


                coppyFlowStep.Id = 0;







                coppyFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                coppyFlowStep.OrderingNum = flowStep.OrderingNum;
                flowStep.OrderingNum++;

                _baseDatawork.FlowSteps.Add(coppyFlowStep);
                await _baseDatawork.SaveChangesAsync();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                _baseDatawork.FlowSteps.Remove(flowStep);

                await _baseDatawork.SaveChangesAsync();
                await RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;
                _baseDatawork.Flows.Remove(flow);

                await _baseDatawork.SaveChangesAsync();
                await RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonCopyClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                CoppiedFlowStepId = flowStep.Id;
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonUpClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplingsAbove = new List<FlowStep>();

                // Get siblings based on flowstep beeing bellow flow or flowstep
                if (flowStep.ParentFlowStepId.HasValue)
                    simplingsAbove = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.ParentFlowStepId.Value)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .Where(x => x.OrderingNum < flowStep.OrderingNum)
                        .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                        .ToListAsync();
                else if (flowStep.FlowId.HasValue)
                    simplingsAbove = await _baseDatawork.Query.Flows
                        .Include(x => x.FlowSteps)
                        .Where(x => x.Id == flowStep.FlowId.Value)
                        .SelectMany(x => x.FlowSteps)
                        .Where(x => x.OrderingNum < flowStep.OrderingNum)
                        .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                        .ToListAsync();


                if (simplingsAbove.Any())
                {
                    // Find max
                    FlowStep simplingAbove = simplingsAbove.Aggregate((currentMax, x) => x.OrderingNum > currentMax.OrderingNum ? x : currentMax);

                    // Swap values
                    (flowStep.OrderingNum, simplingAbove.OrderingNum) = (simplingAbove.OrderingNum, flowStep.OrderingNum);

                    await _baseDatawork.SaveChangesAsync();
                    await RefreshData();
                }
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonDownClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplingsBellow = new List<FlowStep>();

                // Get siblings based on flowstep beeing bellow flow or flowstep
                if (flowStep.ParentFlowStepId.HasValue)
                    simplingsBellow = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.ParentFlowStepId.Value)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .Where(x => x.OrderingNum > flowStep.OrderingNum)
                        .ToListAsync();
                else if (flowStep.FlowId.HasValue)
                    simplingsBellow = await _baseDatawork.Query.Flows
                        .Include(x => x.FlowSteps)
                        .Where(x => x.Id == flowStep.FlowId.Value)
                        .SelectMany(x => x.FlowSteps)
                        .Where(x => x.OrderingNum > flowStep.OrderingNum)
                        .ToListAsync();


                if (simplingsBellow.Any())
                {
                    // Find min
                    FlowStep simplingBellow = simplingsBellow.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);

                    // Swap values
                    (flowStep.OrderingNum, simplingBellow.OrderingNum) = (simplingBellow.OrderingNum, flowStep.OrderingNum);

                    await _baseDatawork.SaveChangesAsync();
                    await RefreshData();
                }
            }
        }


        [RelayCommand]
        private async Task OnTreeViewItemSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is FlowStep)
            {
                FlowStep flowStep = (FlowStep)selectedItem;
                flowStep = await _baseDatawork.Query.FlowSteps.Include(x => x.ChildrenTemplateSearchFlowSteps).Where(x => x.Id == flowStep.Id).FirstOrDefaultAsync();
                NavigateToFlowStepTypeSelectionPage?.Invoke(flowStep);
            }

        }

        [RelayCommand]
        private async Task OnTreeViewItemExpanded(EventParammeters eventParameters)
        {
            if (eventParameters == null)
                return;

            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;

                if (flowStep.ChildrenFlowSteps == null)
                    return;

                foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
                    await LoadFlowStepChildren(childFlowStep);
            }

            else if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;

                foreach (var childFlowStep in flow.FlowSteps)
                    await LoadFlowStepChildren(childFlowStep);

            }
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
