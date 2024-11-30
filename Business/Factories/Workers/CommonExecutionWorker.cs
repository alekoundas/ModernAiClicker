using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Models;
using System.Windows;
using System.Windows.Threading;

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

        public async Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution? parentExecution)
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


        public async Task SaveToJson()
        {
            await _systemService.UpdateFlowsJSON(await _baseDatawork.Flows.GetAllAsync());
        }

        /// <summary>
        /// Refreshes UI manualy because flow execution occurs in a new thread.
        /// </summary>
        public void RefreshUI()
        {
            // DispatcherPriority set to Input, the highest priority
            DispatcherFrame frame = new();
            DispatcherOperationCallback dispatcherOperationCallback = new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                // Stop all processes to make sure the UI update is perform
                Thread.Sleep(100); 
                return null;
            });

            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, dispatcherOperationCallback, null);
            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
        }

        public async Task SaveToDisk(Execution execution)
        {
            await _baseDatawork.SaveChangesAsync();
        }

        public void ClearEntityFrameworkChangeTracker()
        {
            _baseDatawork.Query.ChangeTracker.Clear();
        }
    }
}
