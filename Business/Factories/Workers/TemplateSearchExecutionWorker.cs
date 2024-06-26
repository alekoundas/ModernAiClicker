using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Business;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System.Windows.Forms;

namespace Business.Factories.Workers
{
    public class TemplateSearchExecutionWorker : IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ITemplateSearchService _templateSearchService;
        private readonly ISystemService _systemService;

        public TemplateSearchExecutionWorker(IBaseDatawork baseDatawork, ITemplateSearchService templateSearchService, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _templateSearchService = templateSearchService;
            _systemService = systemService;
        }

        public Task<Execution?> GetNextStep(int id)
        {
            throw new NotImplementedException();
        }


        public async Task ExecuteFlowStepAction(Execution execution)
        {
            FlowStep? flowStep = execution.FlowStep;

            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            if (flowStep?.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
            {
                Rectangle searchRectangle = new Rectangle();

                if (flowStep.ProcessName.Length > 0)
                    searchRectangle = _systemService.GetWindowSize(flowStep.ProcessName);
                else
                    searchRectangle = _systemService.GetScreenSize();


                TemplateMatchingResult result = _templateSearchService.SearchForTemplate(flowStep.TemplateImagePath, searchRectangle);


                _systemService.SetCursorPossition(result.ResultRectangle.Top, result.ResultRectangle.Left);

                int x = searchRectangle.Left;
                int y = searchRectangle.Top;

                x = x + result.ResultRectangle.Top;
                y = y + result.ResultRectangle.Left;

                _systemService.SetCursorPossition(x, y);

                execution.Status = ExecutionStatusEnum.COMPLETED;
                await _baseDatawork.SaveChangesAsync();
            }

        }


    }
}
