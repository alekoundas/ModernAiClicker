using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowDetailVM:ObservableObject, IFlowDetailVM
    {
        private readonly IDataService _dataService;
        [ObservableProperty]
        protected Flow _flow = new Flow();
        public BaseFlowDetailVM(IDataService dataService)
        {
            _dataService = dataService;
        }

        public virtual async Task LoadFlowId(int flowId)
        {
            Flow? flow = await _dataService.Flows.FirstOrDefaultAsync(x => x.Id == flowId);
            if (flow != null)
                Flow = flow;
        }

        public void LoadNewFlow(Flow newFlow)
        {
            Flow = newFlow;
        }
    }
}
