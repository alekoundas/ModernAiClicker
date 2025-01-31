using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Models;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using Wpf.Ui.Controls;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

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
