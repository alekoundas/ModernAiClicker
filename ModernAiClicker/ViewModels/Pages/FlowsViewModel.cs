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

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowsViewModel : BaseViewModel, INavigationAware
    {
        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);


        private Flow? _selectedFlow;
        private FlowStep? _selectedFlowStep;

        public readonly IFlowService FlowService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public FlowsViewModel(IFlowService flowService, IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork)
        {
            FlowService = flowService;
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            RefreshData();
        }



        [RelayCommand]
        private void TreeViewItem_OnButtonNewClick(EventParammeters eventParameters)
        {
            FlowStep flowStep = new FlowStep();

            // If flowId is available
            if (eventParameters.Value != null)
            {
                bool isFlowIdParsable = Int32.TryParse(eventParameters.Value.ToString(), out int flowId);

                if (isFlowIdParsable)
                    flowStep.FlowId = flowId;
            }
            // If flowStepId is available
            else if (eventParameters.SecondValue != null)
            {
                bool isFlowStepIdParsable = Int32.TryParse(eventParameters.SecondValue.ToString(), out int flowStepId);


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
        private async Task OnTreeViewItemButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.Value == null || eventParameters.SecondValue == null)
                return;

            bool isFlowIdParsable = Int32.TryParse(eventParameters.Value.ToString(), out int flowId);
            bool isFlowStepIdParsable = Int32.TryParse(eventParameters.SecondValue.ToString(), out int flowStepId);

            if (isFlowIdParsable && isFlowStepIdParsable)
            {
                FlowStep flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.FlowId == flowId && x.id == flowStepId);
                _baseDatawork.FlowSteps.Remove(flowStep);
                _baseDatawork.SaveChanges();


                _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

                RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowDeleteButtonClick(EventParammeters eventParameters)
        {
            if (eventParameters.Value == null)
                return;

            bool isFlowIdParsable = Int32.TryParse(eventParameters.Value.ToString(), out int flowId);
            if (isFlowIdParsable)
            {
                Flow flow = await _baseDatawork.Flows.FirstOrDefaultAsync(x => x.Id == flowId);
                _baseDatawork.Flows.Remove(flow);
                _baseDatawork.SaveChanges();


                _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

                RefreshData();
            }
        }


        [RelayCommand]
        private void TreeViewItem_OnItemSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
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
        private void ButtonAddClick()
        {
            Flow flow = new Flow();
            FlowStep newFlowStep = new FlowStep() { IsNew = true };

            flow.FlowSteps.Add(newFlowStep);

            _baseDatawork.Flows.Add(flow);
            _baseDatawork.SaveChanges();
            _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            RefreshData();
        }


        [RelayCommand]
        private void ButtonEditClick(RoutedPropertyChangedEventArgs<object> eventArgs)
        {
        }

        [RelayCommand]
        private void OnButtonOpenFileClick(object eventArgs)
        {
        }

        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
