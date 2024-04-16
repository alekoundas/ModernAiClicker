using Model.Models;
using System.Collections.ObjectModel;

namespace Business.Interfaces
{
    public interface IFlowService
    {
        void InitializeExecutionModels(Flow flow);
        void StartFlow(Flow flow);
        void ContinueFlow(Flow flow);

    }
}