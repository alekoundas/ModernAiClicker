using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class CursorMoveExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        public CursorMoveExecutionViewModel(IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _execution = new Execution() { FlowStep = new FlowStep() };
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
            FlowStep? flowStep = _baseDatawork.FlowSteps.Query
                .Where(x => x.Id == Execution.FlowStepId)
                .Select(x => x.ParentTemplateSearchFlowStep)
                .FirstOrDefault();

            if (flowStep != null)
                Parents = new ObservableCollection<FlowStep>() { flowStep };

        }

    }
}
