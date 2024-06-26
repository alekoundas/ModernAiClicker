using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.ConverterModels;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wpf.Ui.Controls;
using Business.Services;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowsViewModel : BaseViewModel, INavigationAware
    {
        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);


        private Flow? _selectedFlow;
        private FlowStep? _selectedFlowStep;

        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public FlowsViewModel( IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            RefreshData();
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
                    FlowStep simplingAbove;
                    FlowStep flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
                    if (flowStep.OrderingNum == 0)
                        return;

                    if (flowStep.ParentFlowStepId != null)
                        simplingAbove = _baseDatawork.FlowSteps
                            .Where(x => x.Id == flowStep.ParentFlowStepId)
                            .Select(x => x.ChildrenFlowSteps.First(y => y.OrderingNum == flowStep.OrderingNum - 1))
                            .First();
                    else
                        simplingAbove = _baseDatawork.Flows
                            .Where(x => x.Id == flowStep.FlowId)
                            .Select(x => x.FlowSteps.First(y => y.OrderingNum == flowStep.OrderingNum - 1))
                            .First();


                    flowStep.OrderingNum--;
                    simplingAbove.OrderingNum++;

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
                    FlowStep simplingBellow;
                    FlowStep flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);

                    if (flowStep.ParentFlowStepId != null)
                        simplingBellow = _baseDatawork.FlowSteps
                            .Where(x => x.Id == flowStep.ParentFlowStepId)
                            .Select(x => x.ChildrenFlowSteps.FirstOrDefault(y => y.OrderingNum == flowStep.OrderingNum + 1 && y.FlowStepType != FlowStepTypesEnum.IS_NEW))
                            .FirstOrDefault();
                    else
                        simplingBellow = _baseDatawork.Flows
                            .Where(x => x.Id == flowStep.FlowId)
                            .Select(x => x.FlowSteps.FirstOrDefault(y => y.OrderingNum == flowStep.OrderingNum + 1 && y.FlowStepType != FlowStepTypesEnum.IS_NEW))
                            .FirstOrDefault();

                    if (simplingBellow == null)
                        return;

                    flowStep.OrderingNum++;
                    simplingBellow.OrderingNum--;

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
            {
                _selectedFlowStep = (FlowStep)selectedItem;
                NavigateToFlowStepTypeSelectionPage?.Invoke(_selectedFlowStep);
            }
            else if (selectedItem is Flow)
                _selectedFlow = (Flow)selectedItem;

        }


        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            Flow flow = new Flow();
            FlowStep newFlowStep = new FlowStep();
            newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;

            flow.FlowSteps.Add(newFlowStep);

            _baseDatawork.Flows.Add(flow);
            _baseDatawork.SaveChanges();
           await  _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            RefreshData();
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
