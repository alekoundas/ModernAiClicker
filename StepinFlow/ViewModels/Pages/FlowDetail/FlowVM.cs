using CommunityToolkit.Mvvm.Input;
using Model.Models;
using DataAccess.Repository.Interface;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class FlowVM : BaseFlowDetailVM
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsVM _flowsViewModel;


        public FlowVM(FlowsVM flowsViewModel, IBaseDatawork baseDatawork) : base(baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _flowsViewModel = flowsViewModel;

        }

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            // Edit mode
            if (Flow.Id > 0)
            {
                Flow updateFlow = await _baseDatawork.Flows.FirstAsync(x => x.Id == Flow.Id);
                updateFlow.Name = Flow.Name;
            }

            /// Add mode
            else
            {
                if (Flow.Name.Length == 0)
                    Flow.Name = "Flow";

                _baseDatawork.Flows.Add(Flow);
            }


            _baseDatawork.SaveChanges();
            _flowsViewModel.RefreshData();
        }
    }
}
