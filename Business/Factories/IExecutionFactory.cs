using Model.Enums;

namespace Business.Factories
{
    public interface IExecutionFactory
    {
        IExecutionWorker GetWorker(FlowStepTypesEnum? flowStep);
        void SetCancellationToken(CancellationTokenSource cancellationToken);
        void DestroyWorkers();
    }
}
