using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class MultipleTemplateSearchExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;
        private readonly IBaseDatawork _baseDatawork;

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public MultipleTemplateSearchExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            
            _execution = new Execution(); 
        }

        public async void  SetExecution(Execution execution)
        {
            execution = await _baseDatawork.Executions.Query
                .Include(x=>x.FlowStep.ChildrenTemplateSearchFlowSteps)
                .FirstAsync(x=>x.Id==execution.Id);

            Execution = execution;


            if (execution.ResultImagePath != null)
                ShowResultImage?.Invoke(execution.ResultImagePath);
        }
    }
}

