using Business.DatabaseContext;
using Business.Helpers;
using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Wpf.Ui.Controls;

namespace StepinFlow.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        [ObservableProperty]
        private string _exportPath = PathHelper.GetExportDataPath();

        [ObservableProperty]
        private string _importFileLocation = string.Empty;

        [ObservableProperty]
        private string _exportedFileLocation = string.Empty;


        // Combobox Flows
        [ObservableProperty]
        private Flow? _comboBoxSelectedFlow = null;

        [ObservableProperty]
        private ObservableCollection<Flow> _comboBoxFlows = new ObservableCollection<Flow>();

        public DataViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            ComboBoxFlows = new ObservableCollection<Flow>(_baseDatawork.Flows.GetAll());
        }

        [RelayCommand]
        private void OnButtonChangeDirectoryClick()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                dialog.ShowNewFolderButton = true; // Allows user to create a new folder

                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ExportPath = dialog.SelectedPath;
                }
            }
        }

        [RelayCommand]
        private void OnButtonChangeImportFilePathClick()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = PathHelper.GetAppDataPath();
            openFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
                ImportFileLocation = openFileDialog.FileName;
        }

        [RelayCommand]
        private void OnButtonResetDirectoryClick()
        {
            ExportPath = PathHelper.GetExportDataPath();
        }

        [RelayCommand]
        private async Task OnButtonExportClick()
        {
            List<Flow> flows = await _baseDatawork.Flows.LoadAllExport(ComboBoxSelectedFlow?.Id);

            string fileDate = DateTime.Now.ToString("yy-MM-dd hh.mm.ss");
            string fileName = "Export " + fileDate + ".json";
            string filePath = Path.Combine(ExportPath, fileName);

            await _systemService.ExportFlowsJSON(flows, filePath);
            ExportedFileLocation = "Exported file: " + filePath;
        }

        [RelayCommand]
        private async Task OnButtonImportClick()
        {
            List<Flow>? flows = _systemService.ImportFlowsJSON(ImportFileLocation);
            //flowStep.TemplateImage = File.ReadAllBytes(TemplateImgPath);

            if (flows != null)
                foreach (Flow flow in flows)
                {
                    Flow? clonedFlow = await FlowStepClone(flow);
                    _baseDatawork.Flows.Add(clonedFlow);
                }

            await _baseDatawork.SaveChangesAsync();
        }

        [RelayCommand]
        private async Task OnButtonDeleteClick()
        {
            var executions = _baseDatawork.Executions.GetAll();
            _baseDatawork.Executions.RemoveRange(executions);
            _baseDatawork.SaveChanges();

            // Reclaim free space in database file.
            await _baseDatawork.Query.Database.ExecuteSqlRawAsync("VACUUM;");
        }


        private async Task<Flow?> FlowStepClone(Flow flow)
        {
            Queue<(FlowStep, FlowStep)> queue = new Queue<(FlowStep, FlowStep)>();
            Dictionary<int, FlowStep> clonedFlowSteps = new Dictionary<int, FlowStep>();

            // Clone the root (Flow).
            Flow clonedFlow = new Flow
            {
                Name = flow.Name,
                IsExpanded = flow.IsExpanded,
                OrderingNum = flow.OrderingNum,
            };

            foreach (FlowStep flowStep in flow.FlowSteps)
            {
                FlowStep clonedFlowStep = CreateFlowStepClone(flowStep);
                queue.Enqueue((flowStep, clonedFlowStep));
                clonedFlowSteps.Add(flowStep.Id, clonedFlowStep);
                clonedFlow.FlowSteps.Add(clonedFlowStep);
            }

            while (queue.Count > 0)
            {
                var (originalNode, clonedNode) = queue.Dequeue();


                foreach (FlowStep child in originalNode.ChildrenFlowSteps)
                {
                    FlowStep? parentTemplateSearchFlowStep = null;
                    if (child.ParentTemplateSearchFlowStepId.HasValue)
                        parentTemplateSearchFlowStep = clonedFlowSteps
                            .Where(x => x.Key == child.ParentTemplateSearchFlowStepId.Value)
                            .FirstOrDefault()
                            .Value;

                    var clonedChild = CreateFlowStepClone(child, clonedNode, parentTemplateSearchFlowStep);

                    // Add to the parent's children
                    clonedNode.ChildrenFlowSteps.Add(clonedChild);

                    // Enqueue for further processing
                    queue.Enqueue((child, clonedChild));
                    clonedFlowSteps.Add(child.Id, clonedChild);

                }

                // Template search flow steps.
                foreach (FlowStep child in originalNode.ChildrenTemplateSearchFlowSteps)
                    clonedNode.ChildrenTemplateSearchFlowSteps.Add(CreateFlowStepClone(child));
            }

            return clonedFlow;

        }

        private FlowStep CreateFlowStepClone(FlowStep flowStep, FlowStep? parentFlowStep = null, FlowStep? parentTemplateSearchFlowStep = null)
        {
            return new FlowStep
            {
                ParentFlowStep = parentFlowStep,
                ParentTemplateSearchFlowStep = parentTemplateSearchFlowStep,
                Name = flowStep.Name,
                ProcessName = flowStep.ProcessName,
                IsExpanded = flowStep.IsExpanded,
                Disabled = flowStep.Disabled,
                IsSelected = false,
                OrderingNum = flowStep.OrderingNum,
                FlowStepType = flowStep.FlowStepType,
                TemplateImagePath = flowStep.TemplateImagePath,
                TemplateImage = flowStep.TemplateImage,
                Accuracy = flowStep.Accuracy,
                LocationX = flowStep.LocationX,
                LocationY = flowStep.LocationY,
                MaxLoopCount = flowStep.MaxLoopCount,
                RemoveTemplateFromResult = flowStep.RemoveTemplateFromResult,
                LoopResultImagePath = flowStep.LoopResultImagePath,
                MouseAction = flowStep.MouseAction,
                MouseButton = flowStep.MouseButton,
                MouseScrollDirectionEnum = flowStep.MouseScrollDirectionEnum,
                MouseLoopInfinite = flowStep.MouseLoopInfinite,
                MouseLoopTimes = flowStep.MouseLoopTimes,
                MouseLoopDebounceTime = flowStep.MouseLoopDebounceTime,
                MouseLoopTime = flowStep.MouseLoopTime,
                SleepForHours = flowStep.SleepForHours,
                SleepForMinutes = flowStep.SleepForMinutes,
                SleepForSeconds = flowStep.SleepForSeconds,
                SleepForMilliseconds = flowStep.SleepForMilliseconds,
                WindowHeight = flowStep.WindowHeight,
                WindowWidth = flowStep.WindowWidth,
            };
        }
        public void OnNavigatedTo() { }
        public void OnNavigatedFrom() { }
    }
}
