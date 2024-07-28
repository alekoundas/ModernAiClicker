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
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .Include(x => x.Executions)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
                .ThenInclude(x => x.ChildExecution)
            .ToList();

            FlowsList = new ObservableCollection<Flow>(flows);
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
            if (eventParameters.FlowStepId == null)
                return;

            bool isFlowStepIdParsable = Int32.TryParse(eventParameters.FlowStepId.ToString(), out int flowStepId);
            if (isFlowStepIdParsable)
            {
                FlowStep flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
                _baseDatawork.FlowSteps.Remove(flowStep);
                _baseDatawork.SaveChanges();


                await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

                RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId == null)
                return;

            bool isFlowIdParsable = Int32.TryParse(eventParameters.FlowId.ToString(), out int flowId);
            if (isFlowIdParsable)
            {
                Flow flow = await _baseDatawork.Flows.FirstOrDefaultAsync(x => x.Id == flowId);
                _baseDatawork.Flows.Remove(flow);
                _baseDatawork.SaveChanges();


                await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

                RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonUpClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowStepId != null)
            {
                bool isFlowStepIdParsable = Int32.TryParse(eventParameters.FlowStepId.ToString(), out int flowStepId);

                if (isFlowStepIdParsable)
                {
                    List<FlowStep> simplingsAbove;
                    FlowStep simplingAbove;
                    FlowStep flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
                    if (flowStep.OrderingNum == 0)
                        return;

                    if (flowStep.ParentFlowStepId != null)
                    {

                        simplingsAbove = _baseDatawork.FlowSteps
                            .Where(x => x.Id == flowStep.ParentFlowStepId)
                            .SelectMany(x => x.ChildrenFlowSteps)
                            .Where(x => x.OrderingNum < flowStep.OrderingNum )
                            .ToList();

                        if (simplingsAbove.Any())
                        {
                            // Find max
                            simplingAbove = simplingsAbove.Aggregate((currentMax, x) => x.OrderingNum > currentMax.OrderingNum ? x : currentMax);

                            // Swap values
                            (flowStep.OrderingNum, simplingAbove.OrderingNum) = (simplingAbove.OrderingNum, flowStep.OrderingNum);
                        }
                    }

                    _baseDatawork.SaveChanges();
                    await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
                    RefreshData();
                }
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonDownClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowStepId != null)
            {
                bool isFlowStepIdParsable = Int32.TryParse(eventParameters.FlowStepId.ToString(), out int flowStepId);

                if (isFlowStepIdParsable)
                {
                    List<FlowStep> simplingsAbove;
                    FlowStep simplingAbove;
                    FlowStep flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
                    if (flowStep.OrderingNum == 0)
                        return;

                    if (flowStep.ParentFlowStepId != null)
                    {

                        simplingsAbove = _baseDatawork.FlowSteps
                            .Where(x => x.Id == flowStep.ParentFlowStepId)
                            .SelectMany(x => x.ChildrenFlowSteps)
                            .Where(x => x.OrderingNum > flowStep.OrderingNum )
                            .ToList();

                        if (simplingsAbove.Any())
                        {
                            // Find min
                            simplingAbove = simplingsAbove.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);

                            // Swap values
                            (flowStep.OrderingNum, simplingAbove.OrderingNum) = (simplingAbove.OrderingNum, flowStep.OrderingNum);
                        }
                    }

                    _baseDatawork.SaveChanges();
                    await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
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



        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
