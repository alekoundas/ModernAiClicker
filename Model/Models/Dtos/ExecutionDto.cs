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

        public ExecutionResultEnum ExecutionResultEnum = ExecutionResultEnum.FAIL;


        public string RunFor = "";

        public DateTime? StartedOn;
        public DateTime? EndedOn;
        public int CurrentLoopCount;


        public bool IsSelected;

        public byte[]? ResultImage;
        public string? ResultImagePath;


        public double ResultAccuracy;
        public Point ResultLocation { get; set; }

        public string ExecutionFolderDirectory = "";

        public int? FlowId { get; set; }
        public int? FlowStepId { get; set; }
        public int? ParentExecutionId { get; set; }
        public int? ChildExecutionId { get; set; }

    }

}
