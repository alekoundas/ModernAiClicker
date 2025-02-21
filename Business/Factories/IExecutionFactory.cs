using Model.Enums;

namespace Business.Factories
{
    public interface IExecutionFactory
    {
        IExecutionWorker GetWorker(TypesEnum? flowStep);
        void SetCancellationToken(CancellationTokenSource cancellationToken);
        void DestroyWorkers();
    }
}
