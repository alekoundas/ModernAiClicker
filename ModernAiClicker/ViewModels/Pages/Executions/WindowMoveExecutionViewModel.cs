using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using Model.Structs;
using Business.Helpers;
using Model.Business;
using DataAccess.Repository.Interface;
using System.Windows.Forms;
using Model.Enums;
using System.Collections.ObjectModel;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class WindowMoveExecutionViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        public WindowMoveExecutionViewModel(Execution execution, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _execution = execution;
        }
    }
}
