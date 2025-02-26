using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowStepDetailVM : ObservableObject, IFlowStepDetailVM
    {
        private readonly IDataService _dataService;
        public virtual event Action<int> OnSave;


        [ObservableProperty]
        protected FlowStep _flowStep = new FlowStep();
        public BaseFlowStepDetailVM(IDataService dataService)
        {
            _dataService = dataService;
        }

        public virtual async Task LoadFlowStepId(int flowStepId)
        {
            FlowStep? flowStep = await _dataService.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
            if (flowStep != null)
                FlowStep = flowStep;
        }

        public virtual Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            FlowStep = newFlowStep;
            return Task.CompletedTask;
        }

    }
}
