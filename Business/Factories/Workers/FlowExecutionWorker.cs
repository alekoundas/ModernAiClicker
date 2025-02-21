using Business.Helpers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class FlowExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public FlowExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            // Should never execute.
            throw new NotImplementedException();
        }

        public async override Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.Query
                .Where(x=>x.FlowId == execution.FlowId)
                .Where(x => x.Type != FlowStepTypesEnum.NEW)
                .OrderBy(x=>x.OrderingNum)
                .FirstOrDefaultAsync();

            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            // Flow steps dont have siblings
            return await Task.FromResult<FlowStep?>(null);
        }

        public async override Task SaveToDisk(Execution execution)
        {
            string folderName = "Execution - " + DateTime.Now.ToString("yy-MM-dd hh.mm");
            string folderPath = Path.Combine(PathHelper.GetExecutionHistoryDataPath(), folderName);

            execution.ExecutionFolderDirectory = folderPath;

            await _baseDatawork.SaveChangesAsync();
            Directory.CreateDirectory(folderPath);
        }
    }
}
