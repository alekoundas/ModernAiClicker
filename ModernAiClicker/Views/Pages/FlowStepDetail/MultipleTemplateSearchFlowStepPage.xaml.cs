using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class MultipleTemplateSearchFlowStepPage : Page
    {
        public MultipleTemplateSearchFlowStepViewModel ViewModel { get; }

        public MultipleTemplateSearchFlowStepPage(MultipleTemplateSearchFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();


            //Image display in ui
            viewModel.ShowResultImage += ShowResultImage;

        }

        public void ShowResultImage(string filename)
        {
            if (filename.Length > 0)
            {

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri(filename);
                bitmap.EndInit();
                this.UIResultImage.Source = bitmap;
            }
            else
            {
                this.UIResultImage.Source = null;

            }
        }
    }
}
