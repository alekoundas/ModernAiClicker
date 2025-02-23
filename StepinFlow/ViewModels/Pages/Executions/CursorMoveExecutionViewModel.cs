using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Model.Enums;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class CursorMoveExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        public CursorMoveExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            _execution = new Execution() { FlowStep = new FlowStep() };
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
            Execution = _baseDatawork.Executions.Query
                .Where(x => x.Id == Execution.Id)
                .Include(x => x.FlowStep.ParentTemplateSearchFlowStep)
                .First();

            GetParents(execution.FlowStepId);
        }

        private void GetParents(int? flowStepId)
        {
            if (!flowStepId.HasValue)
                return;

            FlowStep? parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == flowStepId.Value);

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

                parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == parent.ParentFlowStepId.Value);
            }
        }
    }
}
