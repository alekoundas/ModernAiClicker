using Model.Models;
using CommunityToolkit.Mvvm.Input;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ModernAiClicker.ViewModels.UserControls;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowsViewModel : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly TreeViewUserControlViewModel _treeViewUserControlViewModel;

        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);

        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        public FlowsViewModel(IBaseDatawork baseDatawork, ISystemService systemService, TreeViewUserControlViewModel treeViewUserControlViewModel)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _treeViewUserControlViewModel = treeViewUserControlViewModel;

            Task.Run(async () => await RefreshData());
        }



        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            await _treeViewUserControlViewModel.AddNewFlow();
        }


        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;
            _treeViewUserControlViewModel.IsLocked = IsLocked;

        }

        public async Task RefreshData()
        {
            await _treeViewUserControlViewModel.LoadFlows();

        }

        [RelayCommand]
        private async Task OnButtonSyncClick()
        {
            await _treeViewUserControlViewModel.LoadFlows();
        }

        [RelayCommand]
        private async Task OnButtonExpandAllClick()
        {
            await _treeViewUserControlViewModel.ExpandAll();
        }

        [RelayCommand]
        private async Task OnButtonCollapseAllClick()
        {
            await _treeViewUserControlViewModel.CollapseAll();
        }



        [RelayCommand]
        private async Task OnTreeViewItemSelected(int id)
        {

            FlowStep? flowStep = await _baseDatawork.FlowSteps.Query
                .Include(x => x.ChildrenTemplateSearchFlowSteps)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (flowStep != null)
                NavigateToFlowStepTypeSelectionPage?.Invoke(flowStep);
        }

        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
