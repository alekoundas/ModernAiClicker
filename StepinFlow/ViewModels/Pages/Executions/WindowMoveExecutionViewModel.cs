using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using Business.Helpers;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class WindowMoveExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        public WindowMoveExecutionViewModel()
        {
            _execution = new Execution();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }
    }
}
