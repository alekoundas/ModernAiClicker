using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories
{
    public interface IExecutionWorker
    {
        Task<Execution> CreateExecutionModel(int id, int? parentExecutionId);
        Task ExecuteFlowStepAction(Execution execution);
        Task<FlowStep?> GetNextChildFlowStep(Execution execution);
        Task<FlowStep?> GetNextSiblingFlowStep(Execution execution);
        Task SetExecutionModelStateRunning(Execution execution);
        Task SetExecutionModelStateComplete(Execution execution);
        void ExpandAndSelectFlowStep(Execution execution);

    }
}
