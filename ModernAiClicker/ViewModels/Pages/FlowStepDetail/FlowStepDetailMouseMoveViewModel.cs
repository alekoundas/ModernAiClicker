using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using Model.Structs;
using Business.Helpers;
using Model.Business;
using DataAccess.Repository.Interface;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Business.Extensions;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowStepDetailMouseMoveViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateMatchingService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private bool _isInitialized = false;

        [ObservableProperty]
        private FlowStep _flowStep;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents;

        public FlowStepDetailMouseMoveViewModel(FlowStep flowStep, ISystemService systemService, ITemplateMatchingService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            FlowStep = flowStep;
            Parents = GetParents();

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

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }

        private ObservableCollection<FlowStep> GetParents()
        {
            if (FlowStep.ParentFlowStepId == null)
                return new ObservableCollection<FlowStep>();


            List<FlowStep> parents = _baseDatawork.FlowSteps
                .GetAll()
                .First(x => x.Id == FlowStep.ParentFlowStepId)
                .SelectRecursive<FlowStep>(x => x.ParentFlowStep)
                .ToList();

            return new ObservableCollection<FlowStep>(parents);

        }

    }
}
