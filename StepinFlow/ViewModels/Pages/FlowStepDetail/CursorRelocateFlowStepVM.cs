using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Model.Enums;
using Model.Structs;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class CursorRelocateFlowStepVM : BaseFlowStepDetailVM
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsVM _flowsViewModel;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        [ObservableProperty]
        private FlowStep? _selectedFlowStep = null;


        public CursorRelocateFlowStepVM(FlowsVM flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork) : base(baseDatawork)
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
                SelectedFlowStep = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == flowStep.ParentTemplateSearchFlowStepId);

                if (FlowStep.ParentTemplateSearchFlowStepId.HasValue)
                    GetParents(FlowStep.ParentTemplateSearchFlowStepId.Value);

                if (FlowStep.ParentFlowStepId.HasValue)
                    GetParents(FlowStep.ParentFlowStepId.Value);
            }
        }

        public override async Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            FlowStep = newFlowStep;

            if (FlowStep.ParentTemplateSearchFlowStepId.HasValue)
                GetParents(FlowStep.ParentTemplateSearchFlowStepId.Value);

            if (FlowStep.ParentFlowStepId.HasValue)
                GetParents(FlowStep.ParentFlowStepId.Value);

            return;
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
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;

                if (SelectedFlowStep != null)
                    updateFlowStep.ParentTemplateSearchFlowStepId = SelectedFlowStep.Id;
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
                    FlowStep.Name = "Set cursor possition.";

                if (SelectedFlowStep != null)
                    FlowStep.ParentTemplateSearchFlowStepId = SelectedFlowStep.Id;

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            _flowsViewModel.RefreshData();
        }

        private void GetParents(int? flowStepId)
        {
            Parents = new ObservableCollection<FlowStep>();
            if (!flowStepId.HasValue)
                return;

            FlowStep? parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == flowStepId.Value);

            while (parent != null)
            {
                if (parent.Type == FlowStepTypesEnum.TEMPLATE_SEARCH)
                    Parents.Add(parent);


                if (parent.Type == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH)
                    Parents.Add(parent);


                if (parent.Type == FlowStepTypesEnum.WAIT_FOR_TEMPLATE)
                    Parents.Add(parent);

                if (!parent.ParentFlowStepId.HasValue)
                    return;

                parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == parent.ParentFlowStepId.Value);
            }
        }
    }
}
