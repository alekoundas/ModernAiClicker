using Model.Enums;

namespace Model.Models
{
    public partial class FlowDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public FlowTypesEnum Type { get; set; } = FlowTypesEnum.NO_SELECTION;
        public bool IsSelected { get; set; } = true;
        public bool IsExpanded { get; set; } = true;
        public int OrderingNum { get; set; }

        public int FlowParameterId { get; set; }
        public FlowParameterDto? FlowParameter { get; set; }

        public int FlowStepId { get; set; }
        public FlowStepDto? FlowStep { get; set; }

        // Only used when Flow has IsReferenced = false on FlowStep 
        public int? ParentSubFlowStepId { get; set; }
        //public virtual FlowStepDto? ParentSubFlowStep { get; set; }


        //public virtual List<FlowStepDto> SubFlowSteps { get; set; } = new List<FlowStepDto>();
    }
}
