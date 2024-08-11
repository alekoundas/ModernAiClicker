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
    public partial class NewSelectTypeFlowStepPage : Page
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;
        public NewSelectTypeFlowStepViewModel ViewModel { get; }

        public NewSelectTypeFlowStepPage(NewSelectTypeFlowStepViewModel viewModel, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
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
                TemplateSearchFlowStepViewModel viewModel = new TemplateSearchFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new TemplateSearchFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_CLICK)
            {
                CursorClickFlowStepViewModel viewModel = new CursorClickFlowStepViewModel(flowStep,  _systemService,  _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new CursorClickFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_MOVE_COORDINATES)
            {
                CursorMoveFlowStepViewModel viewModel = new CursorMoveFlowStepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new CursorMoveFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.SLEEP)
            {
                SleepFlowStepViewModel viewModel = new SleepFlowStepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new SleepFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.GO_TO)
            {
                GoToFlowStepViewModel viewModel = new GoToFlowStepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new GoToFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.WINDOW_RESIZE)
            {
                WindowResizeFlowStepViewModel viewModel = new WindowResizeFlowStepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new WindowResizeFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.WINDOW_MOVE)
            {
                WindowMoveFlowStepViewModel viewModel = new WindowMoveFlowStepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new WindowMoveFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_SCROLL)
            {
                CursorScrollFlowStepViewModel viewModel = new CursorScrollFlowStepViewModel(flowStep, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new CursorScrollFlowStepPage(viewModel));
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
