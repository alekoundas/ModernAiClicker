using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Business.BaseViewModels;
using Model.Enums;

namespace StepinFlow.ViewModels.Pages
{
    public partial class FlowViewModel : BaseFlowDetailVM
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsViewModel _flowsViewModel;


        public FlowViewModel(FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork) : base(baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
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

                    Flow.Type = FlowStepTypesEnum.FLOW;
                _baseDatawork.Flows.Add(Flow);
            }


            _baseDatawork.SaveChanges();
             _flowsViewModel.RefreshData();
        }
    }
}
