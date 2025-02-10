using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class TemplateSearchLoopExecutionPage : Page, IExecutionPage
    {
        public TemplateSearchLoopExecutionViewModel ViewModel { get; set; }

        public TemplateSearchLoopExecutionPage()
        {
            ViewModel = new TemplateSearchLoopExecutionViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel.ShowResultImage -= ShowResultImage;
            ViewModel = (TemplateSearchLoopExecutionViewModel)executionViewModel;
            ViewModel.ShowResultImage += ShowResultImage;
            DataContext = ViewModel;
        }

        public void ShowResultImage(string filePath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();
                this.UIResultImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }
}
