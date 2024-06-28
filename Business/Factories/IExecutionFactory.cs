using Model.Enums;
using Model.Models;

namespace Business.Factories
{
    public interface IExecutionFactory
    {
        IExecutionWorker GetWorker(FlowStepTypesEnum? flowStep);

    }
}
