using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using System.Collections.ObjectModel;
using DataAccess.Repository.Interface;
using Business.Factories;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class FrameExecutionFlowViewModel : ObservableObject
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IExecutionFactory _executionFactory;

        public Flow? Flow;

        public ObservableCollection<Execution> Executions;
        public bool StopExecution { get; set; }



        public FrameExecutionFlowViewModel(IBaseDatawork baseDatawork, IExecutionFactory executionFactory)
        {
            _baseDatawork = baseDatawork;
            _executionFactory = executionFactory;
        }
    }
}
