using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Business.Extensions;
using Microsoft.EntityFrameworkCore;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class CursorMoveExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents;

        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        public CursorMoveExecutionViewModel(IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _execution = new Execution();

            Parents = new ObservableCollection<FlowStep>();

            //TODO get result from parent flowstep execution
            if (Execution.ResultLocationX.HasValue && Execution.ResultLocationY.HasValue)
            {
                X = Execution.ResultLocationX.Value;
                Y = Execution.ResultLocationY.Value;
            }
        }
        public void SetExecution(Execution execution)
        {
            Execution = execution;
            FlowStep? flowStep = _baseDatawork.FlowSteps.Query
                .Include(x => x.ParentTemplateSearchFlowStep)
                .FirstOrDefault(x => x.Id == Execution.FlowStepId);

            if (flowStep != null)
                Parents = new ObservableCollection<FlowStep>() { flowStep.ParentTemplateSearchFlowStep };

        }

        [RelayCommand]
        private async Task OnButtonTestClick()
        {
            //Execution templateSearchExecution = GetExecution();

            //_systemService.SetCursorPossition(new Point(Execution.ResultLocationX.Value, Execution.ResultLocationY.Value));
        }

        private Execution GetExecution()
        {
            FlowStep templateSearchFlowStep = Execution.FlowStep.ParentTemplateSearchFlowStep;
            if (templateSearchFlowStep == null)
            {

            }

            // Get recursively all parents of execution.
            List<Execution> parents = _baseDatawork.Executions
                .GetAll()
                .First(x => x.Id == Execution.Id)
                .SelectRecursive<Execution>(x => x.ParentExecution)
                .ToList();

            Execution templateSearchExecution = parents.FirstOrDefault(x => x.FlowStepId == templateSearchFlowStep.Id);

            return templateSearchExecution;
        }


        private List<FlowStep> GetParents(int? flowStepId)
        {
            if (!flowStepId.HasValue)
                return new List<FlowStep>();

            List<FlowStep> parents = new List<FlowStep>();
            FlowStep? parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == flowStepId.Value);

            while (parent != null)
            {
                parents.Add(parent);

                if (!parent.ParentFlowStepId.HasValue)
                    parent = null;
                else
                    parent = _baseDatawork.FlowSteps.FirstOrDefault(x => x.Id == parent.ParentFlowStepId.Value);
            }

            return parents;
        }
    }
}
