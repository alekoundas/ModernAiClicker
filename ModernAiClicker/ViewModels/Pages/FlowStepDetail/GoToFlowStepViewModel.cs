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
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private FlowStep _flowStep;


        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps;
        public GoToFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowsViewModel = flowsViewModel;   
            _flowStep = flowStep;
            PreviousSteps = GetParents();
        }
        private ObservableCollection<FlowStep> GetParents()
        {
            if (FlowStep?.FlowId != null)
            {
                List<FlowStep> siblings = _baseDatawork.Query.Flows
                    .First(x=>x.Id == FlowStep.FlowId)
                    .FlowSteps
                    .Where(x => x.Id != FlowStep.Id)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_SUCCESS)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_FAILURE)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                    .OrderBy(x => x.Id)
                    .ToList();

                return new ObservableCollection<FlowStep>(siblings);
            }

            if (FlowStep?.ParentFlowStepId != null)
            {
                List<FlowStep> parents = _baseDatawork.FlowSteps
                 .GetAll()
                 .First(x => x.Id == FlowStep.ParentFlowStepId)
                 .SelectRecursive<FlowStep>(x => x.ParentFlowStep)
                 .Where(x => x != null)
                 .ToList();

                List<FlowStep> parentsChildrensFlatten = parents
                    .SelectMany(x => x.ChildrenFlowSteps.Append(x))
                    .OrderBy(y => y.Id)
                    .ToList();

                parentsChildrensFlatten = parentsChildrensFlatten
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_SUCCESS)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_FAILURE)
                    .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                    .ToList();

                return new ObservableCollection<FlowStep>(parentsChildrensFlatten);
            }

            return new ObservableCollection<FlowStep>();
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
                await _flowsViewModel.RefreshData();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }
    }
}
