﻿using Business.Helpers;
using Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class FlowExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IDataService _dataService;
        private readonly ISystemService _systemService;

        public FlowExecutionWorker(IDataService dataService, ISystemService systemService) : base(dataService, systemService)
        {
            _dataService = dataService;
            _systemService = systemService;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            // Should never execute.
            throw new NotImplementedException();
        }

        public async override Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            FlowStep? nextFlowStep = await _dataService.FlowSteps.Query
                .Where(x=>x.FlowId == execution.FlowId)
                .SelectMany(x=>x.ChildrenFlowSteps)
                .Where(x => x.Type != FlowStepTypesEnum.NEW)
                .OrderBy(x=>x.OrderingNum)
                .FirstOrDefaultAsync();

            var nextFlowStep2 = await _dataService.FlowSteps.Query
              .Where(x => x.FlowId == execution.FlowId).ToListAsync();

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

            await _dataService.UpdateAsync(execution);
            Directory.CreateDirectory(folderPath);
        }
    }
}
