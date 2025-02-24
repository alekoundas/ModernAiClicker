using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowStepDetailVM : ObservableObject, IFlowStepDetailVM
    {
        private readonly IBaseDatawork _baseDatawork;
        public virtual event Action<int> OnSave;


        [ObservableProperty]
        protected FlowStep _flowStep = new FlowStep();
        public BaseFlowStepDetailVM(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        public virtual async Task LoadFlowStepId(int flowStepId)
        {
            FlowStep? flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
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
