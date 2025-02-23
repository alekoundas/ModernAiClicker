using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using Business.Helpers;
using Model.Business;
using Model.Enums;
using DataAccess.Repository.Interface;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
using StepinFlow.Interfaces;
using System.Windows.Input;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class TemplateSearchFlowStepViewModel : BaseFlowStepDetailVM
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private readonly IWindowService _windowService;
        private readonly FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private byte[]? _resultImage = null;
        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private IEnumerable<TemplateMatchModesEnum> _matchModes;
        [ObservableProperty]
        private IEnumerable<FlowParameter> _flowParameters;

        public TemplateSearchFlowStepViewModel(
            FlowsViewModel flowsViewModel,
            ISystemService systemService,
            ITemplateSearchService templateMatchingService,
            IBaseDatawork baseDatawork,
            IWindowService windowService) : base(baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;
            _windowService = windowService;
            _flowsViewModel = flowsViewModel;

            MatchModes = Enum.GetValues(typeof(TemplateMatchModesEnum)).Cast<TemplateMatchModesEnum>();

        }

        public override async Task LoadFlowStepId(int flowStepId)
        {
            FlowStep? flowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(x => x.Id == flowStepId);
            if (flowStep != null)
                FlowStep = flowStep;

            FlowParameters = await _baseDatawork.FlowParameters.FindParametersFromFlowStep(flowStepId);
            FlowParameters = FlowParameters.Where(x => x.Type == FlowParameterTypesEnum.TEMPLATE_SEARCH_AREA).ToList();
        }

        public override async Task LoadNewFlowStep(FlowStep newFlowStep)
        {
            FlowStep = newFlowStep;

            FlowParameters = await _baseDatawork.FlowParameters.FindParametersFromFlowStep(newFlowStep.ParentFlowStepId.Value);
            FlowParameters = FlowParameters.Where(x => x.Type == FlowParameterTypesEnum.TEMPLATE_SEARCH_AREA).ToList();

            return;
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
                FlowStep.TemplateImage = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        [RelayCommand]
        private async Task OnButtonTakeScreenshotClick()
        {
            byte[]? resultTemplate = await _windowService.OpenScreenshotSelectionWindow();
            if (resultTemplate == null)
                return;

            FlowStep.TemplateImage = resultTemplate;
        }


        [RelayCommand]
        private void OnButtonTestClick()
        {
            // Find search area.
            Model.Structs.Rectangle? searchRectangle = null;
            switch (FlowStep.FlowParameter?.TemplateSearchAreaType)
            {
                case TemplateSearchAreaTypesEnum.SELECT_EVERY_MONITOR:
                    searchRectangle = _systemService.GetScreenSize();
                    break;
                case TemplateSearchAreaTypesEnum.SELECT_MONITOR:
                    searchRectangle = _systemService.GetMonitorArea(FlowStep.FlowParameter.SystemMonitorDeviceName);
                    break;
                case TemplateSearchAreaTypesEnum.SELECT_APPLICATION_WINDOW:
                    searchRectangle = _systemService.GetWindowSize(FlowStep.FlowParameter.ProcessName);
                    break;
                case TemplateSearchAreaTypesEnum.SELECT_CUSTOM_AREA:
                    break;
                default:
                    searchRectangle = _systemService.GetScreenSize();
                    break;
            }

            if (searchRectangle == null)
                searchRectangle = _systemService.GetScreenSize();


            // Get screenshot.
            Bitmap? screenshot = _systemService.TakeScreenShot(searchRectangle.Value);
            if (screenshot == null)
                return;

            if (FlowStep.TemplateImage != null)
                using (var ms = new MemoryStream(FlowStep.TemplateImage))
                {
                    Bitmap templateImage = new Bitmap(ms);
                    TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(templateImage, screenshot, FlowStep.TemplateMatchMode, false);

                    if (result.ResultImagePath.Length > 1)
                        ResultImage = File.ReadAllBytes(result.ResultImagePath);
                }
        }

        [RelayCommand]
        private async Task OnTemplateImageDoubleClick(MouseButtonEventArgs e)
        {
            // Check if it's a double-click.
            if (e.ClickCount == 2)
            {
                byte[]? image = await _windowService.OpenScreenshotSelectionWindow(FlowStep.TemplateImage);
                if (image != null)
                    FlowStep.TemplateImage = image;
            }
        }

        [RelayCommand]
        private async Task OnResultImageDoubleClick(MouseButtonEventArgs e)
        {
            // Check if it's a double-click.
            if (e.ClickCount == 2)
                await _windowService.OpenScreenshotSelectionWindow(ResultImage, false);
        }

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            // Edit mode.
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Accuracy = FlowStep.Accuracy;
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.ProcessName = FlowStep.ProcessName;
            }

            // Add mode.
            else
            {
                FlowStep isNewSimpling;

                if (FlowStep.ParentFlowStepId != null)
                    isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(FlowStep.ParentFlowStepId.Value);
                else if (FlowStep.FlowId.HasValue)
                    isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(FlowStep.FlowId.Value);
                else
                    return;

                FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                isNewSimpling.OrderingNum++;
                await _baseDatawork.SaveChangesAsync();


                // "Success" Flow step
                FlowStep successFlowStep = new FlowStep
                {
                    Name = "Success",
                    IsExpanded = false,
                    Type = FlowStepTypesEnum.SUCCESS,
                    ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        new FlowStep(){Type = FlowStepTypesEnum.NEW}
                    }
                };

                // "Fail" Flow step
                FlowStep failFlowStep = new FlowStep
                {
                    Name = "Fail",
                    IsExpanded = false,
                    Type = FlowStepTypesEnum.FAILURE,
                    ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        new FlowStep(){Type = FlowStepTypesEnum.NEW}
                    }
                };

                FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    successFlowStep,
                    failFlowStep
                };

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Template search";

                FlowStep.IsExpanded = true;

                _baseDatawork.FlowSteps.Add(FlowStep);

                await _baseDatawork.SaveChangesAsync();
                _flowsViewModel.RefreshData();
            }
        }
    }
}

