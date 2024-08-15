using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Business.Extensions;
using Model.Enums;
using Model.Structs;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class CursorMoveExecutionViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents;
        [ObservableProperty]
        private int _x;
        [ObservableProperty]
        private int _y;
        public CursorMoveExecutionViewModel(Execution execution, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _execution = execution;

            //TODO fix parent
            Parents = new ObservableCollection<FlowStep>();
            //Parents = GetParents();
            //Execution execution = GetExecution();

            //TODO get result from parent flowstep execution
            if (Execution.ResultLocationX.HasValue && Execution.ResultLocationY.HasValue)
            {
                X = Execution.ResultLocationX.Value;
                Y = Execution.ResultLocationY.Value;
            }
        }
        [RelayCommand]
        private async Task OnButtonTestClick()
        {
            //Execution templateSearchExecution = GetExecution();

            _systemService.SetCursorPossition(new Point(Execution.ResultLocationX.Value, Execution.ResultLocationY.Value));
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

        private async Task<ObservableCollection<FlowStep>> GetParents()
        {
            if (Execution?.FlowStep?.ParentFlowStepId == null)
                return new ObservableCollection<FlowStep>();

            List<FlowStep> parents = new List<FlowStep>();

            // Get recursively all parents.
            await GetParentsRecursivelly(parents, Execution.FlowStep.ParentFlowStepId.Value);

            parents = parents
                .Where(x => x != null && x.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
                .ToList();

            return new ObservableCollection<FlowStep>(parents);

        }

        private async Task GetParentsRecursivelly(List<FlowStep> parents, int flowStepId)
        {
            FlowStep flowStep = await _baseDatawork.FlowSteps
                .FirstOrDefaultAsync(x => x.Id == flowStepId);

            parents.Add(flowStep);

            if (flowStep.ParentFlowStepId.HasValue)
                await GetParentsRecursivelly(parents, flowStep.ParentFlowStepId.Value);
        }
    }
}
