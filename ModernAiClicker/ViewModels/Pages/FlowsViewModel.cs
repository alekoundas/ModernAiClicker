﻿using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.ConverterModels;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Business.Services;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowsViewModel : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);


        public new event PropertyChangedEventHandler? PropertyChanged;


        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();
        public ObservableCollection<Flow> FlowsList
        {
            get { return _flowsList; }
            set
            {
                _flowsList = value;
                NotifyPropertyChanged(nameof(FlowsList));
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                NotifyPropertyChanged(nameof(IsLocked));
            }
        }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public FlowsViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;




            RefreshData();
        }

        //TODO find a fix for includes
        public void RefreshData()
        {
            List<Flow> flows = _baseDatawork.Query.Flows
                .Include(x => x.FlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ToList();

            foreach (Flow flow in flows)
            {
                foreach (FlowStep flowStep in flow.FlowSteps)
                {
                    LoadChildren(flowStep);
                }
            }

            FlowsList = new ObservableCollection<Flow>(flows);
        }

        private void LoadChildren(FlowStep flowStep)
        {
            List<FlowStep> flowSteps = _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .First(x => x.Id == flowStep.Id)
                        .ChildrenFlowSteps
                        .ToList();

            flowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);

            foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
            {
                if (childFlowStep.IsExpanded)
                    LoadChildren(childFlowStep);
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
            //RefreshData();
        }


        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;
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
        private async Task OnTreeViewItemFlowStepButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                _baseDatawork.FlowSteps.Remove(flowStep);

                await _baseDatawork.SaveChangesAsync();
                RefreshData();
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
                RefreshData();
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
                    RefreshData();
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
                    RefreshData();
                }
            }
        }


        [RelayCommand]
        private void OnTreeViewItemSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is FlowStep)
                NavigateToFlowStepTypeSelectionPage?.Invoke((FlowStep)selectedItem);

        }

        [RelayCommand]
        private void OnTreeViewItemExpanded(EventParammeters eventParameters)
        {
            if (eventParameters == null)
                return;

            if (eventParameters.FlowId is FlowStep)
            {

                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                if (flowStep.ChildrenFlowSteps == null)
                    return;
                foreach (var childrenFlowStep in flowStep.ChildrenFlowSteps)
                {
                    if (childrenFlowStep.ChildrenFlowSteps == null || childrenFlowStep.ChildrenFlowSteps.Count == 0)
                    {
                        List<FlowStep> flowSteps = _baseDatawork.Query.FlowSteps
                            .Include(x => x.ChildrenFlowSteps)
                            .First(x => x.Id == childrenFlowStep.Id)
                            .ChildrenFlowSteps
                            .ToList();

                        childrenFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
                    }
                }
            }

            else if (eventParameters.FlowId is Flow)
            {

                Flow flow = (Flow)eventParameters.FlowId;

                foreach (var childrenFlowStep in flow.FlowSteps)
                {
                    if (childrenFlowStep.ChildrenFlowSteps == null || childrenFlowStep.ChildrenFlowSteps.Count == 0)
                    {
                        List<FlowStep> flowSteps = _baseDatawork.Query.FlowSteps
                            .Include(x => x.ChildrenFlowSteps)
                            .First(x => x.Id == childrenFlowStep.Id)
                            .ChildrenFlowSteps
                            .ToList();

                        childrenFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
                    }
                }
            }
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
