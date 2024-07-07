using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Drawing;
using System.IO;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class TemplateSearchExecutionPage :  INavigableView<TemplateSearchExecutionViewModel>
    {
        public TemplateSearchExecutionViewModel ViewModel { get; }

        public TemplateSearchExecutionPage(TemplateSearchExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();


            //Image display in ui
            viewModel.ShowTemplateImg += ShowTemplateImg;
            viewModel.ShowResultImage += ShowResultImage;

            //if (viewModel.Execution.TemplateImagePath.Length > 0)
            //    ShowTemplateImg(viewModel.Execution.TemplateImagePath);
        }


        public void ShowTemplateImg(string filename)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(filename);
            bitmap.EndInit();
            this.UITemplateImage.Source = bitmap;
        }

        public void ShowResultImage(byte[] imageArray)
        {
                BitmapImage bitmap = new BitmapImage();
            using (var ms = new System.IO.MemoryStream(imageArray))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
            }

            this.UIResultImage.Source = bitmap;
        }
    }
}
