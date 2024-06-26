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
        Task<Execution?> GetNextStep(int id);
        Task ExecuteFlowStepAction(Execution execution);
    }
}
