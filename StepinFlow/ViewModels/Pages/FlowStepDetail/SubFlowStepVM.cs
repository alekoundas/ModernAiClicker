using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using DataAccess.Repository.Interface;
using Model.Enums;
using Business.BaseViewModels;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Business.Interfaces;
using Business.Services;

namespace StepinFlow.ViewModels.Pages
{
    public partial class SubFlowStepVM : BaseFlowStepDetailVM
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ICloneService _cloneService;
        public override event Action<int> OnSave;

        [ObservableProperty]
        private ObservableCollection<Flow> _subFlows = new ObservableCollection<Flow>();
        [ObservableProperty]
        private Flow? _selectedSubFlow = null;


        public SubFlowStepVM(IBaseDatawork baseDatawork, ICloneService cloneService) : base(baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _cloneService = cloneService;
        }


        public override async Task LoadFlowStepId(int flowStepId)
        {
            SubFlows = new ObservableCollection<Flow>(await _baseDatawork.Flows.Query.Where(x => x.Type == FlowTypesEnum.SUB_FLOW).ToListAsync());

            FlowStep? flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
            if (flowStep != null)
                FlowStep = flowStep;

            if (FlowStep.SubFlowId.HasValue)
                SelectedSubFlow = SubFlows.Where(x => x.Id == flowStep.SubFlowId).FirstOrDefault();

        }

        public override async Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            SubFlows = new ObservableCollection<Flow>(await _baseDatawork.Flows.Query.Where(x => x.Type == FlowTypesEnum.SUB_FLOW).ToListAsync());
            FlowStep = newFlowStep;
        }



        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            _baseDatawork.Query.ChangeTracker.Clear();
            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.IsSubFlowReferenced = FlowStep.IsSubFlowReferenced;
                updateFlowStep.SubFlowId = SelectedSubFlow?.Id;
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
                    FlowStep.Name = "Sub-Flow selector.";



                if (FlowStep.IsSubFlowReferenced)
                    FlowStep.SubFlowId = SelectedSubFlow?.Id;
                else
                {
                    if (SelectedSubFlow?.Id != null)
                    {
                        FlowStep.SubFlow = await _cloneService.GetFlowClone(SelectedSubFlow.Id);

                    }
                }



                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            OnSave?.Invoke(FlowStep.Id);
        }
    }
}
