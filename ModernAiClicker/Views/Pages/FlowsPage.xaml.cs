using Business.Interfaces;
using DataAccess;
using DataAccess.Repository.Interface;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using System.Windows;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages
{
    public partial class FlowsPage : INavigableView<FlowsViewModel>
    {
        public FlowsViewModel ViewModel { get; }
        private readonly ISystemService _systemService;
        private readonly ITemplateMatchingService _templateMatchingService;
        private readonly IFlowService _flowService;
        private readonly IBaseDatawork _baseDatawork;


        public FlowsPage(FlowsViewModel viewModel, ISystemService systemService, ITemplateMatchingService templateMatchingService, IFlowService flowService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _flowService = flowService;

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            viewModel.NavigateToFlowStepTypeSelectionPage += NavigateToFlowStepTypeSelectionPage;
            viewModel.refreshtest += aaaa;
        }


        public void NavigateToFlowStepTypeSelectionPage(FlowStep flowStep)
        {
            FlowStepDetailNewSelectTypeViewModel detailViewModel = new FlowStepDetailNewSelectTypeViewModel(_baseDatawork);
            detailViewModel.FlowStep = flowStep;
            //detailViewModel.FlowsList = ViewModel.FlowsList;

            UIFlowStepTypeSelectionFrame.Navigate(new FlowStepDetailNewSelectTypePage(detailViewModel, ViewModel, _systemService, _templateMatchingService, _flowService, _baseDatawork));
        }

        public void aaaa()
        {
            //NavigationService.
        }

        private void HandleChildEvent(object sender, RoutedEventArgs e)
        {
            // Code to handle the event raised from the child
            // Use this line to stop the event bubbling further if you need to
            e.Handled = true;
        }

    }
}
