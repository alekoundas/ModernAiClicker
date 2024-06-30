using Business.Interfaces;
using Business.Services;
using DataAccess;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using ModernAiClicker.CustomEvents;
using ModernAiClicker.ViewModels.Pages;
using System.Windows;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class FlowStepDetailNewSelectTypePage : Page
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;
        public FlowStepDetailNewSelectTypeViewModel ViewModel { get; }

        public FlowStepDetailNewSelectTypePage(FlowStepDetailNewSelectTypeViewModel viewModel, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _flowsViewModel = flowsViewModel;


            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            viewModel.NavigateToFlowStepDetailPage += NavigateToFlowStepDetailPage;

        }

        public void NavigateToFlowStepDetailPage(FlowStep flowStep)
        {

            if (flowStep.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
            {
                FlowStepDetailTemplateSearchViewModel viewModel = new FlowStepDetailTemplateSearchViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new FlowStepDetailTemplateSearchPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_CLICK)
            {
                FlowStepDetailMouseClickViewModel viewModel = new FlowStepDetailMouseClickViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new FlowStepDetailMouseClickPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_MOVE_COORDINATES)
            {
                FlowStepDetailMouseMoveViewModel viewModel = new FlowStepDetailMouseMoveViewModel(flowStep, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new FlowStepDetailMouseMovePage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.SLEEP)
            {
                FlowStepDetailSleepViewModel viewModel = new FlowStepDetailSleepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new FlowStepDetailSleepPage(viewModel));
            }

        }

        public static readonly RoutedEvent MyCustomEvent = EventManager.RegisterRoutedEvent(
           "MyCustom", // Event name
           RoutingStrategy.Bubble, // Bubble means the event will bubble up through the tree
           typeof(RoutedEventHandler), // The event type
           typeof(ChildFrameEvents)); // Belongs to ChildControlBase

        // Allows add and remove of event handlers to handle the custom event
        public event RoutedEventHandler MyCustom
        {
            add { AddHandler(MyCustomEvent, value); }
            remove { RemoveHandler(MyCustomEvent, value); }
        }

        private void HandleButtonClick(object sender, RoutedEventArgs e)
        {
            // This actually raises the custom event
            var newEventArgs = new RoutedEventArgs(MyCustomEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
