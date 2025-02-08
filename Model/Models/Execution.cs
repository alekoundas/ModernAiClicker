using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;

namespace Model.Models
{
    public partial class Execution : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        public bool _isExecuted = false;

        [ObservableProperty]
        public ExecutionStatusEnum _status = ExecutionStatusEnum.DASH;

        [ObservableProperty]
        public ExecutionResultEnum _executionResultEnum = ExecutionResultEnum.NO_RESULT;

        [ObservableProperty]
        public string _runFor = "";

        [ObservableProperty]
        public DateTime? _startedOn;

        [ObservableProperty]
        public DateTime? _endedOn;

        [ObservableProperty]
        public bool _isSelected = true;

        public string ExecutionFolderDirectory = "";

        // Template properties.
        [ObservableProperty]
        public int? _loopCount;

        [ObservableProperty]
        public int? _resultLocationX;

        [ObservableProperty]
        public int? _resultLocationY;

        [ObservableProperty]
        public byte[]? _resultImage;

        [ObservableProperty]
        public string? _resultImagePath;

        [ObservableProperty]
        public decimal _resultAccuracy = 0.00m;

        // Navigation properties.    
        public int? FlowId { get; set; }
        public virtual Flow? Flow { get; set; }

        public int? FlowStepId { get; set; }
        public virtual FlowStep? FlowStep { get; set; }

        public int? ParentExecutionId { get; set; }
        public virtual Execution? ParentExecution { get; set; }

        public int? ChildExecutionId { get; set; }
        public virtual Execution? ChildExecution { get; set; }

        // Loop and Multiple flow step fields
        public int? ParentLoopExecutionId { get; set; }
        public virtual Execution? ParentLoopExecution { get; set; }

        public int? ChildLoopExecutionId { get; set; }
        public virtual Execution? ChildLoopExecution { get; set; }
    }
}
