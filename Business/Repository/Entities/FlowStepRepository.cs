using Business.DatabaseContext;
using Business.Repository.Interfaces;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;

namespace Business.Repository.Entities
{
    public class FlowStepRepository : BaseRepository<FlowStep>, IFlowStepRepository
    {
        public FlowStepRepository(InMemoryDbContext dbContext) : base(dbContext)
        {
        }

        public InMemoryDbContext InMemoryDbContext
        {
            get { return Context as InMemoryDbContext; }
        }

        public async Task<FlowStep> GetIsNewSibling(int flowStepId)
        {
            return await InMemoryDbContext.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStepId)
                        .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW))
                        .FirstAsync();
        }

        public async Task<List<FlowStep>> GetSiblings(int flowStepId)
        {
            List<FlowStep> simplings = new List<FlowStep>();
            FlowStep flowStep = await InMemoryDbContext.FlowSteps.FirstAsync(x => x.Id == flowStepId);

            if (flowStep.ParentFlowStepId.HasValue)
                simplings = await InMemoryDbContext.FlowSteps
                    .Where(x => x.Id == flowStep.ParentFlowStepId.Value)
                    .SelectMany(x => x.ChildrenFlowSteps)
                    .ToListAsync();
            else if (flowStep.FlowId.HasValue)
                simplings = await InMemoryDbContext.Flows
                    .Where(x => x.Id == flowStep.FlowId.Value)
                    .SelectMany(x => x.FlowSteps)
                    .ToListAsync();

            return simplings;
        }

        public async Task<FlowStep?> GetNextSibling(int flowStepId)
        {
            FlowStep? nextSimpling = null;
            IQueryable<FlowStep>? simplings = null;
            FlowStep flowStep = await InMemoryDbContext.FlowSteps.AsNoTracking().FirstAsync(x => x.Id == flowStepId);

            if (flowStep.ParentFlowStepId.HasValue)
                simplings = InMemoryDbContext.FlowSteps
                    .AsNoTracking()
                    .Where(x => x.Id == flowStep.ParentFlowStepId.Value)
                    .SelectMany(x => x.ChildrenFlowSteps);

            else if (flowStep.FlowId.HasValue)
                simplings = InMemoryDbContext.Flows
                    .AsNoTracking()
                    .Where(x => x.Id == flowStep.FlowId.Value)
                    .SelectMany(x => x.FlowSteps);

            if (simplings != null)
                nextSimpling = await simplings
                   .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                   .Where(x => x.OrderingNum > flowStep.OrderingNum)
                   .OrderBy(x => x.OrderingNum)
                   .FirstOrDefaultAsync();

            return nextSimpling;
        }

        public async Task<FlowStep?> GetNextChild(int flowStepId, ExecutionResultEnum? resultEnum)
        {
            IQueryable<FlowStep> childrenFlowSteps = InMemoryDbContext.FlowSteps
                .AsNoTracking()
                .Where(x => x.Id == flowStepId)
                .SelectMany(x => x.ChildrenFlowSteps);

            if (resultEnum.HasValue)
            {
                if (resultEnum == ExecutionResultEnum.SUCCESS)
                    childrenFlowSteps = childrenFlowSteps
                        .Where(x => x.FlowStepType == FlowStepTypesEnum.IS_SUCCESS) // Equals to .First() since only one child will be available.
                        .SelectMany(x => x.ChildrenFlowSteps);
                else
                    childrenFlowSteps = childrenFlowSteps
                        .Where(x => x.FlowStepType == FlowStepTypesEnum.IS_FAILURE) // Equals to .First() since only one child will be available.
                        .SelectMany(x => x.ChildrenFlowSteps);
            }

            FlowStep? nextChild = await childrenFlowSteps
                .Where(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW)
                .OrderBy(x => x.OrderingNum)
                .FirstOrDefaultAsync();

            return nextChild;
        }

        public async Task<FlowStep> LoadAllChildren(FlowStep flowStep)
        {
            // Initialize a stack to simulate recursion.
            var stack = new Stack<FlowStep>();
            stack.Push(flowStep);

            while (stack.Count > 0)
            {
                // Process the current node.
                var currentFlowStep = stack.Pop();

                // Load its children from the database.
                var childFlowSteps = await InMemoryDbContext.FlowSteps
                    .Where(x => x.Id == currentFlowStep.Id)
                    .SelectMany(x => x.ChildrenFlowSteps)
                    .ToListAsync();

                currentFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(childFlowSteps);

                // Push only the expanded children onto the stack for further processing.
                foreach (var childFlowStep in childFlowSteps)
                    if (childFlowStep.IsExpanded)
                        stack.Push(childFlowStep);
            }

            return flowStep;
        }

        public async Task<FlowStep?> GetFlowStepClone(int flowStepId)
        {
            Queue<(FlowStep, FlowStep)> queue = new Queue<(FlowStep, FlowStep)>();
            Dictionary<int, FlowStep> clonedFlowSteps = new Dictionary<int, FlowStep>();

            // Step 1: Load the source branch (including its children and template search relationships)
            FlowStep? originalFlowStep = await InMemoryDbContext.FlowSteps
                .Include(fs => fs.ChildrenFlowSteps)
                .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
                .FirstOrDefaultAsync(fs => fs.Id == flowStepId);

            if (originalFlowStep == null)
                return null;


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
                var originalChildrenFlowSteps = await InMemoryDbContext.FlowSteps
                .Include(fs => fs.ChildrenFlowSteps)
                .Where(fs => fs.Id == originalNode.Id)
                .SelectMany(x => x.ChildrenFlowSteps)
                .ToListAsync();

                // Template search flow steps.
                var originalChildrenTemplateSearchFlowSteps = await InMemoryDbContext.FlowSteps
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
    }
}
