using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using StepinFlow.Interfaces;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class TemplateSearchExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IWindowService _windowService;

        [ObservableProperty]
        private Execution _execution;
        [ObservableProperty]
        private byte[]? _resultImage = null;

        public TemplateSearchExecutionViewModel(IWindowService windowService)
        {
            _windowService = windowService;
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;

            if (execution.ResultImagePath != null)
                ResultImage = File.ReadAllBytes(execution.ResultImagePath);
        }

        [RelayCommand]
        private async Task OnTemplateImageDoubleClick(MouseButtonEventArgs e)
        {

            // Check if it's a double-click.
            if (e.ClickCount == 2)
            {
                if (e.Source is System.Windows.Controls.Image img && img.Source is BitmapSource bitmapSource)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(stream);
                        byte[]? image = await _windowService.OpenScreenshotSelectionWindow(stream.ToArray(), false);
                        if (image != null)
                            using (var stream2 = new MemoryStream(image))
                            {
                                var decoder = BitmapDecoder.Create(stream2, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                                img.Source = decoder.Frames[0];
                            }
                    }
                }

            }
        }

        [RelayCommand]
        private async Task OnResultImageDoubleClick(MouseButtonEventArgs e)
        {
            // Check if it's a double-click.
            if (e.ClickCount == 2)
                await _windowService.OpenScreenshotSelectionWindow(ResultImage, false);
        }


        [RelayCommand]
        private void OnImageFailed(object sender)
        {
            //if (sender is Image img)
            //{
            //    img.Source = null; // This prevents the error
            //}
        }
    }
}

