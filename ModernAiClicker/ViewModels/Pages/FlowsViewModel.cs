using Model.Models;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Model.ConverterModels;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Wpf.Ui.Controls;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AutoMapper;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowsViewModel : ObservableObject, INavigationAware, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public event NavigateToFlowStepTypeSelectionPageEvent? NavigateToFlowStepTypeSelectionPage;
        public delegate void NavigateToFlowStepTypeSelectionPageEvent(FlowStep flowStep);

        [ObservableProperty]
        private ObservableCollection<Flow> _flowsList = new ObservableCollection<Flow>();

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private int? _coppiedFlowStepId;


        public FlowsViewModel(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;

            Task.Run(async () => await RefreshData());

        }

        public async Task RefreshData()
        {
            List<Flow> flows = await _baseDatawork.Query.Flows
                .Include(x => x.FlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .ToListAsync();

            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowSteps)
                    await LoadFlowStepChildren(flowStep);

            FlowsList = null;
            FlowsList = new ObservableCollection<Flow>(flows);
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


        [RelayCommand]
        private async Task OnButtonAddFlowClick()
        {
            Flow flow = new Flow();
            FlowStep newFlowStep = new FlowStep();

            newFlowStep.FlowStepType = FlowStepTypesEnum.IS_NEW;
            newFlowStep.IsSelected = false;
            flow.Name = "Flow";
            flow.IsSelected = true;
            flow.FlowSteps.Add(newFlowStep);
            _baseDatawork.Flows.Add(flow);
            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            FlowsList.Add(flow);
            await RefreshData();
        }


        [RelayCommand]
        private void OnButtonLockClick()
        {
            IsLocked = !IsLocked;
        }

        [RelayCommand]
        private async Task OnButtonSyncClick()
        {
            await RefreshData();
        }

        [RelayCommand]
        private async Task OnButtonExpandAllClick()
        {
            List<Flow> flows = await _baseDatawork.Flows.GetAllAsync();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.GetAllAsync();

            flows.ForEach(x => x.IsExpanded = true);
            flowSteps.ForEach(x => x.IsExpanded = true);

            await _baseDatawork.SaveChangesAsync();
        }

        [RelayCommand]
        private async Task OnButtonCollapseAllClick()
        {
            List<Flow> flows = await _baseDatawork.Flows.GetAllAsync();
            List<FlowStep> flowSteps = await _baseDatawork.FlowSteps.GetAllAsync();

            flows.ForEach(x => x.IsExpanded = false);
            flowSteps.ForEach(x => x.IsExpanded = false);

            await _baseDatawork.SaveChangesAsync();
        }

        [RelayCommand]
        private void OnTreeViewItemButtonNewClick(EventParammeters eventParameters)
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

            NavigateToFlowStepTypeSelectionPage?.Invoke(flowStep);
        }

        [RelayCommand]
        private async Task OnTreeViewItemButtonPasteClick(EventParammeters eventParameters)
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



        //public async Task GetClonedFlowStepAsync(int sourceBranchId, int? targetParentId)
        //{
        //    //var mapper = new MapperConfiguration(x =>
        //    //{
        //    //    x.CreateMap<FlowStep, FlowStep>()
        //    //    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => 0)) // Set Id to 0 for new entities
        //    //    .ForMember(dest => dest.ParentFlowStep, opt => opt.Ignore()) // Set manually
        //    //    .ForMember(dest => dest.ParentFlowStepId, opt => opt.Ignore()) // Set manually
        //    //    .ForMember(dest => dest.ChildrenFlowSteps, opt => opt.Ignore()) // Handle manually for iterative approach
        //    //    .ForMember(dest => dest.ChildrenTemplateSearchFlowSteps, opt => opt.MapFrom(src => src.ChildrenTemplateSearchFlowSteps)); // Retain references to existing entities
        //    //}
        //    //        ).CreateMapper();

        //    // Step 1: Load the source branch (including its children and template search relationships)
        //    var sourceBranch = await _baseDatawork.FlowSteps.Query
        //        .Include(fs => fs.ChildrenFlowSteps)
        //        .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
        //        .FirstOrDefaultAsync(fs => fs.Id == sourceBranchId);

        //    if (sourceBranch == null)
        //        throw new ArgumentException($"Source branch with ID {sourceBranchId} not found.");

        //    // Step 2: Load the target parent
        //    var targetParent = await _baseDatawork.FlowSteps.Query
        //        .Include(fs => fs.ChildrenFlowSteps)
        //        .FirstOrDefaultAsync(fs => fs.Id == targetParentId);

        //    if (targetParent == null)
        //        throw new ArgumentException($"Target parent with ID {targetParentId} not found.");

        //    // Step 3: Use a queue to clone the tree iteratively
        //    var clonedRoot = new FlowStep
        //    {
        //        ParentFlowStep = targetParent,
        //        ParentFlowStepId = targetParent.Id,
        //        ParentTemplateSearchFlowStep = sourceBranch.ParentTemplateSearchFlowStep,
        //        ParentTemplateSearchFlowStepId = sourceBranch.ParentTemplateSearchFlowStepId,
        //        Name = sourceBranch.Name,
        //        ProcessName = sourceBranch.ProcessName,
        //        IsExpanded = sourceBranch.IsExpanded,
        //        Disabled = sourceBranch.Disabled,
        //        IsSelected = false,
        //        OrderingNum = sourceBranch.OrderingNum,
        //        FlowStepType = sourceBranch.FlowStepType,
        //        TemplateImagePath = sourceBranch.TemplateImagePath,
        //        TemplateImage = sourceBranch.TemplateImage,
        //        Accuracy = sourceBranch.Accuracy,
        //        LocationX = sourceBranch.LocationX,
        //        LocationY = sourceBranch.LocationY,
        //        MaxLoopCount = sourceBranch.MaxLoopCount,
        //        RemoveTemplateFromResult = sourceBranch.RemoveTemplateFromResult,
        //        LoopResultImagePath = sourceBranch.LoopResultImagePath,
        //        MouseAction = sourceBranch.MouseAction,
        //        MouseButton = sourceBranch.MouseButton,
        //        MouseScrollDirectionEnum = sourceBranch.MouseScrollDirectionEnum,
        //        MouseLoopInfinite = sourceBranch.MouseLoopInfinite,
        //        MouseLoopTimes = sourceBranch.MouseLoopTimes,
        //        MouseLoopDebounceTime = sourceBranch.MouseLoopDebounceTime,
        //        MouseLoopTime = sourceBranch.MouseLoopTime,
        //        SleepForHours = sourceBranch.SleepForHours,
        //        SleepForMinutes = sourceBranch.SleepForMinutes,
        //        SleepForSeconds = sourceBranch.SleepForSeconds,
        //        SleepForMilliseconds = sourceBranch.SleepForMilliseconds,
        //        WindowHeight = sourceBranch.WindowHeight,
        //        WindowWidth = sourceBranch.WindowWidth,

        //    };

        //    var queue = new Queue<(FlowStep sourceNode, FlowStep clonedNode)>();
        //    queue.Enqueue((sourceBranch, clonedRoot));

        //    while (queue.Count > 0)
        //    {
        //        var (currentSource, currentClone) = queue.Dequeue();

        //        var currentSourceChildrenFlowSteps = await _baseDatawork.FlowSteps.Query
        //        .Include(fs => fs.ChildrenFlowSteps)
        //        .Where(fs => fs.Id == currentSource.Id)
        //        .SelectMany(x => x.ChildrenFlowSteps)
        //        .ToListAsync();

        //        var currentSourceChildrenTemplateSearchFlowSteps = await _baseDatawork.FlowSteps.Query
        //        .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
        //        .Where(fs => fs.Id == currentSource.Id)
        //        .SelectMany(x => x.ChildrenTemplateSearchFlowSteps)
        //        .ToListAsync();

        //        // Clone children (these are new entities)
        //        foreach (var child in currentSourceChildrenFlowSteps)
        //        {
        //            var clonedChild = new FlowStep
        //            {
        //                ParentFlowStep = currentClone,
        //                ParentFlowStepId = currentClone.Id,
        //                ParentTemplateSearchFlowStep = child.ParentTemplateSearchFlowStep, // Preserve template references
        //                ParentTemplateSearchFlowStepId = child.ParentTemplateSearchFlowStepId, // Preserve template IDs
        //                Name = child.Name,
        //                ProcessName = child.ProcessName,
        //                IsExpanded = child.IsExpanded,
        //                Disabled = child.Disabled,
        //                IsSelected = false,
        //                OrderingNum = child.OrderingNum,
        //                FlowStepType = child.FlowStepType,
        //                TemplateImagePath = child.TemplateImagePath,
        //                TemplateImage = child.TemplateImage,
        //                Accuracy = child.Accuracy,
        //                LocationX = child.LocationX,
        //                LocationY = child.LocationY,
        //                MaxLoopCount = child.MaxLoopCount,
        //                RemoveTemplateFromResult = child.RemoveTemplateFromResult,
        //                LoopResultImagePath = child.LoopResultImagePath,
        //                MouseAction = child.MouseAction,
        //                MouseButton = child.MouseButton,
        //                MouseScrollDirectionEnum = child.MouseScrollDirectionEnum,
        //                MouseLoopInfinite = child.MouseLoopInfinite,
        //                MouseLoopTimes = child.MouseLoopTimes,
        //                MouseLoopDebounceTime = child.MouseLoopDebounceTime,
        //                MouseLoopTime = child.MouseLoopTime,
        //                SleepForHours = child.SleepForHours,
        //                SleepForMinutes = child.SleepForMinutes,
        //                SleepForSeconds = child.SleepForSeconds,
        //                SleepForMilliseconds = child.SleepForMilliseconds,
        //                WindowHeight = child.WindowHeight,
        //                WindowWidth = child.WindowWidth,
        //            };

        //            // IMPORTANT: Ensure the ID is null for the cloned entity
        //            clonedChild.Id = 0;

        //            // Add to the parent's children
        //            currentClone.ChildrenFlowSteps.Add(clonedChild);

        //            // Enqueue for further processing
        //            queue.Enqueue((child, clonedChild));
        //        }

        //        // Maintain existing template search relationships
        //        foreach (var templateChild in currentSourceChildrenTemplateSearchFlowSteps)
        //        {
        //            currentClone.ChildrenTemplateSearchFlowSteps.Add(templateChild);
        //        }
        //    }

        //    // Step 4: Attach the cloned root to the target parent
        //    targetParent.ChildrenFlowSteps.Add(clonedRoot);

        //    // Step 5: Save changes
        //    await _baseDatawork.SaveChangesAsync();
        //}














        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                _baseDatawork.FlowSteps.Remove(flowStep);

                await _baseDatawork.SaveChangesAsync();
                await RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowButtonDeleteClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is Flow)
            {
                Flow flow = (Flow)eventParameters.FlowId;
                _baseDatawork.Flows.Remove(flow);

                await _baseDatawork.SaveChangesAsync();
                await RefreshData();
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonCopyClick(EventParammeters eventParameters)
        {
            if (eventParameters.FlowId is FlowStep)
            {
                FlowStep flowStep = (FlowStep)eventParameters.FlowId;
                CoppiedFlowStepId = flowStep.Id;
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonUpClick(EventParammeters eventParameters)
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
                    await RefreshData();
                }
            }
        }

        [RelayCommand]
        private async Task OnTreeViewItemFlowStepButtonDownClick(EventParammeters eventParameters)
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
                    await RefreshData();
                }
            }
        }


        [RelayCommand]
        private async Task OnTreeViewItemSelected(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is FlowStep)
            {
                FlowStep flowStep = (FlowStep)selectedItem;
                flowStep = await _baseDatawork.Query.FlowSteps.Include(x => x.ChildrenTemplateSearchFlowSteps).Where(x => x.Id == flowStep.Id).FirstOrDefaultAsync();
                NavigateToFlowStepTypeSelectionPage?.Invoke(flowStep);
            }

        }

        [RelayCommand]
        private async Task OnTreeViewItemExpanded(EventParammeters eventParameters)
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


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
}
