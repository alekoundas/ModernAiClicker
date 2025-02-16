using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowDetailVM:ObservableObject, IFlowDetailVM
    {
        private readonly IBaseDatawork _baseDatawork;
        [ObservableProperty]
        protected Flow _flow = new Flow();
        public BaseFlowDetailVM(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        public virtual async Task LoadFlowId(int flowId)
        {
            Flow? flow = await _baseDatawork.Flows.FirstOrDefaultAsync(x => x.Id == flowId);
            if (flow != null)
                Flow = flow;
        }

        public void LoadNewFlow(Flow newFlow)
        {
            Flow = newFlow;
        }
    }
}
