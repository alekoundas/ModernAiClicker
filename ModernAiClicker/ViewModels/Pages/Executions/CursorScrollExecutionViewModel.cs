using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class CursorScrollExecutionViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private IEnumerable<MouseButtonsEnum> _mouseButtonsEnum;

        [ObservableProperty]
        private IEnumerable<MouseActionsEnum> _mouseActionsEnum;

        public CursorScrollExecutionViewModel(Execution execution,ISystemService systemService,  IBaseDatawork baseDatawork) 
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _execution= execution;
            MouseButtonsEnum = Enum.GetValues(typeof(MouseButtonsEnum)).Cast<MouseButtonsEnum>();
            MouseActionsEnum = Enum.GetValues(typeof(MouseActionsEnum)).Cast<MouseActionsEnum>();
        }
    }
}
