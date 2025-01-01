using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
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

        public NewSelectTypeFlowStepPage(
            NewSelectTypeFlowStepViewModel viewModel,
            FlowsViewModel flowsViewModel,
            ISystemService systemService,
            ITemplateSearchService templateMatchingService,
            IBaseDatawork baseDatawork
            )
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
            else if (flowStep.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP)
            {
                TemplateSearchLoopFlowStepViewModel viewModel = new TemplateSearchLoopFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new TemplateSearchLoopFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH)
            {
                MultipleTemplateSearchFlowStepViewModel viewModel = new MultipleTemplateSearchFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new MultipleTemplateSearchFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP)
            {
                MultipleTemplateSearchLoopFlowStepViewModel viewModel = new MultipleTemplateSearchLoopFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _templateMatchingService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new MultipleTemplateSearchLoopFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_CLICK)
            {
                CursorClickFlowStepViewModel viewModel = new CursorClickFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new CursorClickFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_MOVE_COORDINATES)
            {
                CursorMoveFlowStepViewModel viewModel = new CursorMoveFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new CursorMoveFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.SLEEP)
            {
                SleepFlowStepViewModel viewModel = new SleepFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new SleepFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.GO_TO)
            {
                GoToFlowStepViewModel viewModel = new GoToFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new GoToFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.WINDOW_RESIZE)
            {
                WindowResizeFlowStepViewModel viewModel = new WindowResizeFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new WindowResizeFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.WINDOW_MOVE)
            {
                WindowMoveFlowStepViewModel viewModel = new WindowMoveFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new WindowMoveFlowStepPage(viewModel));
            }
            else if (flowStep.FlowStepType == FlowStepTypesEnum.MOUSE_SCROLL)
            {
                CursorScrollFlowStepViewModel viewModel = new CursorScrollFlowStepViewModel(flowStep, _flowsViewModel, _systemService, _baseDatawork);
                this.UIFlowStepDetailFrame.Navigate(new CursorScrollFlowStepPage(viewModel));
            }
        }
    }
}
