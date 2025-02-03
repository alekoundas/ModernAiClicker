using Business.Extensions;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;

namespace Business.Factories.Workers
{
    public class CommonExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public CommonExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task<Execution> CreateExecutionModelFlow(int flowId, Execution? _)
        {
            Execution execution = new Execution();
            execution.FlowId = flowId;
            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            return execution;
        }

        public async virtual Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution parentExecution)
        {
            if (parentExecution == null)
                throw new ArgumentNullException(nameof(parentExecution));

            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;

            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            execution.FlowStep = flowStep;
            return execution;
        }

        public async virtual Task SetExecutionModelStateRunning(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;
            execution.LoopCount += 1;

            if (execution.ParentExecution != null)
                execution.ExecutionFolderDirectory = execution.ParentExecution.ExecutionFolderDirectory;

            await _baseDatawork.SaveChangesAsync();
        }

        public async virtual Task SetExecutionModelStateComplete(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.COMPLETED;
            execution.EndedOn = DateTime.Now;

            await _baseDatawork.SaveChangesAsync();
        }

        public virtual Task ExpandAndSelectFlowStep(Execution execution, ObservableCollection<Flow> treeviewFlows)
        {
            if (execution.FlowStep == null)
                return Task.CompletedTask;

            //Application.Current.Dispatcher.Invoke(() =>
            //{
                // Code to update ObservableCollection
                FlowStep? uiFlowStep = treeviewFlows.First()
                    .Descendants()
                    .FirstOrDefault(x => x.Id == execution.FlowStepId);

                if (uiFlowStep != null)
                {
                    uiFlowStep.IsExpanded = true;
                    uiFlowStep.IsSelected = true;
                }

                if (uiFlowStep?.ParentFlowStep != null)
                    uiFlowStep.ParentFlowStep.IsExpanded = true;
                if (uiFlowStep?.Flow != null)
                    uiFlowStep.Flow.IsExpanded = true;
            //});

            return Task.CompletedTask;
        }

        public async virtual Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            return await Task.FromResult<FlowStep?>(null);
        }


        public async Task SaveToJson()
        {
            await _systemService.UpdateFlowsJSON(await _baseDatawork.Flows.GetAllAsync());
        }

        public async virtual Task SaveToDisk(Execution execution)
        {
            await _baseDatawork.SaveChangesAsync();
        }

        public void ClearEntityFrameworkChangeTracker()
        {
            _baseDatawork.Query.ChangeTracker.Clear();
        }
    }
}
