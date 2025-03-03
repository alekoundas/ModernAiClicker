using Business.Interfaces;
using Business.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;

namespace Business.BaseViewModels
{
    public partial class BaseFlowDetailVM : ObservableObject, IFlowDetailVM
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

        public int GetCurrentEntityId()
        {
            return Flow.Id;
        }


        public virtual async Task OnCancel()
        {
            if (Flow.Id == 0)
                Flow = new Flow();
            else
                await LoadFlowId(Flow.Id);
        }

        public virtual void OnPageExit()
        {

        }

        public virtual Task OnSave()
        {
            throw new NotImplementedException();
        }
    }
}
