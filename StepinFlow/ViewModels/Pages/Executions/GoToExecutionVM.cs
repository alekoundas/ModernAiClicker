using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using Model.Enums;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Business.Services.Interfaces;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class GoToExecutionVM : ObservableObject, IExecutionViewModel
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps = new ObservableCollection<FlowStep>();

        public GoToExecutionVM(IDataService dataService)
        {
            _dataService = dataService;
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
            Task.Run(async()=> PreviousSteps = await  GetParents());
        }

        private async Task<ObservableCollection<FlowStep>> GetParents()
        {
            List<FlowStep> previousSteps = new List<FlowStep>();
            var queue = new Queue<FlowStep>();
            if (Execution.FlowStep?.Id != 0)
            {
                List<FlowStep> siblings = await _dataService.FlowSteps.GetSiblings(Execution.FlowStep.Id);
                foreach (FlowStep step in siblings)
                    queue.Enqueue(step);
            }
            else
            {
                queue.Enqueue(Execution.FlowStep);
                if (Execution.FlowStep.ParentFlowStepId != null)
                {
                    List<FlowStep> siblings = await _dataService.FlowSteps.Query
                        .Where(x => x.Id == Execution.FlowStep.ParentFlowStepId.Value)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .ToListAsync();

                    foreach (var sibling in siblings)
                        queue.Enqueue(sibling);
                }
            }


            while (queue.Count > 0)
            {
                FlowStep currentFlowStep = queue.Dequeue();
                previousSteps.Add(currentFlowStep);

                if (currentFlowStep.ParentFlowStepId != null)
                {
                    int parentOrderingNum = _dataService.FlowSteps.Where(x => x.Id == currentFlowStep.ParentFlowStepId.Value).Select(x => x.OrderingNum).First();
                    List<FlowStep> parentSiblings = await _dataService.FlowSteps.GetSiblings(currentFlowStep.ParentFlowStepId.Value);
                    List<FlowStep> parentSiblingsAbove = parentSiblings.Where(x => x.OrderingNum <= parentOrderingNum).ToList();

                    foreach (var sibling in parentSiblingsAbove)
                        queue.Enqueue(sibling);
                }
            }

            List<FlowStep> previousStepsFiltered = previousSteps
           .Where(x => x.Type != FlowStepTypesEnum.SUCCESS)
           .Where(x => x.Type != FlowStepTypesEnum.FAILURE)
           .Where(x => x.Type != FlowStepTypesEnum.NEW)
           .Where(x => x.Type != FlowStepTypesEnum.GO_TO)
           .Distinct() // TODO: Fix query and remove this.
           .ToList();


            return new ObservableCollection<FlowStep>(previousStepsFiltered);
        }
    }
}
