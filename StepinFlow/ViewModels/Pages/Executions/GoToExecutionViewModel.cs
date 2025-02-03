using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.Extensions;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class GoToExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps;

        public GoToExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;

            _execution = new Execution();
            PreviousSteps = GetParents();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }

        private ObservableCollection<FlowStep> GetParents()
        {
            if (Execution?.FlowStep?.ParentFlowStepId == null)
                return new ObservableCollection<FlowStep>();

            // Get recursively all parents.
            List<FlowStep> parents = _baseDatawork.FlowSteps
                .GetAll()
                .First(x => x.Id == Execution.FlowStep.ParentFlowStepId)
                .SelectRecursive<FlowStep>(x => x.ParentFlowStep)
                .ToList();

            parents = parents
                .Where(x => x != null && x.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
                .ToList();

            return new ObservableCollection<FlowStep>(parents);

        }
    }
}
