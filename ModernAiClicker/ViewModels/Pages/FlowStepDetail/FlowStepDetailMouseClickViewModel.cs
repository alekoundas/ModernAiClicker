using Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Model.Models;
using Business.Interfaces;
using Model.Structs;
using Business.Helpers;
using Model.Business;
using Model.Enums;
using DataAccess.Repository.Interface;
using Force.DeepCloner;
using System.Windows.Navigation;
using System.Windows.Forms;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowStepDetailMouseClickViewModel : ObservableObject
    {
        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);

        private readonly ISystemService _systemService;
        private readonly ITemplateMatchingService _templateMatchingService;
        private readonly IFlowService _flowService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        private bool _isInitialized = false;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private string _templateImgPath = "";

        [ObservableProperty]
        private FlowStep _flowStep;

        public event ShowTemplateImgEvent? ShowTemplateImg;
        public delegate void ShowTemplateImgEvent(string filePath);

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public FlowStepDetailMouseClickViewModel(FlowStep flowStep,FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateMatchingService templateMatchingService, IFlowService flowService, IBaseDatawork baseDatawork) 
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _flowService = flowService;

            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;
        }

        [RelayCommand]
        private void OnButtonOpenFileClick()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = PathHelper.GetAppDataPath();
            openFileDialog.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                TemplateImgPath = openFileDialog.FileName;
                ShowTemplateImg?.Invoke(openFileDialog.FileName);
            }

        }


        [RelayCommand]
        private void OnButtonTestClick()
        {
            Rectangle processWindowRectangle;

            if (FlowStep.ProcessName != null && TemplateImgPath != null)
            {
                processWindowRectangle = _systemService.GetWindowSize(FlowStep.ProcessName);



                TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(TemplateImgPath, processWindowRectangle);

                int x = result.ResultRectangle.Left;
                int y = result.ResultRectangle.Top;

                int width = result.ResultRectangle.Right - x;
                int height = result.ResultRectangle.Bottom - y;



                var aaa = new System.Drawing.Rectangle(x, y, width, height);


                var hwnd = Screen.FromRectangle(aaa);






                ShowResultImage?.Invoke(result.ResultImagePath);
            }

        }

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async void OnButtonSaveClick()
        {
            if (FlowStep.ProcessName != null && TemplateImgPath != null)
            {
                FlowStep newFlowStep = FlowStep.CreateModel();
                newFlowStep.IsNew = false;

                _baseDatawork.FlowSteps.Add(newFlowStep);
                _baseDatawork.SaveChanges();
                //RefreshData();

                _flowsViewModel.RefreshData();
                //NavigationService.GetNavigationService().Source.
            }

            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            //refreshtest?.Invoke();

        }
    }
}
