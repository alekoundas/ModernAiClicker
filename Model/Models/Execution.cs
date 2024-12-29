using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using Model.Structs;

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

        [ObservableProperty]
        public int? _currentLoopCount;

        //public Point _resultLocation;


        public int? ResultLocationX { get; set; }
        public int? ResultLocationY { get; set; }

        [ObservableProperty]
        public byte[]? _resultImage;

        [ObservableProperty]
        public string? _resultImagePath;

        [ObservableProperty]
        public decimal _resultAccuracy= 0.00m;

        public string ExecutionFolderDirectory="";

        public int? FlowId { get; set; }
        public virtual Flow? Flow { get; set; }

        public int? FlowStepId { get; set; }
        public virtual FlowStep? FlowStep { get; set; }

        public int? ParentExecutionId { get; set; }
        public virtual Execution? ParentExecution { get; set; }

        public int? ChildExecutionId { get; set; }
        public virtual Execution? ChildExecution { get; set; }
    }
}
