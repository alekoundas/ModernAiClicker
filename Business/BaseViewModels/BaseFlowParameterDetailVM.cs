using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowParameterDetailVM : ObservableObject, IFlowParameterDetailVM
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        protected FlowParameter _flowParameter = new FlowParameter();
        public BaseFlowParameterDetailVM(IDataService dataService)
        {
            _dataService = dataService;
        }

        public virtual async Task LoadFlowParameterId(int flowParameterId)
        {
            FlowParameter? flowParameter = await _dataService.FlowParameters.FirstOrDefaultAsync(x => x.Id == flowParameterId);
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
