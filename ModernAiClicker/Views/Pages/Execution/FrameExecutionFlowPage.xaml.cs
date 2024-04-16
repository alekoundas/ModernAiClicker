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
    public partial class FrameExecutionFlowPage : Page
    {
        private readonly IBaseDatawork _baseDatawork;
        public FrameExecutionFlowViewModel ViewModel { get; }

        public FrameExecutionFlowPage(FrameExecutionFlowViewModel viewModel, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

        }
    }
}
