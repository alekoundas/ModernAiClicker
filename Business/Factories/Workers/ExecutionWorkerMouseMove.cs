using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories.Workers
{
    public class ExecutionWorkerMouseMove : IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;

        public ExecutionWorkerMouseMove(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        public Task<Execution?> GetNextStep(int id)
        {
            throw new NotImplementedException();
        }


        public async Task ExecuteFlowStepAction(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;


        }


    }
}
