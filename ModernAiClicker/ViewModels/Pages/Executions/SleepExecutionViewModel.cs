using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class SleepExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;


        public SleepExecutionViewModel()
        {
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }
    }
}
