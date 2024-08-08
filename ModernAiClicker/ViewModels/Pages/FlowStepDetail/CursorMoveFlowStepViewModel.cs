using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Business.Extensions;
using Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class CursorMoveFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private FlowStep _flowStep;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        public CursorMoveFlowStepViewModel(FlowStep flowStep, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            FlowStep = flowStep;
            Parents = _parents;

            Task.Run(() => GetParentsRecursively(FlowStep.ParentFlowStepId.Value)).Wait();
        }

        [RelayCommand]
        private void OnComboBoxSelectionChanged()
        {
            //TODO
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
                    FlowStep.Name = "Set cursor possition.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }

        private async Task GetParentsRecursively(int flowStepId)
        {
            FlowStep parent = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);

            if (parent.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
                Parents.Add(parent);

            if (!parent.ParentFlowStepId.HasValue)
                return;

            await GetParentsRecursively(parent.ParentFlowStepId.Value);
        }
    }
}
