﻿using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;

namespace Business.Repository.Entities
{
    public class FlowParameterRepository : BaseRepository<FlowParameter>, IFlowParameterRepository
    {
        private readonly IDbContextFactory<InMemoryDbContext> _contextFactory;
        private InMemoryDbContext _dbContext { get => _contextFactory.CreateDbContext(); }

        public FlowParameterRepository(IDbContextFactory<InMemoryDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<FlowParameter> GetIsNewSibling(int flowParameterId)
        {
            return await _dbContext.FlowParameters
                .AsNoTracking()
                .Where(x => x.Id == flowParameterId)
                .Select(x => x.ChildrenFlowParameters.First(y => y.Type == FlowParameterTypesEnum.NEW))
                .FirstAsync();
        }

        public async Task<List<FlowParameter>> FindParametersFromFlowStep(int flowStepId)
        {
            var stack = new Stack<FlowStep>();
            List<FlowParameter> flowParameters = new List<FlowParameter>();
            FlowStep flowStep = await _dbContext.FlowSteps
                .AsNoTracking()
                .Where(x => x.Id == flowStepId)
                .FirstAsync();

            stack.Push(flowStep);

            while (stack.Count > 0)
            {
                // Process the current node
                var currentFlowStep = stack.Pop();

                // if FlowStep contains a Flow parent return the parameters.
                if (currentFlowStep?.FlowId != null)
                {
                    Flow? flow = await _dbContext.Flows
                        .AsNoTracking()
                        .Where(x => x.Id == currentFlowStep.FlowId)
                        .Include(x => x.FlowParameter.ChildrenFlowParameters)
                        .Include(x => x.ParentSubFlowStep)
                        .FirstOrDefaultAsync();

                    if (flow?.FlowParameter.ChildrenFlowParameters.Count() > 0)
                        flowParameters.AddRange(flow.FlowParameter.ChildrenFlowParameters);

                    if (flow?.ParentSubFlowStep != null)
                        stack.Push(flow.ParentSubFlowStep);
                }

                // Else load its parent from the database.
                else if (currentFlowStep?.ParentFlowStepId != null)
                {
                    FlowStep? parentFlowStep = await _dbContext.FlowSteps
                        .AsNoTracking()
                        .Where(x => x.Id == currentFlowStep.Id)
                        .Select(x => x.ParentFlowStep)
                        .FirstOrDefaultAsync();

                    // if FlowStep contains a FlowStep parent add it to the stack.
                    stack.Push(parentFlowStep);
                }
            }

            return flowParameters;
        }
    }
}
