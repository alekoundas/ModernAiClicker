using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Repository.Interface;
using Model.Business;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Business.Services
{
    public class FlowService : IFlowService
    {
        private readonly IBaseDatawork _baseDataWork;
        private readonly ITemplateMatchingService _templateMatchingService;
        private readonly ISystemService _systemService;

        public FlowService(IBaseDatawork baseDatawork, ITemplateMatchingService templateMatchingService, ISystemService systemService)
        {
            _baseDataWork = baseDatawork;
            _templateMatchingService = templateMatchingService;
            _systemService = systemService;

        }

        public void StartFlow(Flow flow)
        {
            if (flow.Execution == null) return;

            flow.Execution.Status = ExecutionStatusEnum.RUNNING;
            flow.Execution.RunFor = "0";

            FlowStep nextFlowStep = FindNextStep(flow);
            if (nextFlowStep != null)
                ExecuteStep(nextFlowStep);

        }

        public void ContinueFlow(Flow flow)
        {

        }

        public void InitializeExecutionModels(Flow flow)
        {
            flow.Execution = new Execution();
            flow.Execution.IsSelected = true;

            foreach (var flowStep in flow.FlowSteps)
            {
                flowStep.Execution = new Execution();
            }
        }



        private FlowStep? FindNextStep(Flow flow)
        {
            FlowStep nextFlowStep = flow.FlowSteps
                .Where(x => x.IsNew == false)
                .Where(x => x.Execution?.IsExecuted == false)
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            return nextFlowStep;
        }

        private void ExecuteStep(FlowStep flowStep)
        {
            if (flowStep.FlowStepType == FlowStepTypesEnum.TEMPLATE_SEARCH)
            {
                Rectangle screenshotRect = new Rectangle();

                if (flowStep.ProcessName.Length > 0)
                    screenshotRect = _systemService.GetWindowSize(flowStep.ProcessName);
                else
                    screenshotRect = _systemService.GetScreenSize();






                TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(flowStep.TemplateImagePath, screenshotRect);

                int x = result.ResultRectangle.Left;
                int y = result.ResultRectangle.Top;

                int width = result.ResultRectangle.Right - x;
                int height = result.ResultRectangle.Bottom - y;



                var aaa = new System.Drawing.Rectangle(x,y,width,height);


                var hwnd = Screen.FromRectangle(aaa);

                var xx= screenshotRect.Left + result.ResultRectangle.Left;
                var yy= screenshotRect.Top+ result.ResultRectangle.Top;

                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(-1875,1375);


                _systemService.SetCursorPossition(result.ResultRectangle.Top, result.ResultRectangle.Left);
            }
        }

    }
}
