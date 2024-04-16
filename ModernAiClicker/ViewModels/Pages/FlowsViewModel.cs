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

        public FlowsViewModel(IFlowService flowService, IBaseDatawork baseDatawork) : base(baseDatawork)
        {
            FlowService = flowService;
            _baseDatawork = baseDatawork;

            RefreshData();
        }



        [RelayCommand]
        private void TreeViewItem_OnButtonNewClick(EventParammeters eventParameters)
        {
            if (eventParameters.Value == null || eventParameters.SecondValue == null)
                return;

            bool isFlowIdParsable = Int32.TryParse(eventParameters.Value.ToString(), out int flowId);
            bool isFlowStepIdParsable = Int32.TryParse(eventParameters.SecondValue.ToString(), out int flowStepId);

            if (isFlowIdParsable && isFlowStepIdParsable)
            {
                // search for FlowStep based on id
                FlowStep flowStep = FlowsList
                  .SelectMany(x => x.FlowSteps)
                  .Where(x => x.Id == flowStepId && x.Flow.Id == flowId)
                  .First();

                NavigateToFlowStepTypeSelectionPage?.Invoke(flowStep);
            }
        }

        [RelayCommand]
        private void OnTreeViewItemButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.Value == null || eventParameters.SecondValue == null)
                return;

            bool isFlowIdParsable = Int32.TryParse(eventParameters.Value.ToString(), out int flowId);
            bool isFlowStepIdParsable = Int32.TryParse(eventParameters.SecondValue.ToString(), out int flowStepId);

            //if (isFlowIdParsable && isFlowStepIdParsable)
            //{
            //    FlowService.RemoveFlowStep(flowId, flowStepId);
            //}
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
