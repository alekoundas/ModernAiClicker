using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowParameterDetailVM : ObservableObject, IFlowParameterDetailVM
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        protected FlowParameter _flowParameter = new FlowParameter();
        public BaseFlowParameterDetailVM(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        public virtual async Task LoadFlowParameterId(int flowParameterId)
        {
            FlowParameter? flowParameter = await _baseDatawork.FlowParameters.FirstOrDefaultAsync(x => x.Id == flowParameterId);
            if (flowParameter != null)
                FlowParameter = flowParameter;
        }

        public virtual Task LoadNewFlowParameter(FlowParameter newFlowParameter)
        {
            FlowParameter = newFlowParameter;
            return Task.CompletedTask;
        }

    }
}
