using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class WaitForTemplateExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public WaitForTemplateExecutionPage(WaitForTemplateExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }

        //public void SetViewModel(IExecutionViewModel executionViewModel)
        //{
        //    ViewModel.ShowResultImage -= ShowResultImage;
        //    ViewModel = (WaitForTemplateExecutionViewModel)executionViewModel;
        //    ViewModel.ShowResultImage += ShowResultImage;
        //    DataContext = ViewModel;
        //}

        //public void ShowResultImage(string filePath)
        //{
        //    try
        //    {
        //        BitmapImage bitmap = new BitmapImage();
        //        bitmap.BeginInit();
        //        bitmap.CacheOption = BitmapCacheOption.None;
        //        bitmap.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
        //        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //        bitmap.UriSource = new Uri(filePath);
        //        bitmap.EndInit();
        //        this.UIResultImage.Source = bitmap;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }

        //}
    }
}
