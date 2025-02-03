using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;

namespace StepinFlow.ViewModels.Pages
{
    public partial class SleepFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private FlowStep _flowStep;

        [ObservableProperty]
        private string _timeTotal;
        public SleepFlowStepViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _flowsViewModel = flowsViewModel;
            _flowStep = flowStep;

            int miliseconds = 0;
            if (FlowStep.SleepForMilliseconds.HasValue)
                miliseconds += FlowStep.SleepForMilliseconds.Value;

            if (FlowStep.SleepForSeconds.HasValue)
                miliseconds += FlowStep.SleepForSeconds.Value * 1000;

            if (FlowStep.SleepForMinutes.HasValue)
                miliseconds += FlowStep.SleepForMinutes.Value * 60 * 1000;

            if (FlowStep.SleepForHours.HasValue)
                miliseconds += FlowStep.SleepForHours.Value * 60 * 60 * 1000;

            TimeTotal = TimeSpan.FromMilliseconds(miliseconds).ToString(@"hh\:mm\:ss");
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
                updateFlowStep.SleepForHours = FlowStep.SleepForHours;
                updateFlowStep.SleepForMinutes = FlowStep.SleepForMinutes;
                updateFlowStep.SleepForSeconds = FlowStep.SleepForSeconds;
                updateFlowStep.SleepForMilliseconds = FlowStep.SleepForMilliseconds;

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
                    FlowStep.Name = "Wait";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            await _flowsViewModel.RefreshData();
        }
    }
}
