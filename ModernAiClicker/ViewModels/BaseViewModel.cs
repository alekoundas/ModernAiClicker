using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModernAiClicker.ViewModels
{
    public partial class BaseViewModel : ObservableObject, INotifyPropertyChanged
    {

        private IBaseDatawork _baseDatawork { get; }

        public new event PropertyChangedEventHandler? PropertyChanged;


        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();
        public ObservableCollection<Flow> FlowsList 
        {
            get { return _flowsList; }
            set
            {
                _flowsList = value;
                NotifyPropertyChanged(nameof(FlowsList));
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                NotifyPropertyChanged(nameof(IsLocked));
            }
        }


        public BaseViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //TODO find a fix for includes
        public void RefreshData()
        {
            List<Flow> flows = _baseDatawork.Query.Flows
                .Include(x => x.FlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps).ThenInclude(x=>x.Executions)
            .ToList();


            FlowsList = new ObservableCollection<Flow>(flows);
        }
    }
}
