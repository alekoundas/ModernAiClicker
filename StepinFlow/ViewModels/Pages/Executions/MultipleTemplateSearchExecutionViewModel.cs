using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class MultipleTemplateSearchExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;
        private readonly IBaseDatawork _baseDatawork;

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);
        [ObservableProperty]
        private ObservableCollection<FlowStep>? _childrenTemplateSearchFlowSteps;
        public MultipleTemplateSearchExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            
            _execution = new Execution(); 
        }

        public async void  SetExecution(Execution execution)
        {
            execution = await _baseDatawork.Executions.Query
                .Include(x=>x.FlowStep.ParentTemplateSearchFlowStep.ChildrenTemplateSearchFlowSteps)
                .FirstAsync(x=>x.Id==execution.Id);

            List<FlowStep> flowSteps = execution.FlowStep.ParentTemplateSearchFlowStep.ChildrenTemplateSearchFlowSteps
              .Where(x => x.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
              .ToList();

            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
            Execution = execution;

            if (execution.ResultImagePath != null)
                ShowResultImage?.Invoke(execution.ResultImagePath);
        }
    }
}

