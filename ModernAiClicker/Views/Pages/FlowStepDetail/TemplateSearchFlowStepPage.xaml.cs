﻿using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class TemplateSearchFlowStepPage : Page
    {
        public TemplateSearchFlowStepViewModel ViewModel { get; }

        public TemplateSearchFlowStepPage(TemplateSearchFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();


            //Image display in ui
            //viewModel.ShowTemplateImg += ShowTemplateImg;
            viewModel.ShowResultImage += ShowResultImage;

            //if (viewModel.FlowStep.TemplateImagePath.Length > 0)
            //    ShowTemplateImg(viewModel.FlowStep.TemplateImagePath);
        }


        //public void ShowTemplateImg(string filename)
        //{
        //    BitmapImage bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.CacheOption = BitmapCacheOption.None;
        //    bitmap.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
        //    bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //    bitmap.UriSource = new Uri(filename);
        //    bitmap.EndInit();
        //    this.UITemplateImage.Source = bitmap;
        //}

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
