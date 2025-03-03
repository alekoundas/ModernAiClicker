using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Model.Enums;
using System.Collections.ObjectModel;
using Business.Interfaces;
using Business.Services.Interfaces;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class SubFlowStepExecutionVM : ObservableObject, IExecutionViewModel
    {
        private readonly IDataService _dataService;
        private readonly ICloneService _cloneService;

        [ObservableProperty]
        private bool _isEnabled;
        [ObservableProperty]
        private ObservableCollection<Flow> _subFlows = new ObservableCollection<Flow>();
        [ObservableProperty]
        private Flow? _selectedSubFlow = null;

        [ObservableProperty]
        private Execution _execution;


        public SubFlowStepExecutionVM(IDataService dataService, ICloneService cloneService)
        {
            _dataService = dataService;
            _cloneService = cloneService;
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
            SubFlows = new ObservableCollection<Flow>(_dataService.Flows.Query.Where(x => x.Type == FlowTypesEnum.SUB_FLOW).ToList());

            if (execution.FlowStep.SubFlowId.HasValue)
                SelectedSubFlow = SubFlows.Where(x => x.Id == execution.FlowStep.SubFlowId).FirstOrDefault();
        }



        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

    }
}
