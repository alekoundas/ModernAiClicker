using Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class TemplateSearchExecutionViewModel : ObservableObject, INavigationAware
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        public event ShowTemplateImgEvent? ShowTemplateImg;
        public delegate void ShowTemplateImgEvent(string filePath);

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public TemplateSearchExecutionViewModel(Execution execution, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            _execution = execution;

            if (execution.FlowStep != null)
            {

                ShowTemplateImg?.Invoke(execution.FlowStep.TemplateImagePath);
                ShowResultImage?.Invoke(execution.ResultImagePath);

            }

        }

        public void OnNavigatedTo()
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom()
        {
            throw new NotImplementedException();
        }
    }
}

