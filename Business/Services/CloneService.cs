using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Business.Services
{
    public class CloneService : ICloneService
    {
        private readonly IBaseDatawork _baseDatawork;

        public CloneService(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
        }

        // Existing method for cloning FlowStep (unchanged)
        public async Task<FlowStep?> GetFlowStepClone(int flowStepId)
        {
            // Queues for processing different types
            Queue<(FlowStep Original, FlowStep Cloned, FlowStep? ParentFlowStep, FlowStep? ParentTemplateSearchFlowStep)> flowStepQueue = new();
            Queue<(Flow Original, Flow Cloned)> flowQueue = new();
            Queue<(FlowParameter Original, FlowParameter Cloned, FlowParameter? ParentFlowParameter)> flowParameterQueue = new();

            // Dictionaries to track cloned objects
            Dictionary<int, FlowStep> clonedFlowSteps = new();
            Dictionary<int, Flow> clonedFlows = new();
            Dictionary<int, FlowParameter> clonedFlowParameters = new();

            // Load the source FlowStep with all related data
            FlowStep? originalFlowStep = await _baseDatawork.FlowSteps.Query
                .Include(fs => fs.ChildrenFlowSteps)
                .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
                .Include(fs => fs.SubFlow)
                    .ThenInclude(f => f.FlowParameter)
                .Include(fs => fs.SubFlow)
                    .ThenInclude(f => f.SubFlowSteps)
                .Include(fs => fs.FlowParameter)
                .FirstOrDefaultAsync(fs => fs.Id == flowStepId);

            if (originalFlowStep == null)
                return null;

            // Clone the root FlowStep and enqueue it
            FlowStep clonedFlowStep = CreateFlowStepClone(originalFlowStep);
            flowStepQueue.Enqueue((originalFlowStep, clonedFlowStep, null, null));
            clonedFlowSteps[originalFlowStep.Id] = clonedFlowStep;

            while (flowStepQueue.Count > 0)
            {
                // Process FlowSteps
                await ProcessFlowSteps(flowStepQueue, flowQueue, clonedFlowSteps, clonedFlows);

                // Process Flows
                ProcessFlow(flowStepQueue, flowQueue, flowParameterQueue, clonedFlowSteps, clonedFlowParameters);

                // Process FlowParameters
                ProcessFlowParameter(flowStepQueue, flowQueue, flowParameterQueue, clonedFlowSteps, clonedFlows, clonedFlowParameters);
            }

            return clonedFlowStep;
        }

        // New method for cloning Flow
        public async Task<Flow?> GetFlowClone(int flowId)
        {
            // Queues for processing different types
            Queue<(Flow Original, Flow Cloned)> flowQueue = new();
            Queue<(FlowStep Original, FlowStep Cloned, FlowStep? ParentFlowStep, FlowStep? ParentTemplateSearchFlowStep)> flowStepQueue = new();
            Queue<(FlowParameter Original, FlowParameter Cloned, FlowParameter? ParentFlowParameter)> flowParameterQueue = new();

            // Dictionaries to track cloned objects
            Dictionary<int, Flow> clonedFlows = new();
            Dictionary<int, FlowStep> clonedFlowSteps = new();
            Dictionary<int, FlowParameter> clonedFlowParameters = new();

            // Load the source Flow with all related data
            Flow? originalFlow = await _baseDatawork.Flows.Query
                .Include(f => f.FlowParameter)
                    .ThenInclude(f => f.ChildrenFlowParameters)
                .Include(f => f.FlowStep)
                .Include(f => f.SubFlowSteps)
                .FirstOrDefaultAsync(f => f.Id == flowId);

            if (originalFlow == null)
                return null;

            // Clone the root Flow and enqueue it
            Flow clonedFlow = CreateFlowClone(originalFlow);
            flowQueue.Enqueue((originalFlow, clonedFlow));
            clonedFlows[originalFlow.Id] = clonedFlow;

            while (flowQueue.Count > 0 || flowStepQueue.Count > 0 || flowParameterQueue.Count > 0)
            {
                // Process Flows
                ProcessFlow(flowStepQueue, flowQueue, flowParameterQueue, clonedFlowSteps, clonedFlowParameters);

                // Process FlowParameters
                ProcessFlowParameter(flowStepQueue, flowQueue, flowParameterQueue, clonedFlowSteps, clonedFlows, clonedFlowParameters);

                // Process FlowSteps
                await ProcessFlowSteps(flowStepQueue, flowQueue, clonedFlowSteps, clonedFlows);

            }

            return clonedFlow;
        }

        // Existing helper method (unchanged)
        private async Task ProcessFlowSteps(Queue<(FlowStep Original, FlowStep Cloned, FlowStep? ParentFlowStep, FlowStep? ParentTemplateSearchFlowStep)> flowStepQueue, Queue<(Flow Original, Flow Cloned)> flowQueue, Dictionary<int, FlowStep> clonedFlowSteps, Dictionary<int, Flow> clonedFlows)
        {
            while (flowStepQueue.Count > 0)
            {
                var (originalFS, clonedFS, parentFS, parentTSFS) = flowStepQueue.Dequeue();

                // Clone SubFlow
                if (originalFS.SubFlow != null && originalFS.IsSubFlowReferenced == false)
                {
                    Flow clonedFlow = CreateFlowClone(originalFS.SubFlow);
                    clonedFS.SubFlow = clonedFlow;
                    flowQueue.Enqueue((originalFS.SubFlow, clonedFlow));
                    clonedFlows[originalFS.SubFlow.Id] = clonedFlow;
                }
                else if (originalFS.SubFlow != null)
                {
                    clonedFS.SubFlowId = originalFS.SubFlowId;
                }

                if (originalFS.FlowParameterId != null)
                    clonedFS.FlowParameterId = originalFS.FlowParameterId;

                // Process ChildrenFlowSteps
                var childrenFlowSteps = await _baseDatawork.FlowSteps.Query
                    .Include(fs => fs.ChildrenFlowSteps)
                    .Where(fs => fs.Id == originalFS.Id)
                    .SelectMany(x => x.ChildrenFlowSteps)
                    .ToListAsync();

                foreach (var child in childrenFlowSteps)
                {
                    FlowStep? clonedParentTSFS = child.ParentTemplateSearchFlowStepId.HasValue
                        ? clonedFlowSteps.GetValueOrDefault(child.ParentTemplateSearchFlowStepId.Value)
                        : null;

                    FlowStep clonedChild = CreateFlowStepClone(child, clonedFS, clonedParentTSFS);
                    clonedFS.ChildrenFlowSteps.Add(clonedChild);
                    flowStepQueue.Enqueue((child, clonedChild, clonedFS, clonedParentTSFS));
                    clonedFlowSteps[child.Id] = clonedChild;
                }

                // Process ChildrenTemplateSearchFlowSteps
                var childrenTSFlowSteps = await _baseDatawork.FlowSteps.Query
                    .Include(fs => fs.ChildrenTemplateSearchFlowSteps)
                    .Where(fs => fs.Id == originalFS.Id)
                    .SelectMany(x => x.ChildrenTemplateSearchFlowSteps)
                    .ToListAsync();

                foreach (var child in childrenTSFlowSteps)
                {
                    FlowStep clonedChild = CreateFlowStepClone(child);
                    clonedFS.ChildrenTemplateSearchFlowSteps.Add(clonedChild);
                    flowStepQueue.Enqueue((child, clonedChild, null, null));
                    clonedFlowSteps[child.Id] = clonedChild;
                }
            }
        }

        // Modified helper method to handle SubFlowSteps
        private void ProcessFlow(Queue<(FlowStep Original, FlowStep Cloned, FlowStep? ParentFlowStep, FlowStep? ParentTemplateSearchFlowStep)> flowStepQueue, Queue<(Flow Original, Flow Cloned)> flowQueue, Queue<(FlowParameter Original, FlowParameter Cloned, FlowParameter? ParentFlowParameter)> flowParameterQueue, Dictionary<int, FlowStep> clonedFlowSteps, Dictionary<int, FlowParameter> clonedFlowParameters)
        {
            while (flowQueue.Count > 0)
            {
                var (originalF, clonedF) = flowQueue.Dequeue();

                // Clone FlowParameter
                if (originalF.FlowParameter != null && !clonedFlowParameters.ContainsKey(originalF.FlowParameter.Id))
                {
                    FlowParameter clonedFP = CreateFlowParameterClone(originalF.FlowParameter);
                    clonedF.FlowParameter = clonedFP;
                    flowParameterQueue.Enqueue((originalF.FlowParameter, clonedFP, null));
                    clonedFlowParameters[originalF.FlowParameter.Id] = clonedFP;
                }

                // Clone FlowStep
                if (originalF.FlowStep != null && !clonedFlowSteps.ContainsKey(originalF.FlowStep.Id))
                {
                    FlowStep clonedFS = CreateFlowStepClone(originalF.FlowStep);
                    clonedF.FlowStep = clonedFS;
                    flowStepQueue.Enqueue((originalF.FlowStep, clonedFS, null, null));
                    clonedFlowSteps[originalF.FlowStep.Id] = clonedFS;
                }

                //// Clone SubFlowSteps (added to support Flow cloning)
                //foreach (var subFlowStep in originalF.SubFlowSteps)
                //{
                //    if (!clonedFlowSteps.ContainsKey(subFlowStep.Id))
                //    {
                //        FlowStep clonedSubFS = CreateFlowStepClone(subFlowStep);
                //        clonedF.SubFlowSteps.Add(clonedSubFS);
                //        flowStepQueue.Enqueue((subFlowStep, clonedSubFS, null, null));
                //        clonedFlowSteps[subFlowStep.Id] = clonedSubFS;
                //    }
                //}
            }
        }

        // Existing helper method (unchanged)
        private void ProcessFlowParameter(Queue<(FlowStep Original, FlowStep Cloned, FlowStep? ParentFlowStep, FlowStep? ParentTemplateSearchFlowStep)> flowStepQueue, Queue<(Flow Original, Flow Cloned)> flowQueue, Queue<(FlowParameter Original, FlowParameter Cloned, FlowParameter? ParentFlowParameter)> flowParameterQueue, Dictionary<int, FlowStep> clonedFlowSteps, Dictionary<int, Flow> clonedFlows, Dictionary<int, FlowParameter> clonedFlowParameters)
        {
            while (flowParameterQueue.Count > 0)
            {
                var (originalFP, clonedFP, parentFP) = flowParameterQueue.Dequeue();

                clonedFP.ParentFlowParameter = parentFP;

                // Clone Flow
                if (originalFP.Flow != null && !clonedFlows.ContainsKey(originalFP.Flow.Id))
                {
                    Flow clonedFlow = CreateFlowClone(originalFP.Flow);
                    clonedFP.Flow = clonedFlow;
                    flowQueue.Enqueue((originalFP.Flow, clonedFlow));
                    clonedFlows[originalFP.Flow.Id] = clonedFlow;
                }
                else if (originalFP.Flow != null)
                {
                    clonedFP.Flow = clonedFlows[originalFP.Flow.Id];
                }


                // Clone ChildrenFlowParameters
                foreach (var childFP in originalFP.ChildrenFlowParameters)
                {
                    if (!clonedFlowParameters.ContainsKey(childFP.Id))
                    {
                        FlowParameter clonedChildFP = CreateFlowParameterClone(childFP);
                        clonedFP.ChildrenFlowParameters.Add(clonedChildFP);
                        flowParameterQueue.Enqueue((childFP, clonedChildFP, clonedFP));
                        clonedFlowParameters[childFP.Id] = clonedChildFP;
                    }
                    else
                    {
                        clonedFP.ChildrenFlowParameters.Add(clonedFlowParameters[childFP.Id]);
                    }
                }
            }
        }

        // Existing clone methods (unchanged)
        private FlowStep CreateFlowStepClone(FlowStep original, FlowStep? parentFlowStep = null, FlowStep? parentTemplateSearchFlowStep = null)
        {
            return new FlowStep
            {
                ParentFlowStep = parentFlowStep,
                ParentTemplateSearchFlowStep = parentTemplateSearchFlowStep,

                Name = original.Name,
                ProcessName = original.ProcessName,
                IsExpanded = original.IsExpanded,
                IsSelected = false,
                OrderingNum = original.OrderingNum,
                Type = original.Type,
                TemplateMatchMode = original.TemplateMatchMode,
                IsSubFlowReferenced = original.IsSubFlowReferenced,
                TemplateImage = original.TemplateImage != null ? (byte[])original.TemplateImage.Clone() : null,
                Accuracy = original.Accuracy,
                RemoveTemplateFromResult = original.RemoveTemplateFromResult,
                CursorAction = original.CursorAction,
                CursorButton = original.CursorButton,
                CursorScrollDirection = original.CursorScrollDirection,
                WaitForHours = original.WaitForHours,
                WaitForMinutes = original.WaitForMinutes,
                WaitForSeconds = original.WaitForSeconds,
                WaitForMilliseconds = original.WaitForMilliseconds,
                Height = original.Height,
                Width = original.Width,
                IsLoop = original.IsLoop,
                IsLoopInfinite = original.IsLoopInfinite,
                LoopCount = original.LoopCount,
                LoopMaxCount = original.LoopMaxCount,
                LoopTime = original.LoopTime,
                LocationX = original.LocationX,
                LocationY = original.LocationY,
            };
        }

        private Flow CreateFlowClone(Flow original)
        {
            return new Flow
            {
                Name = original.Name,
                Type = original.Type,
                IsSelected = false,
                IsExpanded = original.IsExpanded,
                OrderingNum = original.OrderingNum
            };
        }

        private FlowParameter CreateFlowParameterClone(FlowParameter original)
        {
            return new FlowParameter
            {
                Name = original.Name,
                IsExpanded = original.IsExpanded,
                IsSelected = false,
                OrderingNum = original.OrderingNum,
                Type = original.Type,
                TemplateSearchAreaType = original.TemplateSearchAreaType,
                ProcessName = original.ProcessName,
                SystemMonitorDeviceName = original.SystemMonitorDeviceName,
                LocationTop = original.LocationTop,
                LocationLeft = original.LocationLeft,
                LocationRight = original.LocationRight,
                LocationBottom = original.LocationBottom
            };
        }
    }
}