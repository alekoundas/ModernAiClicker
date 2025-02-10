using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Models;
using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;
using StepinFlow.Views.Pages.FlowStepDetail;
using StepinFlow.Interfaces;

namespace StepinFlow.Views.Pages
{
    public partial class FlowsPage : INavigableView<FlowsViewModel>
    {
        public FlowsViewModel ViewModel { get; }
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly IWindowService _windowService;

        public FlowsPage(FlowsViewModel viewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork, IWindowService windowService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _windowService = windowService;

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();



            viewModel.NavigateToFlowStepTypeSelectionPage += NavigateToFlowStepTypeSelectionPage;
        }


        public void NavigateToFlowStepTypeSelectionPage(FlowStep flowStep)
        {

            NewSelectTypeFlowStepViewModel detailViewModel = new NewSelectTypeFlowStepViewModel(_baseDatawork);
            detailViewModel.FlowStep = flowStep;

            UIFlowStepTypeSelectionFrame.Navigate(new NewSelectTypeFlowStepPage(detailViewModel, ViewModel, _systemService, _templateMatchingService, _baseDatawork, _windowService));
        }

        private void OnSelectedFlowStepIdChange(object sender, int id)
        {
            ViewModel.TreeViewItemSelectedCommand.Execute(id);
        }

        private void OnFlowStepClone(object sender, int id)
        {
            ViewModel.CoppiedFlowStepId = id;
        }

        private void OnAddFlowStepClick(object sender, FlowStep newFlowStep)
        {
            NavigateToFlowStepTypeSelectionPage(newFlowStep);
        }
    }
}
