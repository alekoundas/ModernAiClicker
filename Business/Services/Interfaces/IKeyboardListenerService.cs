using Business.Services;

namespace Business.Services.Interfaces
{
    public interface IKeyboardListenerService
    {
        void RegisterListener(KeyCombination combination, Action action);

        void UnregisterListener(KeyCombination combination);
        void UnregisterAllListeners();
    }
}
