using Business.DatabaseContext;
using Business.Repository.Interfaces;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;

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
                        .Include(x => x.FlowSteps)
                        .Where(x => x.Id == id)
                        .Select(x => x.FlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW))
                        .FirstAsync();
        }

        public async Task<List<Flow>> LoadAllExpanded()
        {
            List<Flow> flows= await InMemoryDbContext.Flows
                .Include(x => x.FlowSteps)
                .ToListAsync();

            foreach (Flow flow in flows)
            {
                flow.IsExpanded = true;

                foreach (FlowStep flowStep in flow.FlowSteps)
                    await LoadAllChildren(flowStep,true);
            }

            return flows;
        }

        public async Task<List<Flow>> LoadAllCollapsed()
        {
            List<Flow> flows = await InMemoryDbContext.Flows
                .Include(x => x.FlowSteps)
                .ToListAsync();

            foreach (Flow flow in flows)
            {
                flow.IsExpanded = false;

                foreach (FlowStep flowStep in flow.FlowSteps)
                    await LoadAllChildren(flowStep,false);
            }

            return flows;
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
                    .ToListAsync();

                currentFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(childFlowSteps);
                currentFlowStep.IsExpanded = isExpanded;

                // Push children onto the stack for further processing.
                foreach (var childFlowStep in childFlowSteps)
                    stack.Push(childFlowStep);
            }

            return flowStep;
        }

    }
}
