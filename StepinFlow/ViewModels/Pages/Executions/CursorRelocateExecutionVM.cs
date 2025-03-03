using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Business.Services.Interfaces;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class CursorRelocateExecutionVM : ObservableObject, IExecutionViewModel
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        public CursorRelocateExecutionVM(IDataService dataService)
        {
            _dataService = dataService;
            _execution = new Execution() { FlowStep = new FlowStep() };
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
            Execution = _dataService.Executions.Query
                .Where(x => x.Id == Execution.Id)
                .Include(x => x.FlowStep.ParentTemplateSearchFlowStep)
                .First();

            GetParents(execution.FlowStepId);
        }

        private void GetParents(int? flowStepId)
        {
            if (!flowStepId.HasValue)
                return;

            FlowStep? parent = _dataService.FlowSteps.FirstOrDefault(x => x.Id == flowStepId.Value);

            while (parent != null)
            {
                if (parent.Type == FlowStepTypesEnum.TEMPLATE_SEARCH)
                    Parents.Add(parent);

                if (parent.Type == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH)
                    Parents.Add(parent);

                if (parent.Type == FlowStepTypesEnum.WAIT_FOR_TEMPLATE)
                    Parents.Add(parent);

                if (!parent.ParentFlowStepId.HasValue)
                    return;

                parent = _dataService.FlowSteps.FirstOrDefault(x => x.Id == parent.ParentFlowStepId.Value);
            }
        }
    }
}
