using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using System.Collections.ObjectModel;
using DataAccess.Repository.Interface;
using Business.Factories;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FrameExecutionFlowViewModel : ObservableObject
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IExecutionFactory _executionFactory;

        [ObservableProperty]
        private Flow? _flow;


        public FrameExecutionFlowViewModel(IBaseDatawork baseDatawork, IExecutionFactory executionFactory)
        {
            _baseDatawork = baseDatawork;
            _executionFactory = executionFactory;
        }


        [RelayCommand]
        private async Task OnButtonStartClick()
        {
            if (Flow == null)
                return;

            IExecutionWorker flowWorker = _executionFactory.GetWorker(null);
            Execution? firstExecutionStep =  await flowWorker.GetNextStep(Flow.Id);

            //Cant execute flow because no executable steps found!
            if(firstExecutionStep == null)
                return;

            //Should never hapen.
            if (firstExecutionStep.FlowStep == null)
                return;


            IExecutionWorker flowStepWorker = _executionFactory.GetWorker(firstExecutionStep.FlowStep);
            await flowStepWorker.ExecuteFlowStepAction(firstExecutionStep);




        }


        private void whatever()
        {

        }


        [RelayCommand]
        private void OnButtonStopClick()
        {
        }

    }
}
