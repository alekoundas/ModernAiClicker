using Business.Interfaces;
using DataAccess;
using DataAccess.Repository.Interface;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages
{
    public partial class FlowsPage : INavigableView<FlowsViewModel>
    {
        public FlowsViewModel ViewModel { get; }
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;


        public FlowsPage(FlowsViewModel viewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();



            viewModel.NavigateToFlowStepTypeSelectionPage += NavigateToFlowStepTypeSelectionPage;
        }


        public void NavigateToFlowStepTypeSelectionPage(FlowStep flowStep)
        {

            NewSelectTypeFlowStepViewModel detailViewModel = new NewSelectTypeFlowStepViewModel(_baseDatawork);
            detailViewModel.FlowStep = flowStep;

            UIFlowStepTypeSelectionFrame.Navigate(new NewSelectTypeFlowStepPage(detailViewModel, ViewModel, _systemService, _templateMatchingService, _baseDatawork));
        }

    }
}
