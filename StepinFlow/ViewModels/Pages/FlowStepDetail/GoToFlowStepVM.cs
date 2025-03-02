﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using DataAccess.Repository.Interface;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.BaseViewModels;
using Microsoft.EntityFrameworkCore;

namespace StepinFlow.ViewModels.Pages
{
    public partial class GoToFlowStepVM : BaseFlowStepDetailVM
    {
        private readonly IDataService _dataService;
        //public override event Action<int> OnSave;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _previousSteps = new ObservableCollection<FlowStep>();


        public GoToFlowStepVM(IDataService dataService) : base(dataService)
        {
            _dataService = dataService;
        }

        public override async Task LoadFlowStepId(int flowStepId)
        {
            FlowStep? flowStep = await _dataService.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
            if (flowStep != null)
            {
                FlowStep = flowStep;
                if (flowStep.ParentFlowStepId != null)
                    PreviousSteps = await GetParents(flowStep.ParentFlowStepId.Value);
            }
        }

        public override async Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            FlowStep = newFlowStep;
            if (newFlowStep.ParentFlowStepId != null)
                PreviousSteps = await GetParents(newFlowStep.ParentFlowStepId.Value);
        }



        public override async Task OnSave()
        {
            _dataService.Query.ChangeTracker.Clear();
            // Edit mode
            if (FlowStep.Id > 0)
            {

            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _dataService.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else if (FlowStep.FlowId.HasValue)
                    isNewSimpling = await _dataService.Flows.GetIsNewSibling(FlowStep.FlowId.Value);
                else
                    return;

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _dataService.SaveChangesAsync();

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Run again an earlier step.";

                _dataService.FlowSteps.Add(FlowStep);
            }

            _dataService.SaveChanges();
            //OnSave?.Invoke(FlowStep.Id);
        }

        private async Task<ObservableCollection<FlowStep>> GetParents(int flowStepId)
        {
            List<FlowStep> previousSteps = new List<FlowStep>();

            FlowStep? parent = _dataService.FlowSteps.Query
                .AsNoTracking()
                .Include(x => x.ParentFlowStep)
                .FirstOrDefault(x => x.Id == flowStepId);

            while (parent != null)
            {

                List<FlowStep> siblings = await _dataService.FlowSteps.GetSiblings(parent.Id);
                siblings = siblings
                    .Where(x => x.OrderingNum < parent.OrderingNum)
                    .Where(x => x.Type != FlowStepTypesEnum.NEW)
                    .Where(x => x.Type != FlowStepTypesEnum.SUCCESS)
                    .Where(x => x.Type != FlowStepTypesEnum.FAILURE)
                    .Where(x => x.Type != FlowStepTypesEnum.FLOW_STEPS)
                    .Where(x => x.Type != FlowStepTypesEnum.FLOW_PARAMETERS)
                    .Where(x => x.Type != FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
                    .OrderByDescending(x => x.OrderingNum)
                    .ToList();

                previousSteps.AddRange(siblings);

                //Get parent flowStep
                if (parent?.ParentFlowStepId != null)
                    parent = _dataService.FlowSteps.Query
                        .AsNoTracking()
                        .Include(x => x.ParentFlowStep)
                        .FirstOrDefault(x => x.Id == parent.ParentFlowStepId);

                //Get parent SubflowStep
                else if (parent?.FlowId != null)
                    parent = _dataService.Flows.Query
                        .AsNoTracking()
                        .Where(x => x.Id == parent.FlowId)
                        .Select(x => x.ParentSubFlowStep)
                        .FirstOrDefault();
                else
                    parent = null;
            }



            return new ObservableCollection<FlowStep>(previousSteps);
        }
    }
}
