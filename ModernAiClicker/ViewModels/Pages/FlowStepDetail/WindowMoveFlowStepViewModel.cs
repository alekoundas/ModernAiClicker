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

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class WindowMoveFlowStepViewModel : ObservableObject
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private FlowStep _flowStep;


        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private int _x = 0;

        [ObservableProperty]
        private int _y = 0;

        public WindowMoveFlowStepViewModel(FlowStep flowStep, ISystemService systemService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;

            _flowStep = flowStep;
            X = FlowStep.ResultLocation.X;
            Y = FlowStep.ResultLocation.Y;
        }




        [RelayCommand]
        private void OnButtonRecordClick()
        {
            if (FlowStep.ProcessName.Length <= 1)
                return;

            Rectangle windowRect = _systemService.GetWindowSize(FlowStep.ProcessName);

            X = windowRect.Left;
            Y = windowRect.Top; 
            FlowStep.ResultLocation = new Point(X,Y);
        }

        [RelayCommand]
        private void OnButtonTestClick()
        {
            if (FlowStep.ProcessName.Length <= 1)
                return;

            Rectangle windowRect = _systemService.GetWindowSize(FlowStep.ProcessName);
            Rectangle newWindowRect = new Rectangle();

            int height = Math.Abs(windowRect.Bottom - windowRect.Top);
            int width = Math.Abs(windowRect.Left - windowRect.Right);

            newWindowRect.Left = X;
            newWindowRect.Top = Y;
            newWindowRect.Right = X + width;
            newWindowRect.Bottom = Y + height;

            bool result = _systemService.MoveWindow(FlowStep.ProcessName, newWindowRect);
        }



        [RelayCommand]
        private async Task OnButtonSaveClick()
        {

            FlowStep.ResultLocation = new Point(X, Y);

            // Edit mode
            if (FlowStep.Id > 0)
            {

            }

            /// Add mode
            else
            {
                if (FlowStep.ParentFlowStepId != null)
                {
                    FlowStep isNewSimpling = _baseDatawork.FlowSteps
                        .Where(x => x.Id == FlowStep.ParentFlowStepId)
                        .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }
                else
                {
                    FlowStep isNewSimpling = _baseDatawork.Flows
                        .Where(x => x.Id == FlowStep.FlowId)
                        .Select(x => x.FlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                    FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                    isNewSimpling.OrderingNum++;
                }
                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Set window location.";

                _baseDatawork.FlowSteps.Add(FlowStep);
            }


            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
        }
    }
}
