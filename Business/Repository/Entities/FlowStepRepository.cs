using Business.DatabaseContext;
using Business.Repository.Interfaces;
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
            //TODO remove 1st call to db.
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

            flowStep = await InMemoryDbContext.FlowSteps
                .Where(x => x.Id == flowStep.Id)
                .Include(x => x.ChildrenTemplateSearchFlowSteps)
                .Include(x => x.FlowParameter)
                .Include(x => x.SubFlow!.FlowStep)
                .Include(x => x.SubFlow!.FlowParameter.ChildrenFlowParameters)
                .FirstAsync();

            // Initialize a stack to simulate recursion.
            var stack = new Stack<FlowStep>();
            stack.Push(flowStep);

            while (stack.Count > 0)
            {
                // Process the current node.
                FlowStep currentFlowStep = stack.Pop();

                // Load its children from the database.
                var childFlowSteps = await InMemoryDbContext.FlowSteps
                  .Where(x => x.Id == currentFlowStep.Id)
                  .SelectMany(x => x.ChildrenFlowSteps)
                  .Include(x => x.ChildrenTemplateSearchFlowSteps)
                  .Include(x => x.FlowParameter)
                  .Include(x => x.SubFlow!.FlowStep)
                  .Include(x => x.SubFlow!.FlowParameter.ChildrenFlowParameters)
                  .ToListAsync();

                currentFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(childFlowSteps);

                // Load Sub-Flows if available and not referenced.
                if (!currentFlowStep.IsSubFlowReferenced && currentFlowStep.SubFlow != null)
                    stack.Push(currentFlowStep.SubFlow.FlowStep);
                else
                    currentFlowStep.SubFlow = null;

                // Push only the expanded children onto the stack for further processing.
                foreach (var childFlowStep in childFlowSteps)
                    if (childFlowStep.IsExpanded)
                        stack.Push(childFlowStep);

                    //TODO this is propably tooo slow
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

        public async Task<FlowStep?> LoadAllClone(int id)
        {
            FlowStep? flowStep = await InMemoryDbContext.FlowSteps
                .Where(x => x.Id == id)
                   .Include(x => x.ChildrenTemplateSearchFlowSteps)
                   .Include(x => x.FlowParameter)
                   .Include(x => x.SubFlow!.FlowStep)
                   .Include(x => x.SubFlow!.FlowParameter.ChildrenFlowParameters)
                   .FirstOrDefaultAsync();
            if (flowStep == null)
                return null;

            await LoadAllChildrenExport(flowStep);
            return flowStep;
        }

        private async Task<FlowStep> LoadAllChildrenExport(FlowStep flowStep)
        {
            // Initialize a stack to simulate recursion.
            var stack = new Stack<FlowStep>();
            if (flowStep.SubFlow.FlowStep != null)
                stack.Push(flowStep.SubFlow.FlowStep);
            stack.Push(flowStep);

            while (stack.Count > 0)
            {
                // Process the current node
                var currentFlowStep = stack.Pop();

                // Load its children from the database.
                var childFlowSteps = await InMemoryDbContext.FlowSteps
                    .Where(x => x.Id == currentFlowStep.Id)
                    .SelectMany(x => x.ChildrenFlowSteps)
                    .Include(x => x.ChildrenTemplateSearchFlowSteps)
                    .Include(x => x.FlowParameter)
                    .Include(x => x.SubFlow!.FlowStep)
                    .Include(x => x.SubFlow!.FlowParameter.ChildrenFlowParameters)
                    .ToListAsync();

                currentFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(childFlowSteps);
                currentFlowStep.IsExpanded = true;

                // Push children onto the stack for further processing.
                foreach (var childFlowStep in childFlowSteps)
                    stack.Push(childFlowStep);

                foreach (var subFlowtep in childFlowSteps.Select(x => x.SubFlow?.FlowStep).ToList())
                    if (subFlowtep != null)
                        stack.Push(subFlowtep);
            }

            return flowStep;
        }
    }
}
