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
                        .Select(x => x.ChildrenFlowSteps.First(y => y.Type == FlowStepTypesEnum.NEW))
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
                    .SelectMany(x => x.FlowStep.ChildrenFlowSteps)
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
                    .SelectMany(x => x.FlowStep.ChildrenFlowSteps);

            if (simplings != null)
                nextSimpling = await simplings
                   .Where(x => x.Type != FlowStepTypesEnum.NEW)
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
                        .Where(x => x.Type == FlowStepTypesEnum.SUCCESS)
                        .SelectMany(x => x.ChildrenFlowSteps);
                else
                    childrenFlowSteps = childrenFlowSteps
                        .Where(x => x.Type == FlowStepTypesEnum.FAILURE)
                        .SelectMany(x => x.ChildrenFlowSteps);
            }

            FlowStep? nextChild = await childrenFlowSteps
                .Where(x => x.Type != FlowStepTypesEnum.NEW)
                .OrderBy(x => x.OrderingNum)
                .FirstOrDefaultAsync();

            return nextChild;
        }

        public async Task<FlowStep> LoadAllExpandedChildren(FlowStep flowStep)
        {
            // Initialize a stack to simulate recursion.
            var stack = new Stack<FlowStep>();
            stack.Push(flowStep);

            while (stack.Count > 0)
            {
                // Process the current node.
                FlowStep currentFlowStep = stack.Pop();

                // Load its children from the database.
                List<FlowStep> childFlowSteps = await InMemoryDbContext.FlowSteps
                    .Include(x => x.SubFlow)
                    .Where(x => x.Id == currentFlowStep.Id)
                    .SelectMany(x => x.ChildrenFlowSteps)
                    .ToListAsync();

                currentFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(childFlowSteps);


                Flow? subFlow = await InMemoryDbContext.Flows
                    .Include(x=>x.FlowStep).ThenInclude(x=>x.ChildrenFlowSteps)
                    .Include(x=>x.FlowParameter).ThenInclude(x => x.ChildrenFlowParameters)
                    .Where(x => x.Id == currentFlowStep.SubFlowId)
                    .FirstOrDefaultAsync();

                currentFlowStep.SubFlow = subFlow;

                // Push only the expanded children onto the stack for further processing.
                foreach (var childFlowStep in childFlowSteps)
                    if (childFlowStep.IsExpanded)
                        stack.Push(childFlowStep);

                    // Add one more layer to make expander in ui visible.
                    else
                    {
                        List<FlowStep> notexpandedFlowSteps = await InMemoryDbContext.FlowSteps
                            .Where(x => x.Id == childFlowStep.Id)
                            .SelectMany(x => x.ChildrenFlowSteps)
                            .ToListAsync();

                        childFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(notexpandedFlowSteps);
                    }
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
            FlowStep clonedFlowStep = CreateFlowStepClone(originalFlowStep);

            queue.Enqueue((originalFlowStep, clonedFlowStep));
            clonedFlowSteps.Add(originalFlowStep.Id, clonedFlowStep);

            while (queue.Count > 0)
            {
                var (originalNode, clonedNode) = queue.Dequeue();

                // Children flow steps.
                List<FlowStep> originalChildrenFlowSteps = await InMemoryDbContext.FlowSteps
                .Include(fs => fs.ChildrenFlowSteps)
                .Where(fs => fs.Id == originalNode.Id)
                .SelectMany(x => x.ChildrenFlowSteps)
                .ToListAsync();

                foreach (FlowStep child in originalChildrenFlowSteps)
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
                List<FlowStep> originalChildrenTemplateSearchFlowSteps = await InMemoryDbContext.FlowSteps
                .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
                .Where(fs => fs.Id == originalNode.Id)
                .SelectMany(x => x.ChildrenTemplateSearchFlowSteps)
                .ToListAsync();

                foreach (FlowStep child in originalChildrenTemplateSearchFlowSteps)
                    clonedNode.ChildrenTemplateSearchFlowSteps.Add(CreateFlowStepClone(child));
            }

            return clonedFlowStep;

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
                IsSelected = false,
                OrderingNum = flowStep.OrderingNum,
                Type = flowStep.Type,
                TemplateImage = flowStep.TemplateImage,
                Accuracy = flowStep.Accuracy,
                LocationX = flowStep.LocationX,
                LocationY = flowStep.LocationY,
                LoopMaxCount = flowStep.LoopMaxCount,
                RemoveTemplateFromResult = flowStep.RemoveTemplateFromResult,
                CursorAction = flowStep.CursorAction,
                CursorButton = flowStep.CursorButton,
                CursorScrollDirection = flowStep.CursorScrollDirection,
                IsLoopInfinite = flowStep.IsLoopInfinite,
                LoopCount = flowStep.LoopCount,
                LoopTime = flowStep.LoopTime,
                WaitForHours = flowStep.WaitForHours,
                WaitForMinutes = flowStep.WaitForMinutes,
                WaitForSeconds = flowStep.WaitForSeconds,
                WaitForMilliseconds = flowStep.WaitForMilliseconds,
                Height = flowStep.Height,
                Width = flowStep.Width,
            };
        }
    }
}
