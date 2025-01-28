using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.ConverterModels;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ModernAiClicker.ViewModels.UserControls
{
    public partial class TreeViewUserControlViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public event OnSelectedFlowStepIdChanged? OnSelectedFlowStepIdChangedEvent;
        public delegate void OnSelectedFlowStepIdChanged(int Id);

        public event OnFlowStepClone? OnFlowStepCloneEvent;
        public delegate void OnFlowStepClone(int Id);

        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        public TreeViewUserControlViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task LoadFlowsAsync(int flowId = 0)
        {
            List<Flow> flows = new List<Flow>();

            if(flowId > 0)
                flows = await _baseDatawork.Query.Flows
                    .Include(x => x.FlowSteps)
                    .ThenInclude(x => x.ChildrenFlowSteps)
                    .Where(x => x.Id == flowId)
                    .ToListAsync();
            else
                flows = await _baseDatawork.Query.Flows
                    .Include(x => x.FlowSteps)
                    .ThenInclude(x => x.ChildrenFlowSteps)
                    .ToListAsync();

            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowSteps)
                    await LoadFlowStepChildren(flowStep);

            FlowsList = null;
            FlowsList = new ObservableCollection<Flow>(flows);
        }

        [RelayCommand]
        private void OnButtonCopyClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                CoppiedFlowStepId = flowStep.Id;

                // Fire event.
                OnFlowStepCloneEvent?.Invoke(flowStep.Id);
            }
        }

        private async Task LoadFlowStepChildren(FlowStep flowStep)
        {
            List<FlowStep> flowSteps = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .ThenInclude(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.Id)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .ToListAsync();

            flowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);

            foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
            {
                if (childFlowStep.IsExpanded)
                    await LoadFlowStepChildren(childFlowStep);
            }
        }


        public async Task AddNewFlow()
        {
            FlowStep newFlowStep = new FlowStep();
            newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;

            Flow flow = new Flow();
            flow.Name = "Flow";
            flow.IsSelected = true;
            flow.FlowSteps.Add(newFlowStep);

            _baseDatawork.Flows.Add(flow);
            _baseDatawork.SaveChanges();

            //await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            FlowsList.Add(flow);
        }

        public async Task ExpandAll()
        {
            List<Flow> flows = await _baseDatawork.Flows.GetAllAsync();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.GetAllAsync();

            flows.ForEach(x => x.IsExpanded = true);
            flowSteps.ForEach(x => x.IsExpanded = true);

            await _baseDatawork.SaveChangesAsync();
        }

        public async Task CollapseAll()
        {
            List<Flow> flows = await _baseDatawork.Flows.GetAllAsync();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.GetAllAsync();

            flows.ForEach(x => x.IsExpanded = false);
            flowSteps.ForEach(x => x.IsExpanded = false);

            await _baseDatawork.SaveChangesAsync();
        }

        [RelayCommand]
        private void OnButtonNewClick(EventParammeters eventParameters)
        {
            FlowStep flowStep = new FlowStep();

            // If flowId is available
            if (eventParameters.FlowId != null)
            {
                bool isFlowIdParsable = Int32.TryParse(eventParameters.FlowId.ToString(), out int flowId);

                if (isFlowIdParsable)
                    flowStep.FlowId = flowId;
            }
            // If flowStepId is available
            else if (eventParameters.FlowStepId != null)
            {
                bool isFlowStepIdParsable = Int32.TryParse(eventParameters.FlowStepId.ToString(), out int flowStepId);
                if (isFlowStepIdParsable)
                {

                    int? parentFlowStepId = _baseDatawork.FlowSteps
                        .Where(x => x.Id == flowStepId)
                        .Select(x => x.ParentFlowStepId)
                        .First();

                    flowStep.ParentFlowStepId = parentFlowStepId;
                }
            }

        }

        [RelayCommand]
        private async Task OnButtonPasteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                int? parentId = flowStep.ParentFlowStepId ?? flowStep.FlowId ?? null;
                if (CoppiedFlowStepId.HasValue)
                {

                    FlowStep clonedFlowStep = await GetClonedFlowStepAsync(CoppiedFlowStepId.Value);

                    if (flowStep.ParentFlowStepId.HasValue)
                    {

                        //  Load the target parent.
                        FlowStep? targetParent = await _baseDatawork.FlowSteps.Query
                        .Include(fs => fs.ChildrenFlowSteps)
                        .FirstOrDefaultAsync(fs => fs.Id == flowStep.ParentFlowStepId.Value);

                        if (targetParent == null)
                            return;



                        FlowStep isNewSimpling = await _baseDatawork.FlowSteps.GetIsNewSibling(targetParent.id);

                        clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                        clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                        isNewSimpling.OrderingNum++;

                        // Attach the cloned root to the target parent.
                        targetParent.ChildrenFlowSteps.Add(clonedFlowStep);

                    }
                    else if (flowStep.FlowId.HasValue)
                    {

                        //  Load the target parent.
                        Flow? targetParent = await _baseDatawork.Flows.Query
                        .Include(fs => fs.FlowSteps)
                        .FirstOrDefaultAsync(fs => fs.Id == flowStep.FlowId.Value);

                        if (targetParent == null)
                            return;

                        FlowStep isNewSimpling = await _baseDatawork.Flows.GetIsNewSibling(targetParent.Id);
                        clonedFlowStep.ParentFlowStepId = flowStep.ParentFlowStepId;
                        clonedFlowStep.OrderingNum = isNewSimpling.OrderingNum;
                        isNewSimpling.OrderingNum++;

                        // Attach the cloned root to the target parent.
                        targetParent.FlowSteps.Add(clonedFlowStep);

                    }
                    // Save changes.
                    await _baseDatawork.SaveChangesAsync();
                }
            }
        }


        public async Task<FlowStep> GetClonedFlowStepAsync(int cloneId)
        {
            Queue<(FlowStep, FlowStep)> queue = new Queue<(FlowStep, FlowStep)>();
            Dictionary<int, FlowStep> clonedFlowSteps = new Dictionary<int, FlowStep>();

            // Step 1: Load the source branch (including its children and template search relationships)
            FlowStep? originalFlowStep = await _baseDatawork.FlowSteps.Query
                .Include(fs => fs.ChildrenFlowSteps)
                .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
                .FirstOrDefaultAsync(fs => fs.Id == cloneId);

            if (originalFlowStep == null)
                throw new ArgumentException($"Source branch with ID {cloneId} not found.");


            // Step 2: Use a queue to clone the tree iteratively
            FlowStep clonedFlowStep = new FlowStep
            {
                //ParentFlowStep = targetParent,
                //ParentFlowStepId = targetParent.Id,
                //ParentTemplateSearchFlowStep = sourceBranch.ParentTemplateSearchFlowStep,
                //ParentTemplateSearchFlowStepId = sourceBranch.ParentTemplateSearchFlowStepId,
                Name = originalFlowStep.Name,
                ProcessName = originalFlowStep.ProcessName,
                IsExpanded = originalFlowStep.IsExpanded,
                Disabled = originalFlowStep.Disabled,
                IsSelected = false,
                OrderingNum = originalFlowStep.OrderingNum,
                FlowStepType = originalFlowStep.FlowStepType,
                TemplateImagePath = originalFlowStep.TemplateImagePath,
                TemplateImage = originalFlowStep.TemplateImage,
                Accuracy = originalFlowStep.Accuracy,
                LocationX = originalFlowStep.LocationX,
                LocationY = originalFlowStep.LocationY,
                MaxLoopCount = originalFlowStep.MaxLoopCount,
                RemoveTemplateFromResult = originalFlowStep.RemoveTemplateFromResult,
                LoopResultImagePath = originalFlowStep.LoopResultImagePath,
                MouseAction = originalFlowStep.MouseAction,
                MouseButton = originalFlowStep.MouseButton,
                MouseScrollDirectionEnum = originalFlowStep.MouseScrollDirectionEnum,
                MouseLoopInfinite = originalFlowStep.MouseLoopInfinite,
                MouseLoopTimes = originalFlowStep.MouseLoopTimes,
                MouseLoopDebounceTime = originalFlowStep.MouseLoopDebounceTime,
                MouseLoopTime = originalFlowStep.MouseLoopTime,
                SleepForHours = originalFlowStep.SleepForHours,
                SleepForMinutes = originalFlowStep.SleepForMinutes,
                SleepForSeconds = originalFlowStep.SleepForSeconds,
                SleepForMilliseconds = originalFlowStep.SleepForMilliseconds,
                WindowHeight = originalFlowStep.WindowHeight,
                WindowWidth = originalFlowStep.WindowWidth,

            };

            queue.Enqueue((originalFlowStep, clonedFlowStep));
            clonedFlowSteps.Add(originalFlowStep.Id, clonedFlowStep);

            while (queue.Count > 0)
            {
                var (originalNode, clonedNode) = queue.Dequeue();
                // Children flow steps.
                var originalChildrenFlowSteps = await _baseDatawork.FlowSteps.Query
                .Include(fs => fs.ChildrenFlowSteps)
                .Where(fs => fs.Id == originalNode.Id)
                .SelectMany(x => x.ChildrenFlowSteps)
                .ToListAsync();

                // Template search flow steps.
                var originalChildrenTemplateSearchFlowSteps = await _baseDatawork.FlowSteps.Query
                .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
                .Where(fs => fs.Id == originalNode.Id)
                .SelectMany(x => x.ChildrenTemplateSearchFlowSteps)
                .ToListAsync();

                foreach (var child in originalChildrenFlowSteps)
                {
                    FlowStep? parentTemplateSearchFlowStep = null;
                    if (child.ParentTemplateSearchFlowStepId.HasValue)
                        parentTemplateSearchFlowStep = clonedFlowSteps
                            .Where(x => x.Key == child.ParentTemplateSearchFlowStepId.Value)
                            .FirstOrDefault()
                            .Value;

                    var clonedChild = new FlowStep
                    {
                        ParentFlowStep = clonedNode,
                        //ParentFlowStepId = currentClone.Id,
                        ParentTemplateSearchFlowStep = parentTemplateSearchFlowStep, // Preserve template references
                        //ParentTemplateSearchFlowStepId = child.ParentTemplateSearchFlowStepId, // Preserve template IDs
                        Name = child.Name,
                        ProcessName = child.ProcessName,
                        IsExpanded = child.IsExpanded,
                        Disabled = child.Disabled,
                        IsSelected = false,
                        OrderingNum = child.OrderingNum,
                        FlowStepType = child.FlowStepType,
                        TemplateImagePath = child.TemplateImagePath,
                        TemplateImage = child.TemplateImage,
                        Accuracy = child.Accuracy,
                        LocationX = child.LocationX,
                        LocationY = child.LocationY,
                        MaxLoopCount = child.MaxLoopCount,
                        RemoveTemplateFromResult = child.RemoveTemplateFromResult,
                        LoopResultImagePath = child.LoopResultImagePath,
                        MouseAction = child.MouseAction,
                        MouseButton = child.MouseButton,
                        MouseScrollDirectionEnum = child.MouseScrollDirectionEnum,
                        MouseLoopInfinite = child.MouseLoopInfinite,
                        MouseLoopTimes = child.MouseLoopTimes,
                        MouseLoopDebounceTime = child.MouseLoopDebounceTime,
                        MouseLoopTime = child.MouseLoopTime,
                        SleepForHours = child.SleepForHours,
                        SleepForMinutes = child.SleepForMinutes,
                        SleepForSeconds = child.SleepForSeconds,
                        SleepForMilliseconds = child.SleepForMilliseconds,
                        WindowHeight = child.WindowHeight,
                        WindowWidth = child.WindowWidth,
                    };

                    // Add to the parent's children
                    clonedNode.ChildrenFlowSteps.Add(clonedChild);

                    // Enqueue for further processing
                    queue.Enqueue((child, clonedChild));
                    clonedFlowSteps.Add(child.Id, clonedChild);

                }

                foreach (var child in originalChildrenTemplateSearchFlowSteps)
                {
                    clonedNode.ChildrenTemplateSearchFlowSteps.Add(new FlowStep
                    {
                        //ParentFlowStep = currentClone,
                        //ParentFlowStepId = currentClone.Id,
                        //ParentTemplateSearchFlowStep = clonedNode, // Preserve template references
                        //ParentTemplateSearchFlowStepId = child.ParentTemplateSearchFlowStepId, // Preserve template IDs
                        Name = child.Name,
                        ProcessName = child.ProcessName,
                        IsExpanded = child.IsExpanded,
                        Disabled = child.Disabled,
                        IsSelected = false,
                        OrderingNum = child.OrderingNum,
                        FlowStepType = child.FlowStepType,
                        TemplateImagePath = child.TemplateImagePath,
                        TemplateImage = child.TemplateImage,
                        Accuracy = child.Accuracy,
                        LocationX = child.LocationX,
                        LocationY = child.LocationY,
                        MaxLoopCount = child.MaxLoopCount,
                        RemoveTemplateFromResult = child.RemoveTemplateFromResult,
                        LoopResultImagePath = child.LoopResultImagePath,
                        MouseAction = child.MouseAction,
                        MouseButton = child.MouseButton,
                        MouseScrollDirectionEnum = child.MouseScrollDirectionEnum,
                        MouseLoopInfinite = child.MouseLoopInfinite,
                        MouseLoopTimes = child.MouseLoopTimes,
                        MouseLoopDebounceTime = child.MouseLoopDebounceTime,
                        MouseLoopTime = child.MouseLoopTime,
                        SleepForHours = child.SleepForHours,
                        SleepForMinutes = child.SleepForMinutes,
                        SleepForSeconds = child.SleepForSeconds,
                        SleepForMilliseconds = child.SleepForMilliseconds,
                        WindowHeight = child.WindowHeight,
                        WindowWidth = child.WindowWidth,
                    });
                }
            }

            return clonedFlowStep;

        }




        [RelayCommand]
        private async Task OnFlowStepButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                _baseDatawork.FlowSteps.Remove(flowStep);

                await _baseDatawork.SaveChangesAsync();
                await LoadFlowsAsync();
            }
        }

        [RelayCommand]
        private async Task OnFlowButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;
                _baseDatawork.Flows.Remove(flow);

                await _baseDatawork.SaveChangesAsync();
                await LoadFlowsAsync();
            }
        }


        [RelayCommand]
        private async Task OnButtonUpClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplingsAbove = new List<FlowStep>();

                // Get siblings based on flowstep beeing bellow flow or flowstep
                if (flowStep.ParentFlowStepId.HasValue)
                    simplingsAbove = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.ParentFlowStepId.Value)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .Where(x => x.OrderingNum < flowStep.OrderingNum)
                        .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                        .ToListAsync();
                else if (flowStep.FlowId.HasValue)
                    simplingsAbove = await _baseDatawork.Query.Flows
                        .Include(x => x.FlowSteps)
                        .Where(x => x.Id == flowStep.FlowId.Value)
                        .SelectMany(x => x.FlowSteps)
                        .Where(x => x.OrderingNum < flowStep.OrderingNum)
                        .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                        .ToListAsync();


                if (simplingsAbove.Any())
                {
                    // Find max
                    FlowStep simplingAbove = simplingsAbove.Aggregate((currentMax, x) => x.OrderingNum > currentMax.OrderingNum ? x : currentMax);

                    // Swap values
                    (flowStep.OrderingNum, simplingAbove.OrderingNum) = (simplingAbove.OrderingNum, flowStep.OrderingNum);

                    await _baseDatawork.SaveChangesAsync();
                    await LoadFlowsAsync();
                }
            }
        }

        [RelayCommand]
        private async Task OnButtonDownClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                List<FlowStep> simplingsBellow = new List<FlowStep>();

                // Get siblings based on flowstep beeing bellow flow or flowstep
                if (flowStep.ParentFlowStepId.HasValue)
                    simplingsBellow = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.ParentFlowStepId.Value)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .Where(x => x.OrderingNum > flowStep.OrderingNum)
                        .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                        .ToListAsync();
                else if (flowStep.FlowId.HasValue)
                    simplingsBellow = await _baseDatawork.Query.Flows
                        .Include(x => x.FlowSteps)
                        .Where(x => x.Id == flowStep.FlowId.Value)
                        .SelectMany(x => x.FlowSteps)
                        .Where(x => x.OrderingNum > flowStep.OrderingNum)
                        .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                        .ToListAsync();


                if (simplingsBellow.Any())
                {
                    // Find min
                    FlowStep simplingBellow = simplingsBellow.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);

                    // Swap values
                    (flowStep.OrderingNum, simplingBellow.OrderingNum) = (simplingBellow.OrderingNum, flowStep.OrderingNum);

                    await _baseDatawork.SaveChangesAsync();
                    await LoadFlowsAsync();
                }
            }
        }


        [RelayCommand]
        private void OnSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is FlowStep)
            {
                FlowStep flowStep = (FlowStep)selectedItem;
                OnSelectedFlowStepIdChangedEvent?.Invoke(flowStep.Id);
            }

        }

        [RelayCommand]
        private async Task OnExpanded(EventParammeters eventParameters)
        {
            if (eventParameters == null)
                return;

            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;

                if (flowStep.ChildrenFlowSteps == null)
                    return;

                foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
                    await LoadFlowStepChildren(childFlowStep);
            }

            else if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;

                foreach (var childFlowStep in flow.FlowSteps)
                    await LoadFlowStepChildren(childFlowStep);

            }
        }
    }
}
