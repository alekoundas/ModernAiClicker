using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Model.Enums;
using Model.Structs;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class CursorMoveFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private FlowStep _flowStep;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        public CursorMoveFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowsViewModel = flowsViewModel;
            FlowStep = flowStep;

            if (FlowStep.ParentTemplateSearchFlowStepId.HasValue)
                GetParents(FlowStep.ParentTemplateSearchFlowStepId.Value);
        }


        [RelayCommand]
        private void OnButtonTestClick()
        {
            Point point = new Point(FlowStep.LocationX, FlowStep.LocationY);
            _systemService.SetCursorPossition(point);
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
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FindAsync(FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;

                if (FlowStep.ParentTemplateSearchFlowStep != null)
                    updateFlowStep.ParentTemplateSearchFlowStepId = FlowStep.ParentTemplateSearchFlowStep.Id;
            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else
                    isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(FlowStep.FlowId.Value);

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();


                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Set cursor possition.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            await _flowsViewModel.RefreshData();
        }

        private void GetParents(int? flowStepId)
        {
            if (!flowStepId.HasValue)
                return;

            FlowStep? parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == flowStepId.Value);

            while (parent != null)
            {
                if (parent.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
                    Parents.Add(parent);

                if (parent.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP)
                    Parents.Add(parent);

                if (parent.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH)
                    Parents.Add(parent);

                if (parent.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP)
                    Parents.Add(parent);

                if (parent.FlowStepType == FlowStepTypesEnum.WAIT_FOR_TEMPLATE)
                    Parents.Add(parent);

                if (!parent.ParentFlowStepId.HasValue)
                    return;

                parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == parent.ParentFlowStepId.Value);
            }
        }
    }
}
