using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.Extensions;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class GoToExecutionViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps;

        public GoToExecutionViewModel(Execution execution, ISystemService systemService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _execution = execution;
            PreviousSteps = GetParents();

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
