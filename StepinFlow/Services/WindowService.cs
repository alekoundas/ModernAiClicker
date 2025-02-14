using Microsoft.Extensions.DependencyInjection;
using StepinFlow.Interfaces;
using StepinFlow.Views.Windows;
using System.Windows;

namespace StepinFlow.Services
{
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<byte[]?> OpenScreenshotSelectionWindow(byte[]? image = null, bool allowSave = true)
        {
            TaskCompletionSource<byte[]?> taskCompletionSource = new TaskCompletionSource<byte[]?>();
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = _serviceProvider.GetRequiredService<ScreenshotSelectionWindow>();
                if (window == null)
                {
                    taskCompletionSource.SetResult(null);
                    return;
                }

                // Handle data return when window closes.
                window.Closed += (s, e) => taskCompletionSource.SetResult(window.ViewModel.ResultImage);

                // Initialize view model.
                window.ViewModel.SetScreenshot(image);
                window.ViewModel.ImportVisibility = allowSave ? Visibility.Visible : Visibility.Collapsed;
                
                // Show window.
                window.ShowDialog();
            });

            return taskCompletionSource.Task;
        }

        public void CloseScreenshotSelectionWindow()
        {
            var window = Application.Current.Windows
                .OfType<ScreenshotSelectionWindow>()
                .FirstOrDefault(x => x.IsActive);

            if (window != null)
                window.Close();
        }
    }
}
