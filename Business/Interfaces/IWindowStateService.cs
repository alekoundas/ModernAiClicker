using Model.Structs;

namespace Business.Interfaces
{
    public interface IWindowStateService
    {
        WindowSize GetMainWindowState();
        WindowSize GetSelectorWindowState();
        void SaveMainWindowState(WindowSize windowState);
        void SaveSelectorWindowState(WindowSize windowState);
    }
}
