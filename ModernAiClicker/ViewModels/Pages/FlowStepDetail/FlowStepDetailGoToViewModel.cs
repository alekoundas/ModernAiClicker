using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowStepDetailGoToViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private FlowStep _flowStep;


        public FlowStepDetailGoToViewModel(FlowStep flowStep, ISystemService systemService,  IBaseDatawork baseDatawork) 
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _flowStep = flowStep;
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
            if (FlowStep.Id > 0)
            {

            }

            /// Add mode
            else
            {
                if (FlowStep.ParentFlowStepId != null)
                {
                    FlowStep isNewSimpling = _baseDatawork.FlowSteps
                        .Where(x => x.Id == FlowStep.ParentFlowStepId)
                        .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }
                else
                {
                    FlowStep isNewSimpling = _baseDatawork.Flows
                        .Where(x => x.Id == FlowStep.FlowId)
                        .Select(x => x.FlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }

                _baseDatawork.FlowSteps.Add(FlowStep);
            }

            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }
    }
}
