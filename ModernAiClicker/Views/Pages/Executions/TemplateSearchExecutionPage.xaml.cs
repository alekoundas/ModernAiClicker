using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.IO;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class TemplateSearchExecutionPage : Page, IExecutionPage
    {
        public TemplateSearchExecutionViewModel ViewModel { get; set; }

        public TemplateSearchExecutionPage()
        {
            ViewModel = new TemplateSearchExecutionViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (TemplateSearchExecutionViewModel)executionViewModel;
            DataContext = ViewModel;

            //Image display in ui
            ViewModel.ShowResultImage += ShowResultImage;
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
