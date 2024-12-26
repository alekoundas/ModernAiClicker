using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class CursorScrollExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private IEnumerable<MouseButtonsEnum> _mouseButtonsEnum;

        [ObservableProperty]
        private IEnumerable<MouseActionsEnum> _mouseActionsEnum;

        public CursorScrollExecutionViewModel() 
        {
            _execution = new Execution();

            MouseButtonsEnum = Enum.GetValues(typeof(MouseButtonsEnum)).Cast<MouseButtonsEnum>();
            MouseActionsEnum = Enum.GetValues(typeof(MouseActionsEnum)).Cast<MouseActionsEnum>();
        }
        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }

    }
}
