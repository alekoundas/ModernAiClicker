using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Business.Repository.Entities
{
    public class FlowRepository : BaseRepository<Flow>, IFlowRepository
    {
        public FlowRepository(InMemoryDbContext dbContext) : base(dbContext)
        {
        }

        public InMemoryDbContext InMemoryDbContext
        {
            get { return Context as InMemoryDbContext; }
        }

        public async Task<FlowStep> GetIsNewSibling(int id)
        {
            return await InMemoryDbContext.Flows
                        .Include(x => x.FlowStep.ChildrenFlowSteps)
                        .Where(x => x.Id == id)
                        .Select(x => x.FlowStep.ChildrenFlowSteps.First(y => y.Type == FlowStepTypesEnum.NEW))
                        .FirstAsync();
        }

        public async Task<List<Flow>> LoadAllExpanded()
        {
            List<Flow> flows = await InMemoryDbContext.Flows
                .Include(x => x.FlowStep.ChildrenFlowSteps)
                .ToListAsync();

            foreach (Flow flow in flows)
            {
                flow.IsExpanded = true;

                foreach (FlowStep flowStep in flow.FlowStep.ChildrenFlowSteps)
                    await LoadAllChildren(flowStep, true);
            }

            return flows;
        }

        public async Task<List<Flow>> LoadAllCollapsed()
        {
            List<Flow> flows = await InMemoryDbContext.Flows
                .Include(x => x.FlowStep.ChildrenFlowSteps)
                .ToListAsync();

            foreach (Flow flow in flows)
            {
                flow.IsExpanded = false;

                foreach (FlowStep flowStep in flow.FlowStep.ChildrenFlowSteps)
                    await LoadAllChildren(flowStep, false);
            }

            return flows;
        }

        public async Task<List<Flow>> LoadAllExport(int? flowId = null)
        {
            List<Flow> flows = new List<Flow>();

            if (flowId.HasValue)
                flows = await InMemoryDbContext.Flows
                    .Include(x => x.FlowParameter.ChildrenFlowParameters)
                    .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.FlowParameter)
                    .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.SubFlow!.FlowStep)
                    .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.SubFlow!.FlowParameter.ChildrenFlowParameters)

                    .Where(x => x.Id == flowId)
                    .ToListAsync();
            else
                flows = await InMemoryDbContext.Flows
                    .Include(x => x.FlowParameter.ChildrenFlowParameters)
                    .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.FlowParameter)
                    .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.SubFlow!.FlowStep)
                    .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.SubFlow!.FlowParameter.ChildrenFlowParameters)
                    .ToListAsync();

            foreach (Flow flow in flows)
                foreach (FlowStep flowStep in flow.FlowStep.ChildrenFlowSteps)
                    await LoadAllChildrenExport(flowStep);

            return flows;
        }

        //public async Task FixOneToOneRelationIds(int flowId)
        //{
        //    List<Flow> flows = await InMemoryDbContext.Flows
        //        .AsNoTracking()
        //        .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.SubFlow!.FlowStep)
        //        .Where(x => x.Type == FlowTypesEnum.FLOW)
        //        .ToListAsync();

        //    foreach (var flow in flows)
        //    {

        //        if (flow != null)
        //        {
        //            var stack = new Stack<FlowStep>(flow.FlowStep.ChildrenFlowSteps);
        //            while (stack.Count > 0)
        //            {
        //                // Process the current node
        //                var currentFlowStep = stack.Pop();

        //                // Load its children from the database.
        //                var childFlowSteps = await InMemoryDbContext.FlowSteps
        //                    .AsNoTracking()
        //                    .Where(x => x.Id == currentFlowStep.Id)
        //                    .SelectMany(x => x.ChildrenFlowSteps)
        //                    .Include(x => x.SubFlow!.FlowStep)
        //                    .ToListAsync();

        //                // Do the actual fix!
        //                if (currentFlowStep.SubFlow != null && currentFlowStep.IsSubFlowReferenced == false)
        //                {
        //                    Flow updateFlow = currentFlowStep.SubFlow;
        //                    updateFlow.ParentSubFlowStepId = currentFlowStep.Id;
        //                    InMemoryDbContext.Update(updateFlow);
        //                }

        //                // Push children onto the stack for further processing.
        //                foreach (var childFlowStep in childFlowSteps)
        //                    stack.Push(childFlowStep);

        //                foreach (var subFlowtep in childFlowSteps.Select(x => x.SubFlow?.FlowStep).ToList())
        //                    if (subFlowtep != null)
        //                        stack.Push(subFlowtep);

        //            }
        //        }
        //    }
        //    await InMemoryDbContext.SaveChangesAsync();
        //}

        public async Task FixOneToOneRelationIds(int flowId)
        {
            Flow? flow = await InMemoryDbContext.Flows
                .AsNoTracking()
                .Include(x => x.FlowStep.ChildrenFlowSteps).ThenInclude(x => x.SubFlow!.FlowStep)
                .Where(x => x.Id == flowId)
                .FirstOrDefaultAsync();

            if (flow != null)
            {
                var stack = new Stack<FlowStep>(flow.FlowStep.ChildrenFlowSteps);
                while (stack.Count > 0)
                {
                    // Process the current node
                    var currentFlowStep = stack.Pop();

                    // Load its children from the database.
                    var childFlowSteps = await InMemoryDbContext.FlowSteps
                        .AsNoTracking()
                        .Where(x => x.Id == currentFlowStep.Id)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .Include(x => x.SubFlow!.FlowStep)
                        .ToListAsync();

                    // Do the actual fix!
                    if (currentFlowStep.SubFlow != null && currentFlowStep.IsSubFlowReferenced == false)
                    {
                        Flow updateFlow = await InMemoryDbContext.Flows.FirstAsync(x=>x.Id==currentFlowStep.SubFlowId);
                        updateFlow.ParentSubFlowStepId = currentFlowStep.Id;
                    }

                    // Push children onto the stack for further processing.
                    foreach (var childFlowStep in childFlowSteps)
                        stack.Push(childFlowStep);

                    foreach (var subFlowtep in childFlowSteps.Select(x => x.SubFlow?.FlowStep).ToList())
                        if (subFlowtep != null)
                            stack.Push(subFlowtep);

                }
            }
            await InMemoryDbContext.SaveChangesAsync();
        }

        private async Task<FlowStep> LoadAllChildren(FlowStep flowStep, bool isExpanded)
        {
            // Initialize a stack to simulate recursion.
            var stack = new Stack<FlowStep>();
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
                currentFlowStep.IsExpanded = isExpanded;

                // Push children onto the stack for further processing.
                foreach (var childFlowStep in childFlowSteps)
                    stack.Push(childFlowStep);
            }

            return flowStep;
        }

        private async Task<FlowStep> LoadAllChildrenExport(FlowStep flowStep)
        {
            // Initialize a stack to simulate recursion.
            var stack = new Stack<FlowStep>();
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
