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
        private readonly ITemplateMatchingService _templateMatchingService;
        private readonly IFlowService _flowService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;
        public FlowStepDetailNewSelectTypeViewModel ViewModel { get; }

        public FlowStepDetailNewSelectTypePage(FlowStepDetailNewSelectTypeViewModel viewModel, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateMatchingService templateMatchingService, IFlowService flowService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _flowService = flowService;
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

                FlowStepDetailTemplateMatchingViewModel viewModel = new FlowStepDetailTemplateMatchingViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _flowService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new FlowStepDetailTemplateMatchingPage(viewModel));

                //this.UIFlowStepDetailFrame.RaiseEvent();
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_CLICK)
            {

                FlowStepDetailMouseClickViewModel viewModel = new FlowStepDetailMouseClickViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _flowService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new FlowStepDetailMouseClickPage(viewModel));

                //this.UIFlowStepDetailFrame.RaiseEvent();
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
