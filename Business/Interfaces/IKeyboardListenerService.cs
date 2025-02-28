using Business.Services;

namespace Business.Interfaces
{
    public interface IKeyboardListenerService
    {
        void RegisterListener(KeyCombination combination, Action action);

        void UnregisterListener(KeyCombination combination);
        void UnregisterAllListeners();
    }
}
