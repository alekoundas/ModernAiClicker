using Business.Helpers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
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
            Expression<Func<FlowStep, bool>> firstFlowStepFilter =
                (x) => x.FlowId == execution.FlowId
                    && x.OrderingNum == 0
                    && x.FlowStepType != FlowStepTypesEnum.IS_NEW;

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(firstFlowStepFilter);

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
            DateTime dateTime = DateTime.Now;
            string filename = "";

            filename += "Execution";
            filename += " - ";
            filename += DateTime.Now.ToString("yy-MM-dd hh.mm");

            execution.ExecutionFolderDirectory = PathHelper.GetAppDataPath() + "\\" + filename;
            await _baseDatawork.SaveChangesAsync();

            _systemService.CreateFolderOnDisk(filename);
        }
    }
}
