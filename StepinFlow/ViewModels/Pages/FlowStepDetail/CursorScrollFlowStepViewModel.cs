using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Wpf.Ui.Controls;

namespace StepinFlow.ViewModels.Pages
{
    public partial class CursorScrollFlowStepViewModel : ObservableObject, INavigationAware
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private FlowStep _flowStep;


        [ObservableProperty]
        private IEnumerable<MouseScrollDirectionEnum> _mouseScrollDirectionEnum;


        public CursorScrollFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;


            MouseScrollDirectionEnum = Enum.GetValues(typeof(MouseScrollDirectionEnum)).Cast<MouseScrollDirectionEnum>();
        }

        public void OnNavigatedFrom()
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo()
        {
            throw new NotImplementedException();
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
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FindAsync(FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.MouseScrollDirectionEnum = FlowStep.MouseScrollDirectionEnum;
                updateFlowStep.MouseLoopTimes = FlowStep.MouseLoopTimes;

            }

            /// Add mode
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else
                    isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(FlowStep.FlowId.Value);

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();


                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Set cursor Action.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            await _flowsViewModel.RefreshData();
        }
    }
}
