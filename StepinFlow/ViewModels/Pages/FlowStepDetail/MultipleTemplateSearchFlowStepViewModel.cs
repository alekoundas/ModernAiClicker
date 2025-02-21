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
using Rectangle = Model.Structs.Rectangle;
using Microsoft.EntityFrameworkCore;
using StepinFlow.Interfaces;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Business.BaseViewModels;

namespace StepinFlow.ViewModels.Pages
{
    public partial class MultipleTemplateSearchFlowStepViewModel : BaseFlowStepDetailVM
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
        private ObservableCollection<FlowStep> _childrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>();
        private readonly List<FlowStep> _childrenTemplateSearchFlowStepsToRemove = new List<FlowStep>();


        public MultipleTemplateSearchFlowStepViewModel(
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

        }

        public override async Task LoadFlowStepId(int flowStepId)
        {
            FlowStep? flowStep = await _baseDatawork.FlowSteps.Query
                .Include(x => x.ChildrenTemplateSearchFlowSteps)
                .FirstOrDefaultAsync(x => x.Id == flowStepId);

            if (flowStep != null)
            {
                FlowStep = flowStep;
                List<FlowStep> flowSteps = flowStep.ChildrenTemplateSearchFlowSteps.Where(x => x.Type == TypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD).ToList();
                ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
            }
        }

        [RelayCommand]
        private void OnButtonOpenFileClick(FlowStep flowStep)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = PathHelper.GetAppDataPath();
            openFileDialog.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                flowStep.TemplateImage = File.ReadAllBytes(openFileDialog.FileName);
            }
        }
        [RelayCommand]
        private async Task OnButtonTakeScreenshotClick(FlowStep flowStep)
        {
            byte[]? resultTemplate = await _windowService.OpenScreenshotSelectionWindow();
            if (resultTemplate == null)
                return;

            flowStep.TemplateImage = resultTemplate;
        }


        [RelayCommand]
        private void OnButtonUpClick(FlowStep flowStep)
        {
            List<FlowStep> simplingsAbove = ChildrenTemplateSearchFlowSteps
                    .Where(x => x.OrderingNum < flowStep.OrderingNum)
                    .ToList();

            flowStep.OrderingNum++;
            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(ChildrenTemplateSearchFlowSteps.OrderBy(x => x.OrderingNum).ToList());

        }

        [RelayCommand]
        private void OnButtonDownClick(FlowStep flowStep)
        {
            List<FlowStep> simplingsBellow = ChildrenTemplateSearchFlowSteps
                    .Where(x => x.OrderingNum > flowStep.OrderingNum)
                    .ToList();

            flowStep.OrderingNum--;
            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(ChildrenTemplateSearchFlowSteps.OrderBy(x => x.OrderingNum).ToList());
        }

        [RelayCommand]
        public void OnButtonAddClick()
        {
            FlowStep newFlowStep = new FlowStep();

            // In edit mode set parent ID.
            if (FlowStep.Id > 0)
                newFlowStep.ParentTemplateSearchFlowStepId = FlowStep.Id;

            ChildrenTemplateSearchFlowSteps.Add(newFlowStep);
        }

        [RelayCommand]
        private void OnButtonDeleteClick(FlowStep flowStep)
        {
            if (flowStep.Id > 0)
                _childrenTemplateSearchFlowStepsToRemove.Add(flowStep);

            ChildrenTemplateSearchFlowSteps.Remove(flowStep);
        }

        [RelayCommand]
        private void OnButtonClearTestClick(FlowStep flowStep)
        {
            ResultImage = null;
        }

        [RelayCommand]
        private void OnButtonTestClick(FlowStep flowStep)
        {
            if (flowStep.TemplateImage == null)
                return;

            // Find search area.
            Rectangle searchRectangle;
            if (FlowStep.ProcessName.Length > 0)
                searchRectangle = _systemService.GetWindowSize(FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();

            // Get screenshot.
            Bitmap? screenshot = _systemService.TakeScreenShot(searchRectangle);
            if (screenshot == null)
                return;

            using (var ms = new MemoryStream(flowStep.TemplateImage))
            {
                Bitmap templateImage = new Bitmap(ms);
                TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(templateImage, screenshot, flowStep.RemoveTemplateFromResult);

                if (result.ResultImagePath.Length > 0)
                {
                    ResultImage = File.ReadAllBytes(result.ResultImagePath);
                }

            }
        }

        [RelayCommand]
        private async Task OnTemplateImageDoubleClick(MouseButtonEventArgs e)
        {

            // Check if it's a double-click.
            if (e.ClickCount == 2)
            {
                if (e.Source is System.Windows.Controls.Image img && img.Source is BitmapSource bitmapSource)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(stream);
                        byte[]? image = await _windowService.OpenScreenshotSelectionWindow(stream.ToArray());
                        if (image != null)
                            using (var stream2 = new MemoryStream(image))
                            {
                                var decoder = BitmapDecoder.Create(stream2, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                                img.Source = decoder.Frames[0];
                            }
                    }
                }

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
            // Remove flow steps that dont contain a template image.
            List<FlowStep> templateFlowSteps = ChildrenTemplateSearchFlowSteps
                .Where(x => x.TemplateImage == null)
                .ToList();

            foreach (var templateFlowStep in templateFlowSteps)
                ChildrenTemplateSearchFlowSteps.Remove(templateFlowStep);

            foreach (var templateFlowStep in ChildrenTemplateSearchFlowSteps)
                templateFlowStep.Type = TypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD;

            // Edit mode
            if (FlowStep.Id > 0)
            {
                FlowStep updateFlowStep = await _baseDatawork.FlowSteps.FirstAsync(x => x.Id == FlowStep.Id);
                updateFlowStep.Name = FlowStep.Name;
                updateFlowStep.ProcessName = FlowStep.ProcessName;

                _baseDatawork.UpdateRange(ChildrenTemplateSearchFlowSteps.Where(x => x.Id > 0).ToList());
                _baseDatawork.FlowSteps.AddRange(ChildrenTemplateSearchFlowSteps.Where(x => x.Id == 0).ToList());
                _baseDatawork.FlowSteps.RemoveRange(_childrenTemplateSearchFlowStepsToRemove);
            }

            /// Add mode
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
                    Type = TypesEnum.IS_SUCCESS,
                    ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        new FlowStep(){Type = TypesEnum.IS_NEW}
                    }
                };

                // "Fail" Flow step
                FlowStep failFlowStep = new FlowStep
                {
                    Name = "Fail",
                    IsExpanded = false,
                    Type = TypesEnum.IS_FAILURE,
                    ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        new FlowStep(){Type = TypesEnum.IS_NEW}
                    }
                };

                FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                {
                    successFlowStep,
                    failFlowStep
                };

                if (FlowStep.Name.Length == 0)
                    FlowStep.Name = "Multiple template search loop.";

                FlowStep.IsExpanded = true;
                FlowStep.ChildrenTemplateSearchFlowSteps = ChildrenTemplateSearchFlowSteps;

                _baseDatawork.FlowSteps.Add(FlowStep);
            }



            await _baseDatawork.SaveChangesAsync();
            _flowsViewModel.RefreshData();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.Query
                .AsNoTracking()
                .Where(x => x.ParentTemplateSearchFlowStepId == FlowStep.Id)
                .Where(x => x.Type == TypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
                .ToListAsync();
            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
        }
    }
}

