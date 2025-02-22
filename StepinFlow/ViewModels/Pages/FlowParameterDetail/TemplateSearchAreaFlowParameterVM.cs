using CommunityToolkit.Mvvm.Input;
using Model.Models;
using DataAccess.Repository.Interface;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages.FlowParameterDetail
{
    public partial class TemplateSearchAreaFlowParameterVM : BaseFlowParameterDetailVM
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly FlowsViewModel _flowsViewModel;


        public TemplateSearchAreaFlowParameterVM(FlowsViewModel flowsViewModel, IBaseDatawork baseDatawork) : base(baseDatawork)
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
            if (FlowParameter.Id > 0)
            {
                Flow updateFlow = await _baseDatawork.Flows.FirstAsync(x => x.Id == FlowParameter.Id);
                updateFlow.Name = FlowParameter.Name;
            }

            // Add mode
            else
            {
                FlowParameter isNewSimpling;

                if (FlowParameter.ParentFlowParameterId != null)
                    isNewSimpling = await _baseDatawork.FlowParameters.GetIsNewSibling(FlowParameter.ParentFlowParameterId.Value);
                else
                    return;

                FlowParameter.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();

                if (FlowParameter.Name.Length == 0)
                    FlowParameter.Name = "Template search area parameter.";

                _baseDatawork.FlowParameters.Add(FlowParameter);
            }


            _baseDatawork.SaveChanges();
             _flowsViewModel.RefreshData();
        }
    }
}
