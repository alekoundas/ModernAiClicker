using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.Extensions;
using Microsoft.EntityFrameworkCore;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class GoToExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps = new ObservableCollection<FlowStep>();

        public GoToExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
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
                List<FlowStep> siblings = await _baseDatawork.FlowSteps.GetSiblings(Execution.FlowStep.Id);
                foreach (FlowStep step in siblings)
                    queue.Enqueue(step);
            }
            else
            {
                queue.Enqueue(Execution.FlowStep);
                if (Execution.FlowStep.ParentFlowStepId != null)
                {
                    List<FlowStep> siblings = await _baseDatawork.FlowSteps.Query
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
                    int parentOrderingNum = _baseDatawork.FlowSteps.Where(x => x.Id == currentFlowStep.ParentFlowStepId.Value).Select(x => x.OrderingNum).First();
                    List<FlowStep> parentSiblings = await _baseDatawork.FlowSteps.GetSiblings(currentFlowStep.ParentFlowStepId.Value);
                    List<FlowStep> parentSiblingsAbove = parentSiblings.Where(x => x.OrderingNum <= parentOrderingNum).ToList();

                    foreach (var sibling in parentSiblingsAbove)
                        queue.Enqueue(sibling);
                }
            }

            List<FlowStep> previousStepsFiltered = previousSteps
           .Where(x => x.Type != TypesEnum.IS_SUCCESS)
           .Where(x => x.Type != TypesEnum.IS_FAILURE)
           .Where(x => x.Type != TypesEnum.IS_NEW)
           .Where(x => x.Type != TypesEnum.GO_TO)
           .Distinct() // TODO: Fix query and remove this.
           .ToList();


            return new ObservableCollection<FlowStep>(previousStepsFiltered);
        }
    }
}
