using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class TemplateSearchExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        public event ShowTemplateImgEvent? ShowTemplateImg;
        public delegate void ShowTemplateImgEvent(string filePath);

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public TemplateSearchExecutionViewModel()
        {
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;

            if (execution.FlowStep != null)
            {
                ShowTemplateImg?.Invoke(execution.FlowStep.TemplateImagePath);
                if (execution.ResultImagePath != null)
                    ShowResultImage?.Invoke(execution.ResultImagePath);
            }
        }
    }
}

