using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class MultipleTemplateSearchLoopExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;
        private readonly IBaseDatawork _baseDatawork;

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        [ObservableProperty]
        private ObservableCollection<FlowStep>? _childrenTemplateSearchFlowSteps;

        public MultipleTemplateSearchLoopExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _execution = new Execution();
        }

        public async void SetExecution(Execution execution)
        {
            execution = await _baseDatawork.Executions.Query
                .Include(x=>x.FlowStep.ParentTemplateSearchFlowStep.ChildrenTemplateSearchFlowSteps)
                .FirstAsync(x => x.Id == execution.Id);

            Execution = execution;

            List<FlowStep> flowSteps = execution.FlowStep.ParentTemplateSearchFlowStep.ChildrenTemplateSearchFlowSteps
                .Where(x => x.FlowStepType == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD)
                .ToList();

            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);

            if (execution.ResultImagePath != null)
                ShowResultImage?.Invoke(execution.ResultImagePath);
        }
    }
}

