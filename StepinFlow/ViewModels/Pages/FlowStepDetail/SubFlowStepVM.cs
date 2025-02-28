using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using DataAccess.Repository.Interface;
using Model.Enums;
using Business.BaseViewModels;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Business.Interfaces;

namespace StepinFlow.ViewModels.Pages
{
    public partial class SubFlowStepVM : BaseFlowStepDetailVM
    {
        private readonly IDataService _dataService;
        private readonly ICloneService _cloneService;
        public override event Action<int> OnSave;

        [ObservableProperty]
        private bool _isEnabled;
        [ObservableProperty]
        private ObservableCollection<Flow> _subFlows = new ObservableCollection<Flow>();
        [ObservableProperty]
        private Flow? _selectedSubFlow = null;


        public SubFlowStepVM(IDataService dataService, ICloneService cloneService) : base(dataService)
        {
            _dataService = dataService;
            _cloneService = cloneService;
        }


        public override async Task LoadFlowStepId(int flowStepId)
        {
            IsEnabled = false;
            SubFlows = new ObservableCollection<Flow>(await _dataService.Flows.Query.Where(x => x.Type == FlowTypesEnum.SUB_FLOW).ToListAsync());

            FlowStep? flowStep = await _dataService.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
            if (flowStep != null)
                FlowStep = flowStep;

            if (FlowStep.SubFlowId.HasValue)
                SelectedSubFlow = SubFlows.Where(x => x.Id == flowStep.SubFlowId).FirstOrDefault();

        }

        public override async Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            SubFlows = new ObservableCollection<Flow>(await _dataService.Flows.Query.Where(x => x.Type == FlowTypesEnum.SUB_FLOW).ToListAsync());
            FlowStep = newFlowStep;
            IsEnabled = true;
        }



        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            _dataService.Query.ChangeTracker.Clear();
            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _dataService.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.IsSubFlowReferenced = FlowStep.IsSubFlowReferenced;
                updateFlowStep.SubFlowId = SelectedSubFlow?.Id;
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



                _dataService.FlowSteps.Add(FlowStep);
            }


            _dataService.SaveChanges();
            OnSave?.Invoke(FlowStep.Id);
        }
    }
}
