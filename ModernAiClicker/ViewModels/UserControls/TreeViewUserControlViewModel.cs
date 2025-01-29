﻿using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.ConverterModels;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ModernAiClicker.ViewModels.UserControls
{
    public partial class TreeViewUserControlViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public event OnSelectedFlowStepIdChanged? OnSelectedFlowStepIdChangedEvent;
        public delegate void OnSelectedFlowStepIdChanged(int Id);

        public event OnFlowStepClone? OnFlowStepCloneEvent;
        public delegate void OnFlowStepClone(int Id);

        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        public TreeViewUserControlViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task LoadFlows(int flowId = 0)
        {
            List<Flow> flows = new List<Flow>();

            if (flowId > 0)
                flows = await _baseDatawork.Query.Flows
                    .Include(x => x.FlowSteps)
                    .Where(x => x.Id == flowId)
                    .ToListAsync();
            else
                flows = await _baseDatawork.Query.Flows
                    .Include(x => x.FlowSteps)
                    .ToListAsync();

            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowSteps)
                    await _baseDatawork.FlowSteps.LoadAllChildren(flowStep);

            //FlowsList = null;
            FlowsList = new ObservableCollection<Flow>(flows);
        }

        [RelayCommand]
        private void OnButtonCopyClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                CoppiedFlowStepId = flowStep.Id;

                // Fire event.
                OnFlowStepCloneEvent?.Invoke(flowStep.Id);
            }
        }

        public async Task AddNewFlow()
        {
            FlowStep newFlowStep = new FlowStep();
            newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;

            Flow flow = new Flow();
            flow.Name = "Flow";
            flow.IsSelected = true;
            flow.FlowSteps.Add(newFlowStep);

            _baseDatawork.Flows.Add(flow);
            await _baseDatawork.SaveChangesAsync();

            FlowsList.Add(flow);
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

        [RelayCommand]
        private void OnButtonNewClick(EventParammeters eventParameters)
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

        }

        [RelayCommand]
        private async Task OnButtonPasteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                int? parentId = flowStep.ParentFlowStepId ?? flowStep.FlowId ?? null;
                if (CoppiedFlowStepId.HasValue)
                {

                    FlowStep? clonedFlowStep = await _baseDatawork.FlowSteps.GetFlowStepClone(CoppiedFlowStepId.Value);
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
                        .Include(fs => fs.FlowSteps)
                        .FirstOrDefaultAsync(fs => fs.Id == flowStep.FlowId.Value);

                        if (targetParent == null)
                            return;

                        FlowStep isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(targetParent.Id);
                        clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                        clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                        isNewSimpling.OrderingNum++;

                        // Attach the cloned root to the target parent.
                        targetParent.FlowSteps.Add(clonedFlowStep);

                    }
                    // Save changes.
                    await _baseDatawork.SaveChangesAsync();
                }
            }
        }



        [RelayCommand]
        private async Task OnFlowStepButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                _baseDatawork.FlowSteps.Remove(flowStep);

                await _baseDatawork.SaveChangesAsync();
                await LoadFlows();
            }
        }

        [RelayCommand]
        private async Task OnFlowButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;
                _baseDatawork.Flows.Remove(flow);

                await _baseDatawork.SaveChangesAsync();
                await LoadFlows();
            }
        }


        [RelayCommand]
        private async Task OnButtonUpClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplings = await _baseDatawork.FlowSteps.GetSiblings(flowStep.Id);
                List<FlowStep> simplingsAbove = simplings
                    .Where(x => x.OrderingNum < flowStep.OrderingNum)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
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
        }

        [RelayCommand]
        private async Task OnButtonDownClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplings = await _baseDatawork.FlowSteps.GetSiblings(flowStep.Id);
                List<FlowStep> simplingsBellow = simplings
                    .Where(x => x.OrderingNum > flowStep.OrderingNum)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
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
        }


        [RelayCommand]
        private void OnSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is FlowStep)
            {
                FlowStep flowStep = (FlowStep)selectedItem;
                OnSelectedFlowStepIdChangedEvent?.Invoke(flowStep.Id);
            }

        }

        [RelayCommand]
        private async Task OnExpanded(EventParammeters eventParameters)
        {
            if (eventParameters == null)
                return;

            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;

                foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
                    await _baseDatawork.FlowSteps.LoadAllChildren(childFlowStep);
            }

            else if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;

                foreach (var childFlowStep in flow.FlowSteps)
                    await _baseDatawork.FlowSteps.LoadAllChildren(childFlowStep);
            }
        }
    }
}
