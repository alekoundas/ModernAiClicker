using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;

namespace Model.Models
{
    public partial class ExecutionDto 
    {
        public int Id { get; set; }

        public bool IsExecuted;

        public ExecutionStatusEnum Status = ExecutionStatusEnum.DASH;

        public string RunFor = "";

        public DateTime StartedOn = new DateTime();

        public string RemainingSteps = "";

        public int? FlowId { get; set; }
        public int? FlowStepId { get; set; }
        public int? ParentExecutionId { get; set; }
        public int? ChildExecutionId { get; set; }

    }

}
