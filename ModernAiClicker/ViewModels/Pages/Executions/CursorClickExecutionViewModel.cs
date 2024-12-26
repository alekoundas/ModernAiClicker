using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Model.Enums;
using Business.Interfaces;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class CursorClickExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private IEnumerable<MouseButtonsEnum> _mouseButtonsEnum;


        [ObservableProperty]
        private IEnumerable<MouseActionsEnum> _mouseActionsEnum;
        public CursorClickExecutionViewModel() 
        {
            _execution= new Execution();
            MouseButtonsEnum = Enum.GetValues(typeof(MouseButtonsEnum)).Cast<MouseButtonsEnum>();
            MouseActionsEnum = Enum.GetValues(typeof(MouseActionsEnum)).Cast<MouseActionsEnum>();
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
        }
    }
}
