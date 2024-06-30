using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using Model.Structs;

namespace Model.Models
{
    public partial class ExecutionDto 
    {
        public int Id { get; set; }

        public bool IsExecuted;


        public ExecutionStatusEnum Status = ExecutionStatusEnum.DASH;

        public string RunFor = "";

        public DateTime? StartedOn;
        public DateTime? EndedOn;


        public string RemainingSteps = "";
        public bool IsSelected;
        public bool IsSuccessful { get; set; }
        public Point ResultLocation { get; set; }


        public int? FlowId { get; set; }
        public int? FlowStepId { get; set; }
        public int? ParentExecutionId { get; set; }
        public int? ChildExecutionId { get; set; }

    }

}
