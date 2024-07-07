using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Business.Extensions;
using Model.Enums;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class CursorMoveExecutionViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        public CursorMoveExecutionViewModel(Execution execution, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _execution = execution;
        }

    }
}
