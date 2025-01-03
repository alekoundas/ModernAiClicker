using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class LoopExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        public LoopExecutionViewModel()
        {
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }
    }
}

