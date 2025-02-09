namespace Model.Models
{
    public partial class FlowDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        public bool IsExpanded;

        public int OrderingNum;

        public virtual List<FlowStepDto> FlowSteps { get; set; } = new List<FlowStepDto>();
        public virtual List<ExecutionDto> Executions { get; set; } = new List<ExecutionDto>();

    }
}
