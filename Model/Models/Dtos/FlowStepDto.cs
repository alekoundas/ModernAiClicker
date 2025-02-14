using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;

namespace Model.Models
{
    public partial class FlowStepDto : ObservableObject
    {
        public int Id;
        public string Name = string.Empty;
        public string ProcessName = string.Empty;
        public bool IsExpanded;
        public int OrderingNum;
        public FlowStepTypesEnum FlowStepType;
        public bool Disabled { get; set; }

        // Template search
        public byte[] TemplateImage = new byte[0];
        public decimal Accuracy = 0.00m;
        public int LocationX;
        public int LocationY;
        public int MaxLoopCount;
        public bool RemoveTemplateFromResult;


        // Mouse
        public MouseActionsEnum MouseAction;
        public MouseButtonsEnum MouseButton;
        public MouseScrollDirectionEnum MouseScrollDirectionEnum;
        public bool MouseLoopInfinite;
        public int? MouseLoopTimes;
        public int? MouseLoopDebounceTime;
        public TimeOnly? MouseLoopTime;


        //System
        public int? SleepForHours;
        public int? SleepForMinutes;
        public int? SleepForSeconds;
        public int? SleepForMilliseconds;

        // Window
        public int WindowHeight;
        public int WindowWidth;


        public int? FlowId { get; set; }
        public int? ParentFlowStepId { get; set; }

        public int? ParentTemplateSearchFlowStepId;

        public virtual List<FlowStepDto> ChildrenFlowSteps { get; set; } = new List<FlowStepDto>();
        public virtual List<FlowStepDto> ChildrenTemplateSearchFlowSteps { get; set; } = new List<FlowStepDto>();

        //public virtual ObservableCollection<ExecutionDto> Executions { get; set; } = new ObservableCollection<ExecutionDto>();

    }
}
