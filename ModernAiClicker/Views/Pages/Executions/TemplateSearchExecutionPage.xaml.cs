using ModernAiClicker.ViewModels.Pages.Executions;
using System.Net.Cache;
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
            //viewModel.ShowTemplateImg += ShowTemplateImg;
            viewModel.ShowResultImage += ShowResultImage;

            if (viewModel.Execution.ResultImagePath!= null)
                ShowResultImage(viewModel.Execution.ResultImagePath);
        }


        //public void ShowTemplateImg(string filename)
        public void ShowResultImage(string filePath)
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
    }
}
