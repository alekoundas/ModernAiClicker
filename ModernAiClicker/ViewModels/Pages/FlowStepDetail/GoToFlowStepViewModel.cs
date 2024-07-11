using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.Extensions;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class GoToFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private FlowStep _flowStep;


        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps;
        public GoToFlowStepViewModel(FlowStep flowStep, ISystemService systemService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _flowStep = flowStep;
            PreviousSteps = GetParents();
        }
        private ObservableCollection<FlowStep> GetParents()
        {
            if (FlowStep?.ParentFlowStepId == null)
                return new ObservableCollection<FlowStep>();

            // Get recursively all parents.
            List<FlowStep> parents = _baseDatawork.FlowSteps
                .GetAll()
                .First(x => x.Id == FlowStep.ParentFlowStepId)
                .SelectRecursive<FlowStep>(x => x.ParentFlowStep)
                .ToList();

            // Add Siblings needs fix
            parents.AddRange( parents.SelectMany(x => x.ChildrenFlowSteps));

            parents = parents.OrderByDescending(x=>x.Id)
                .ToList();

            return new ObservableCollection<FlowStep>(parents);

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
                if (FlowStep.ParentFlowStepId != null)
                {
                    FlowStep isNewSimpling = _baseDatawork.FlowSteps
                        .Where(x => x.Id == FlowStep.ParentFlowStepId)
                        .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }
                else
                {
                    FlowStep isNewSimpling = _baseDatawork.Flows
                        .Where(x => x.Id == FlowStep.FlowId)
                        .Select(x => x.FlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Run again an earlier step.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }

            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }
    }
}
