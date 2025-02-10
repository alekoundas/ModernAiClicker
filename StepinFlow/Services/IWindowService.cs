using System.Windows.Media.Imaging;

namespace StepinFlow.Interfaces
{
    public interface IWindowService
    {
        Task<byte[]?> OpenScreenshotSelectionWindow();
        void CloseScreenshotSelectionWindow();
    }
}
