using Business.Interfaces;
using DataAccess.Repository.Interface;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class MultipleTemplateSearchLoopExecutionPage : Page, IExecutionPage
    {
        public MultipleTemplateSearchLoopExecutionViewModel ViewModel { get; set; }

        public MultipleTemplateSearchLoopExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new MultipleTemplateSearchLoopExecutionViewModel(baseDatawork);
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel.ShowResultImage -= ShowResultImage;
            ViewModel = (MultipleTemplateSearchLoopExecutionViewModel)executionViewModel;
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
