using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class MultipleTemplateSearchExecutionPage : Page, IExecutionPage
    {
        public MultipleTemplateSearchExecutionViewModel ViewModel { get; set; }

        public MultipleTemplateSearchExecutionPage()
        {
            ViewModel = new MultipleTemplateSearchExecutionViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel.ShowResultImage -= ShowResultImage;
            ViewModel = (MultipleTemplateSearchExecutionViewModel)executionViewModel;
            ViewModel.ShowResultImage += ShowResultImage;
            DataContext = ViewModel;

            //Image display in ui
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
