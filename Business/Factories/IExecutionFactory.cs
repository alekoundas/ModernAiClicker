using Model.Models;

namespace Business.Factories
{
    public interface IExecutionFactory
    {
        IExecutionWorker GetWorker(FlowStep? flowStep);

    }
}
