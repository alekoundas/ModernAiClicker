using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.Extensions;
using Business.BaseViewModels;
using Microsoft.EntityFrameworkCore;

namespace StepinFlow.ViewModels.Pages
{
    public partial class GoToFlowStepViewModel : BaseFlowStepDetailVM
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps = new ObservableCollection<FlowStep>();


        public GoToFlowStepViewModel(FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork) : base(baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowsViewModel = flowsViewModel;

        }

        public override async Task LoadFlowStepId(int flowStepId)
        {
            FlowStep? flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
            if (flowStep != null)
            {
                FlowStep = flowStep;
                PreviousSteps = await GetParents();
            }
        }

        public override async Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            FlowStep = newFlowStep;
            PreviousSteps = await GetParents();
        }

      

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            // Edit mode
            if (FlowStep.Id > 0)
            {

            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else if (FlowStep.FlowId.HasValue)
                    isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(FlowStep.FlowId.Value);
                else
                    return;

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Run again an earlier step.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }

            _baseDatawork.SaveChanges();
            _flowsViewModel.RefreshData();
        }

        private async Task<ObservableCollection<FlowStep>> GetParents()
        {
            List<FlowStep> previousSteps = new List<FlowStep>();
            var queue = new Queue<FlowStep>();
            if (FlowStep?.Id != 0)
            {
                List<FlowStep> siblings = await _baseDatawork.FlowSteps.GetSiblings(FlowStep.Id);
                foreach (FlowStep step in siblings)
                    queue.Enqueue(step);
            }
            else
            {
                queue.Enqueue(FlowStep);
                if (FlowStep.ParentFlowStepId != null)
                {
                    List<FlowStep> siblings = await _baseDatawork.FlowSteps.Query
                        .Where(x => x.Id == FlowStep.ParentFlowStepId.Value)
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
