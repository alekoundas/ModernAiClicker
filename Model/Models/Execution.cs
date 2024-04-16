using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;

namespace Model.Models
{
    public partial class Execution : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        public bool _isSelected = false;

        [ObservableProperty]
        public bool _isExpanded = true;

        [ObservableProperty]
        public bool _isExecuted = false;

        [ObservableProperty]
        public ExecutionStatusEnum _status = ExecutionStatusEnum.DASH;

        [ObservableProperty]
        public string _runFor = "";

        [ObservableProperty]
        public string _remainingSteps = "";


        public int? FlowId { get; set; }
        public Flow? Flow { get; set; }

        public int? FlowStepId { get; set; }
        public FlowStep? FlowStep { get; set; }

    }

}
