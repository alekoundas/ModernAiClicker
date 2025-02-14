using System.Windows.Media.Imaging;

namespace StepinFlow.Interfaces
{
    public interface IWindowService
    {
        Task<byte[]?> OpenScreenshotSelectionWindow(byte[]? image = null, bool allowSave = true);

        void CloseScreenshotSelectionWindow();
    }
}
