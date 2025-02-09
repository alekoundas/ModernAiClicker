using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class CursorMoveExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;

        [ObservableProperty]
        private Execution _execution;

        [ObservableProperty]
        private ObservableCollection<FlowStep> _parents = new ObservableCollection<FlowStep>();

        public CursorMoveExecutionViewModel(IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _execution = new Execution() { FlowStep = new FlowStep() };
        }

        public void SetExecution(Execution execution)
        {
            Execution = execution;
            Execution = _baseDatawork.Executions.Query
                .Where(x => x.Id == Execution.Id)
                .Include(x => x.FlowStep.ParentTemplateSearchFlowStep)
                .First();


            if (Execution?.FlowStep?.ParentTemplateSearchFlowStep != null)
                Parents = new ObservableCollection<FlowStep>() { Execution.FlowStep.ParentTemplateSearchFlowStep };

        }

    }
}
