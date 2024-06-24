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

        public event PropertyChangedEventHandler PropertyChanged;


        private ObservableCollection<Flow> _flowsList;
        public ObservableCollection<Flow> FlowsList
        {
            get { return _flowsList; }
            set
            {
                _flowsList = value;
                NotifyPropertyChanged(nameof(FlowsList));
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
            .ToList();

            SortFlows(flows);

            FlowsList = new ObservableCollection<Flow>(flows);
        }

        private void SortFlows(List<Flow> flows)
        {
            foreach (Flow flow in flows)
            {
                if (flow.FlowSteps.Count > 0)
                {
                    flow.FlowSteps.ToList().Sort((x, y) =>
                    {
                        return x.OrderingNum.CompareTo(y.OrderingNum);
                    });
                    SortFlowSteps(flow.FlowSteps.ToList());
                }

            }
        }


        //Recursion on every child of flow step
        private void SortFlowSteps(List<FlowStep> flowSteps)
        {
            foreach (FlowStep flowStep in flowSteps)
            {
                if (flowStep.ChildrenFlowSteps?.Count > 0)
                {
                    flowStep.ChildrenFlowSteps.ToList().Sort((x, y) => y.OrderingNum.CompareTo(x.OrderingNum));
                    SortFlowSteps(flowStep.ChildrenFlowSteps.ToList());
                }
            }
        }
    }
}
