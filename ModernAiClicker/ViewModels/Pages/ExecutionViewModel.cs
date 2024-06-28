using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using ModernAiClicker.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class ExecutionViewModel : ObservableObject, INavigationAware
    {
        public event FrameNavigateToFlowEvent? FrameNavigateToFlow;
        public delegate void FrameNavigateToFlowEvent(Flow flowStep);


        [ObservableProperty]
        private ObservableCollection<Flow> _comboBoxFlows = new ObservableCollection<Flow>();

        [ObservableProperty]
        private ObservableCollection<Flow> _treeviewFlows = new ObservableCollection<Flow>();

        [ObservableProperty]
        private Flow? _comboBoxSelectedFlow;

        private readonly IBaseDatawork _baseDatawork;


        public ExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            ComboBoxFlows = GetFlows();
        }

        private ObservableCollection<Flow> GetFlows()
        {
            List<Flow> flows = _baseDatawork.Flows.GetAll();

            return new ObservableCollection<Flow>(flows);
        }


        [RelayCommand]
        private void OnComboBoxSelectionChanged(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (ComboBoxSelectedFlow == null)
                return;

            Flow flow = _baseDatawork.Flows.Query
                .Include(x => x.FlowSteps)
                .Where(x => x.Id == ComboBoxSelectedFlow.Id)
                .First();

            TreeviewFlows.Clear();
            TreeviewFlows.Add(flow);

            FrameNavigateToFlow?.Invoke(flow);
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom() { }

    }
}
